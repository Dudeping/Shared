using Microsoft.Ajax.Utilities;
using PagedList;
using ShareWeb.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShareWeb.Controllers
{
    [Authorize]
    public class UploadShareController : BaseController
    {
        // GET: /UploadShare/CategoryList
        // 分享类别列表
        public ActionResult CategoryList(int page = 1)
        {
            if (User.IsInRole("Administrator"))
            {
                ViewBag.CategoryList = db.Categories.OrderByDescending(p => p.Id).ToPagedList(page, 12);
                return View();
            }

            ViewBag.CategoryList = db.Categories.Where(p => p.Author == User.Identity.Name).OrderByDescending(p => p.Id).ToPagedList(page, 12);
            return View();
        }

        // POST: /UploadShare/AddCategory
        // 添加分享类别
        // model: 类别信息
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCategory(CategoryModel model)
        {
            if (ModelState.IsValid)
            {
                if (CheckVerifyCode(model.Code))
                {
                    LogHelper.Log(User.Identity.Name, LogType.error.ToString(), "添加类别失败", "验证码错误！", IpHelper.GetIp());
                    return Content("<Script>alert('验证码错误!'); location.href='/UploadShare/CategoryList'</Script>");
                }

                Category entity = new Category();
                entity.Code = model.Code;
                entity.Name = model.Name;
                entity.Author = User.Identity.Name;

                db.Categories.Add(entity);
                db.SaveChanges();

                return Content("<Script>alert('添加成功!');location.href='/UploadShare/CategoryList'</Script>");
            }
            return Content("<Script>alert('操作错误!请重试.'); location.href='/UploadShare/CategoryList'</Script>");
        }

        // GET: /UploadShare/EditCategory
        // 编辑分享类别
        // id: 该类别Id
        [HttpGet]
        public ActionResult EditCategory(int id)
        {
            var model = db.Categories.Find(id);
            if (model != null)
            {
                var data = new CategoryModel();
                data.Id = model.Id;
                data.Name = model.Name;
                return View(data);
            }
            return Content("操作失败!请重试.");
        }

        // POST: /UploadShare/EditCategory
        // 编辑分享类别
        // id: 该类别Id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCategory(CategoryModel model)
        {
            if (ModelState.IsValid)
            {
                if (CheckVerifyCode(model.Code))
                {
                    ModelState.AddModelError("Code", "验证码错误!");
                    model.Code = "";
                    return View(model);
                }

                var entity = db.Categories.Find(model.Id);
                if (entity.Author != User.Identity.Name)
                {
                    return Content("<Script>alert('操作异常!');location.href='/UploadShare/CategoryList'</Script>");
                }
                entity.Code = model.Code;
                entity.Name = model.Name;
                db.Entry<Category>(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return Content("<Script>alert('编辑成功!');location.href='/UploadShare/CategoryList'</Script>");
            }
            model.Code = "";
            return View(model);
        }

        // POST: /UploadShare/DeleteCategory
        // 删除分享类别
        // id: 该分享类别Id
        [HttpPost]
        public ActionResult DeleteCategory(int id)
        {
            var model = db.Categories.Find(id);
            if (model.Author != User.Identity.Name)
            {
                return Json(false);
            }
            if (model!= null)
            {
                var entity = db.Categories.Where(p => p.Name == "其他").FirstOrDefault();
                foreach (var item in model.ShareModels)
                {
                    item.Category = entity;
                    db.Entry<ShareModel>(item).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                db.Categories.Remove(model);
                db.SaveChanges();
                return Json(true);
            }
            return Json(false);
        }
        
        // GET: /UploadShare/Shalist
        // 分享列表
        // category: 列表的类别
        public ActionResult ShareList(string category, int page = 1)
        {
            ViewBag.Category = category;
            if (category != null)
            {
                var model = db.Categories.Where(p => p.Name == category).FirstOrDefault();

                if (User.IsInRole("Administrator"))
                {
                    return View(db.ShareModels.Where(x => x.Category.Name == category).OrderByDescending(p => p.Id).Select(p => new ManageShare { Id = p.Id, Category = p.Category, Name = p.Name }).ToPagedList(page, 12));
                }
                return View(db.ShareModels.Where(x => x.Category.Name == category && x.Editor.Email == User.Identity.Name).OrderByDescending(p => p.Id).Select(p => new ManageShare { Id = p.Id, Category = p.Category, Name = p.Name }).ToPagedList(page, 12));
            }
            if (User.IsInRole("Administrator"))
            {
                return View(db.ShareModels.OrderByDescending(p => p.Id).Select(p => new ManageShare { Id = p.Id, Category = p.Category, Name = p.Name }).ToPagedList(page, 12));
            }
            return View(db.ShareModels.Where(p => p.Editor.Email == User.Identity.Name).OrderByDescending(p => p.Id).Select(p => new ManageShare { Id = p.Id, Category = p.Category, Name = p.Name }).ToPagedList(page, 12));
        }

        // GET: /UploadShare/AddShare
        // 添加分享
        [HttpGet]
        public ActionResult AddShare()
        {
            var user = db.Users.Where(p => p.Email == User.Identity.Name).FirstOrDefault();
            if (user != null)
            {
                if (user.UpdowningTime.AddDays(1) < DateTime.Now)
                {
                    user.UpdownTimes = 0;
                    user.Code = "1234";
                    db.Entry<User>(user).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }

                if (user.UpdownTimes > 10 && user.UpdowningTime.AddDays(1) < DateTime.Now)
                {
                    return Content("<Script>alert('操作频繁!每天只能上传10个分享.');location.href = '/UploadShare/AddShare';</Script>");
                }
            }

            ViewBag.Category = (from x in db.Categories orderby x.Name.Length descending select x.Name).ToList();
            return View();
        }

        // POST: /UploadShare/AddShare
        // 添加分享
        // model: 该分享数据
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddShare(MShare model)
        {
            ViewBag.Category = (from x in db.Categories orderby x.Name.Length descending select x.Name).ToList();
            if (ModelState.IsValid)
            {
                if (CheckVerifyCode(model.Code))
                {
                    ModelState.AddModelError("Code", "验证码错误!重新提交请注意选择类别.");
                    model.Code = "";
                    LogHelper.Log(User.Identity.Name, LogType.error.ToString(), "编辑分享失败", "添加分享失败！验证码错误。", IpHelper.GetIp());
                    return View(model);
                }

                var user = db.Users.Where(p => p.Email == User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    if (user.UpdowningTime.AddDays(1) < DateTime.Now)
                    {
                        user.UpdownTimes = 0;
                        user.Code = "1234";
                        db.Entry<User>(user).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }

                    if (user.UpdowningTime.AddMinutes(1) > DateTime.Now)
                    {
                        ModelState.AddModelError("", "操作频繁!两次上传时间请间隔1分钟.");
                        LogHelper.Log(User.Identity.Name, LogType.error.ToString(), "编辑分享失败", "添加分享失败！操作频繁，间隔为："+(user.UpdowningTime - DateTime.Now).TotalSeconds+" s", IpHelper.GetIp());
                        return View(model);
                    }

                    if (user.UpdownTimes > 10 && user.UpdowningTime.AddDays(1) < DateTime.Now)
                    {
                        LogHelper.Log(User.Identity.Name, LogType.error.ToString(), "编辑分享失败", "添加分享失败！一天添加超过10次。", IpHelper.GetIp());
                        return Content("<Script>alert('操作频繁!每天只能上传10个分享.');location.href = '/UploadShare/AddShare';</Script>");
                    }
                }

                var category = db.Categories.Where(p => p.Name == model.Category).FirstOrDefault();
                if (category == null)
                {
                    ModelState.AddModelError("", "类别不存在!请重新选择.");
                    model.Code = "";
                    LogHelper.Log(User.Identity.Name, LogType.error.ToString(), "编辑分享失败", "添加分享失败！选择的类别不存在。", IpHelper.GetIp());
                    return View(model);
                }

                var pic = Request.Files[0];
                var aCode = Request.Files[1];

                if (pic == null)
                {
                    ModelState.AddModelError("", "未选择效果图!重新提交请注意选择类别.");
                    model.Code = "";
                    LogHelper.Log(User.Identity.Name, LogType.error.ToString(), "编辑分享失败", "添加分享失败！未选择效果图。", IpHelper.GetIp());
                    return View(model);
                }

                if (pic.ContentLength > 4 * 1024 * 1024)
                {
                    ModelState.AddModelError("", "效果图超过4M!重新提交请注意选择类别.");
                    model.Code = "";
                    LogHelper.Log(User.Identity.Name, LogType.error.ToString(), "编辑分享失败", "添加分享失败！效果图大小超出限制。", IpHelper.GetIp());
                    return View(model);
                }

                var extension = Path.GetExtension(pic.FileName);
                if (extension.ToLower() != ".jpg" && extension.ToLower() != ".png")
                {
                    ModelState.AddModelError("", "效果图格式不正确!重新提交请注意选择类别.");
                    model.Code = "";
                    LogHelper.Log(User.Identity.Name, LogType.error.ToString(), "编辑分享失败", "添加分享失败！效果图格式不正确。", IpHelper.GetIp());
                    return View(model);
                }

                if (aCode == null)
                {
                    ModelState.AddModelError("", "未选择源码!重新提交请注意选择类别.");
                    model.Code = "";
                    LogHelper.Log(User.Identity.Name, LogType.error.ToString(), "编辑分享失败", "添加分享失败！未选择源码。", IpHelper.GetIp());
                    return View(model);
                }

                if (aCode.ContentLength > 8 * 1024 * 1024)
                {
                    ModelState.AddModelError("", "源码文件大于8M!重新提交请注意选择类别.");
                    model.Code = "";
                    LogHelper.Log(User.Identity.Name, LogType.error.ToString(), "编辑分享失败", "添加分享失败！源码大次奥超出限制。", IpHelper.GetIp());
                    return View(model);
                }

                var exCode = Path.GetExtension(aCode.FileName);
                if (exCode.ToLower() != ".rar" && exCode.ToLower() != ".zip")
                {
                    ModelState.AddModelError("", "源码文件格式不正确!重新提交请注意选择类别.");
                    model.Code = "";
                    LogHelper.Log(User.Identity.Name, LogType.error.ToString(), "编辑分享失败", "添加分享失败！源码格式不正确。", IpHelper.GetIp());
                    return View(model);
                }

                ShareModel entity = new ShareModel();

                entity.ResultPic = "~/UploadFile/" + DateTime.Now.ToString("yyyyMMddHHmmssfffffff") + extension;
                pic.SaveAs(HttpContext.Server.MapPath(entity.ResultPic));
                entity.CodeAddress = "~/UploadFile/" + DateTime.Now.ToString("yyyyMMddHHmmssfffffff") + exCode;
                aCode.SaveAs(HttpContext.Server.MapPath(entity.CodeAddress));

                entity.Name = model.Name;
                entity.Introduce = model.Introduce;
                entity.GitHubAddress = model.GitHubAddress;
                entity.Apply = model.Apply;
                entity.Author = model.Author ?? db.Users.Where(p => p.Email == User.Identity.Name).FirstOrDefault().NickName ?? User.Identity.Name;
                entity.Category = category;
                entity.CreateTime = DateTime.Now;
                entity.DemoAddress = model.DemoAddress;
                entity.Deploy = model.Deploy;
                entity.Download = 0;
                entity.Look = 0;
                entity.Editor = db.Users.Where(p => p.Email == User.Identity.Name).FirstOrDefault();
                entity.OldAddress = model.OldAddress;
                entity.Skill = model.Skill;
                entity.Tool = model.Tool;

                db.ShareModels.Add(entity);
                db.SaveChanges();

                user.UpdownTimes++;
                user.UpdowningTime = DateTime.Now;
                db.SaveChanges();

                LogHelper.Log(User.Identity.Name, LogType.add.ToString(), "添加分享成功", "添加分享成功！", IpHelper.GetIp());
                return Content("<Script>alert('上传成功!');location.href='/UploadShare/ShareList';</Script>");
            }

            model.Code = "";
            return View();
        }

        // GET: /UploadShare/EditShare
        // 编辑分享
        [HttpGet]
        public ActionResult EditShare(int id)
        {
            var model = db.ShareModels.Find(id);
            if (model != null)
            {
                MShare entity = new MShare();
                entity.Name = model.Name;
                entity.Introduce = model.Introduce;
                entity.GitHubAddress = model.GitHubAddress;
                entity.Apply = model.Apply;
                entity.Author = model.Author ?? db.Users.Where(p => p.Email == User.Identity.Name).FirstOrDefault().NickName ?? User.Identity.Name;
                entity.Category = model.Category.Name;
                entity.DemoAddress = model.DemoAddress;
                entity.Deploy = model.Deploy;
                entity.OldAddress = model.OldAddress;
                entity.Skill = model.Skill;
                entity.Tool = model.Tool;
                entity.Id = model.Id;

                ViewBag.Category = (from x in db.Categories orderby x.Name.Length descending select x.Name).ToList();
                return View(entity);
            }
            LogHelper.Log(User.Identity.Name, LogType.error.ToString(), "编辑分享失败", "编辑分享失败！该分享不存在。", IpHelper.GetIp());
            return Content("<Script>alert('无该分享!');location.href='/'</Script>");
        }

        // POST /UploadShare/EditShare
        // 编辑分享
        // id: 该分享Id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditShare(MShare model)
        {
            ViewBag.Category = (from x in db.Categories orderby x.Name.Length descending select x.Name).ToList();
            if (ModelState.IsValid)
            {
                if (CheckVerifyCode(model.Code))
                {
                    ModelState.AddModelError("Code", "验证码错误!重新提交请注意选择类别.");
                    model.Code = "";
                    LogHelper.Log(User.Identity.Name, LogType.error.ToString(), "编辑分享失败", "编辑分享失败！验证码错误。", IpHelper.GetIp());
                    return View(model);
                }

                var entity = db.ShareModels.Find(model.Id);
                if (entity != null)
                {
                    var category = db.Categories.Where(p => p.Name == model.Category).FirstOrDefault();
                    if (category == null)
                    {
                        ModelState.AddModelError("", "类别不存在!请重新选择.");
                        model.Code = "";
                        LogHelper.Log(User.Identity.Name, LogType.error.ToString(), "编辑分享失败", "编辑分享失败！选择的类别不存在。", IpHelper.GetIp());
                        return View(model);
                    }

                    var pic = Request.Files[0];
                    var aCode = Request.Files[1];

                    if (pic.ContentLength > 0)
                    {
                        if (pic.ContentLength > 4 * 1024 * 1024)
                        {
                            ModelState.AddModelError("", "效果图超过4M!重新提交请注意选择类别.");
                            model.Code = "";
                            LogHelper.Log(User.Identity.Name, LogType.error.ToString(), "编辑分享失败", "编辑分享失败！效果图大小超出范围。", IpHelper.GetIp());
                            return View(model);
                        }

                        var extension = Path.GetExtension(pic.FileName);
                        if (extension.ToLower() != ".jpg" && extension.ToLower() != ".png")
                        {
                            ModelState.AddModelError("", "效果图格式不正确!重新提交请注意选择类别.");
                            LogHelper.Log(User.Identity.Name, LogType.error.ToString(), "编辑分享失败", "编辑分享失败！效果图格式不正确。", IpHelper.GetIp());
                            model.Code = "";
                            return View(model);
                        }

                        System.IO.File.Delete(HttpContext.Server.MapPath(entity.ResultPic));
                        entity.ResultPic = "~/UploadFile/" + DateTime.Now.ToString("yyyyMMddHHmmssfffffff") + extension;
                        pic.SaveAs(HttpContext.Server.MapPath(entity.ResultPic));
                    }

                    if (aCode.ContentLength > 0)
                    {
                        if (aCode.ContentLength > 8 * 1024 * 1024)
                        {
                            ModelState.AddModelError("", "源码文件大于8M!重新提交请注意选择类别.");
                            model.Code = "";
                            LogHelper.Log(User.Identity.Name, LogType.error.ToString(), "编辑分享失败", "编辑分享失败！源码格式大小超出规定。", IpHelper.GetIp());
                            return View(model);
                        }

                        var exCode = Path.GetExtension(aCode.FileName);
                        if (exCode.ToLower() != ".rar" && exCode.ToLower() != ".zip")
                        {
                            ModelState.AddModelError("", "源码文件格式不正确!重新提交请注意选择类别.");
                            model.Code = "";
                            LogHelper.Log(User.Identity.Name, LogType.error.ToString(), "编辑分享失败", "编辑分享失败！源码格式不正确。", IpHelper.GetIp());
                            return View(model);
                        }

                        System.IO.File.Delete(HttpContext.Server.MapPath(entity.CodeAddress));
                        entity.CodeAddress = "~/UploadFile/" + DateTime.Now.ToString("yyyyMMddHHmmssfffffff") + exCode;
                        aCode.SaveAs(HttpContext.Server.MapPath(entity.CodeAddress));
                    }

                    entity.Name = model.Name;
                    entity.Introduce = model.Introduce;
                    entity.GitHubAddress = model.GitHubAddress;
                    entity.Apply = model.Apply;
                    entity.Author = model.Author ?? entity.Author;
                    entity.Category = category;
                    entity.DemoAddress = model.DemoAddress;
                    entity.Editor = entity.Editor;
                    entity.Deploy = model.Deploy;
                    entity.OldAddress = model.OldAddress;
                    entity.Skill = model.Skill;
                    entity.Tool = model.Tool;

                    db.Entry<ShareModel>(entity).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    LogHelper.Log(User.Identity.Name, LogType.edit.ToString(), "编辑分享成功", "编辑分享成功！", IpHelper.GetIp());
                    return Content("<Script>alert('修改成功!');location.href='/UploadShare/ShareList';</Script>");
                }
                ModelState.AddModelError("", "无该分享!");
                model.Code = "";
                LogHelper.Log(User.Identity.Name, LogType.error.ToString(), "编辑分享失败", "编辑分享失败！找不到该分享，可能存在攻击。", IpHelper.GetIp());
                return View(model);
            }
            return View();
        }

        // POST: /UploadShare/DeleteShare
        // 删除分享
        // id: 该分享Id
        [HttpPost]
        public ActionResult DeleteShare(int id)
        {
            var model = db.ShareModels.Find(id);
            if (model == null)
            {
                LogHelper.Log(User.Identity.Name, LogType.delete.ToString(), "删除分享失败", "删除分享失败！找不到该分享，可能存在攻击。", IpHelper.GetIp());
                return Json(false);
            }

            if ((!User.IsInRole("Administrator")) && model.Editor.Email!=User.Identity.Name)
            {
                LogHelper.Log(User.Identity.Name, LogType.error.ToString(), "删除分享失败", "非管理员删除非自己添加的分享，可能存在攻击！", IpHelper.GetIp());
                return Json(false);
            }

            System.IO.File.Delete(HttpContext.Server.MapPath(model.ResultPic));
            System.IO.File.Delete(HttpContext.Server.MapPath(model.CodeAddress));
            db.ShareModels.Remove(model);
            db.SaveChanges();
            LogHelper.Log(User.Identity.Name, LogType.delete.ToString(), "删除分享成功", "删除分享成功！", IpHelper.GetIp());
            return Json(true);
        }

        /// <summary>
        /// 校验分享名
        /// </summary>
        /// <param name="name">分享名</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CheckShare(string name)
        {
            if (db.ShareModels.Where(p =>  p.Name == name).Count() > 0)
            {
                return Json(false);
            }
            else
            {
                return Json(true);
            }
        }

        /// <summary>
        /// 校验类别
        /// </summary>
        /// <param name="name">类别名</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CheckCategory(string name)
        {
            if (db.Categories.Where(p => p.Name == name).Count() > 0)
            {
                return Json(false);
            }
            return Json(true);
        }

        // 析构函数
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}