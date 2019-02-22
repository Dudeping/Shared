using PagedList;
using ShareWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShareWeb.Controllers
{
    [Authorize(Roles ="Administrator")]
    public class AdminController : BaseController
    {
        // GET: /Admin/Index
        // 后台首页
        public ActionResult Index(int date=10)
        {
            // 未读记录
            var model = db.Logs.Where(p => p.IsRead == false);
            // 新增分享
            ViewBag.NewAdd = model.Count(p => p.IsRead == false && p.Type == "add");
            // 新增下载
            ViewBag.NewDownLoad = model.Count(p => p.Type == LogType.download.ToString());
            // 未读反馈
            ViewBag.NewContact = db.Letters.Count(p => p.To == User.Identity.Name && p.Type != LetterType.system.ToString());
            // 系统出错
            ViewBag.NewError = model.Count(p => p.Type == "error");
            // 浏览下载日期范围
            string[] _dataTime = new string[date];
            // 日期范围
            string vDataTime = "";
            // 浏览量
            string vTotalLook = "";
            // 下载量
            string vTotalDownLoad = "";
            // date天前
            var oldDay = DateTime.Now.AddDays(-(date-1));
            // date天来浏览量统计
            var totalLook = db.Logs.Where(p => p.Type == LogType.look.ToString() && (p.CreateTime <= DateTime.Now && p.CreateTime >= oldDay)).ToList().GroupBy(p => p.CreateTime.ToShortDateString()).Select(p => new { CreateTime = p.Key, Count = p.Count() });
            // date天来下载量统计
            var totalDownLoad = db.Logs.Where(p => p.Type == LogType.download.ToString() && (p.CreateTime <= DateTime.Now && p.CreateTime >= oldDay)).ToList().GroupBy(p => p.CreateTime.ToShortDateString()).Select(p => new { CreateTime = p.Key, Count = p.Count() });
            
            for (int i = 0; i < date; i++)
            {
                // 时间
                _dataTime[i] = DateTime.Now.AddDays(i - (date-1)).ToShortDateString();
                vDataTime += "\"" + _dataTime[i].Substring(_dataTime[i].IndexOf('/') + 1) + "\",";
                // 浏览量
                var __totalLook = totalLook.FirstOrDefault(p => p.CreateTime == _dataTime[i]);
                vTotalLook += (__totalLook == null ? "0" : __totalLook.Count.ToString()) + ",";
                // 下载量
                var __totalDownLoad = totalDownLoad.FirstOrDefault(p => p.CreateTime == _dataTime[i]);
                vTotalDownLoad += (__totalDownLoad == null ? "0" : __totalDownLoad.Count.ToString()) +",";
            }

            ViewBag.xDataTime = vDataTime.TrimEnd(',');
            ViewBag.TotalLook = vTotalLook.TrimEnd(',');
            ViewBag.TotalDownLoad = vTotalDownLoad.TrimEnd(',');

            return View();
        }

        // GET: /Admin/WebMasterNews
        // 站长信息
        [HttpGet]
        public ActionResult WebMasterNews()
        {
            var model = db.WebConfigs.FirstOrDefault();
            if (model != null)
            {
                var entity = new WebMaster();
                entity.Name = model.Name;
                entity.NowAddress = model.NowAddress;
                entity.OldAddress = model.OldAddress;
                entity.LikeMusic = model.LikeMusic;
                entity.LikeBook = model.LikeBook;
                entity.Job = model.Job;

                return View(entity);
            }

            return Content("<Script>alert('操作错误!');location.href='/Admin';</Script>");
        }

        // POST: /Admin/WebMasterNews
        // 处理站长信息
        // model:处理数据
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult WebMasterNews(WebMaster model)
        {
            if (ModelState.IsValid)
            {
                if (CheckVerifyCode(model.Code))
                {
                    ModelState.AddModelError("Code", "验证码不正确!");
                    model.Code = "";
                    return View(model);
                }

                var entity = db.WebConfigs.FirstOrDefault();

                if (entity != null)
                {
                    entity.Name = model.Name;
                    entity.NowAddress = model.NowAddress;
                    entity.OldAddress = model.OldAddress;
                    entity.LikeMusic = model.LikeMusic;
                    entity.LikeBook = model.LikeBook;
                    entity.Job = model.Job;

                    db.Entry<WebConfig>(entity).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    return Content("<Script>alert('编辑成功!'); location.href='/Admin/WebMasterNews';</Script>");
                }
            }
            model.Code = "";
            return View(model);
        }

        // GET: /Admin/ShareWebNews
        // 网站信息
        [HttpGet]
        public ActionResult ShareWebNews()
        {
            var model = db.WebConfigs.FirstOrDefault();
            if (model != null)
            {
                var entity = new WebNews();
                entity.Introduce = model.Introduce;
                entity.Method = model.Method;
                entity.Point = model.Point;
                entity.Use = model.Use;

                return View(entity);
            }

            return Content("<Script>alert('操作错误!');location.href='/Admin';</Script>");
        }

        // POST: /Admin/ShareWebNews
        // 处理网站信息
        // model:网站信息数据
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ShareWebNews(WebNews model)
        {
            if (ModelState.IsValid)
            {
                if (CheckVerifyCode(model.Code))
                {
                    ModelState.AddModelError("Code", "验证码不正确!");
                    model.Code = "";
                    return View(model);
                }

                var entity = db.WebConfigs.FirstOrDefault();
                if (entity != null)
                {
                    entity.Introduce = model.Introduce;
                    entity.Method = model.Method;
                    entity.Point = model.Point;
                    entity.Use = model.Use;

                    db.Entry<WebConfig>(entity).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    return Content("<Script>alert('编辑成功!'); location.href='/Admin/ShareWebNews';</Script>");
                }
            }
            model.Code = "";
            return View(model);
        }

        // GET: /Admin/ShareWebHistory
        // 网站历史
        [HttpGet]
        public ActionResult ShareWebHistory()
        {
            ViewBag.HistoryList = GetHistory();
            return View();
        }

        // POST: /Admin/AddHistory
        // 添加网站历史
        // model: 历史信息
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddHistory(WebHistory model)
        {
            ViewBag.HistoryList = GetHistory();
            if (ModelState.IsValid)
            {
                if (CheckVerifyCode(model.Code))
                {
                    return Content("<Script>alert('验证码错误!');location.href='/Admin/ShareWebHistory'</Script>");
                }

                if (CheckVersion(model))
                {
                    return Content("<Script>alert('已有该版本!请勿重复添加.');location.href='/Admin/ShareWebHistory'</Script>");
                }

                var entity = GetHistory();
                model.CreateTime = DateTime.Now;
                entity.Add(model);
                if (SaveHistory(entity))
                {
                    return Content("<Script>alert('添加成功!');location.href='/Admin/ShareWebHistory';</Script>");
                }
                else
                {
                    ViewBag.JS = "添加失败! 请重试.";
                    ModelState.AddModelError("", "添加失败! 请重试.");
                    model.Code = "";
                    return View("ShareWebHistory", model);
                }
            }

            return Content("<Script>alert('操作失败!请重试.');location.href='/Admin/ShareWebHistory'</Script>");
        }

        // GET: /Admin/EditHistory
        // 编辑网站版本信息
        // version:该条历史版本
        [HttpGet]
        public ActionResult EditHistory(string version)
        {
            var model = (from x in GetHistory() where x.Version == version select x).FirstOrDefault();
            if (model != null)
            {
                return View(model);
            }
            return Content("<Script>alert('操作失败!');location.href='/Admin/ShareWebHistory';</Script>");
        }

        // POST: /Admin/EditHistory
        // 编辑网站版本信息
        // model:信息
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditHistory(WebHistory model)
        {
            ViewBag.HistoryList = GetHistory();
            if (ModelState.IsValid)
            {
                if (CheckVerifyCode(model.Code))
                {
                    model.Code = "";
                    ModelState.AddModelError("Code", "验证码错误!");
                    return View(model);
                }

                if (CheckVersion(model))
                {
                    var entity = GetHistory();
                    entity[entity.FindIndex(p => p.Version == model.Version)].Content = model.Content;
                    if (SaveHistory(entity))
                    {
                        return Content("<Script>alert('编辑成功!');location.href='/Admin/ShareWebHistory';</Script>");
                    }
                    else
                    {
                        model.Code = "";
                        ModelState.AddModelError("", "编辑失败! 请重试.");
                        return View(model);
                    }
                }
            }

            model.Code = "";
            return View(model);
        }

        // POST: /Admin/DeleteHistory
        // 删除历史信息
        // version: 该条信息版本
        [HttpPost]
        public ActionResult DeleteHistory(string version)
        {
            var entity = GetHistory();
            if (entity.Where(p => p.Version == version).Count() > 0)
            {
                entity.RemoveAll(p => p.Version == version);
                if (SaveHistory(entity))
                {
                    return Json(true);
                }
            }
            return Json(false);
        }

        // GET: /Admin/AuthorList
        // 管理作者
        public ActionResult AuthorList()
        {
            return View(db.Users.Where(p => p.Email != User.Identity.Name).OrderByDescending(p => p.Id).ToList());
        }

        // POST: /Admin/DeleteAuthor
        // 删除作者
        // id: 该作者Id
        [HttpPost]
        public ActionResult DeleteAuthor(int id)
        {
            var model = db.Users.Find(id);

            if (model != null)
            {
                var entity = db.LoginUsers.Where(p => p.Email == model.Email).FirstOrDefault();
                if (entity != null)
                {
                    entity.IsDelete = true;
                    model.Introduce = "该用户因为行为恶劣违反本站相关约定已被删除!";
                    model.Code = "1234";
                    db.SaveChanges();

                    return Json(true);
                }
            }
            return Json(false);
        }

        // GET: /Admin/ContactList
        // 联系反馈
        [HttpGet]
        public ActionResult ContactList()
        {
            return View(db.Letters.Where(p => p.To == User.Identity.Name && p.Type != LetterType.system.ToString()).OrderByDescending(p => p.Id));
        }

        // GET: /Admin/PublishNews
        // 发布公告
        [HttpGet]
        public ActionResult PublishNews()
        {
            ViewBag.LetterData = db.Letters.Where(p => p.Type == LetterType.system.ToString() && p.From.Email == p.To && p.To == User.Identity.Name).OrderByDescending(p => p.Id).ToList();
            return View();
        }

        // POST: /Admin/PublishNews
        // 发布公告
        // model: 公告信息
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PublishNews(PublishNews model)
        {
            if (ModelState.IsValid)
            {
                if (CheckVerifyCode(model.Code))
                {
                    ModelState.AddModelError("Code", "验证码错误!");
                    model.Code = "";
                    return View(model);
                }

                List<Letter> letterList = new List<Letter>();
                foreach (var item in db.LoginUsers.ToList())
                {
                    Letter entity = new Letter();
                    entity.Sub = "系统公告";
                    entity.Content = model.Content;
                    entity.From = db.Users.Where(p => p.Email == User.Identity.Name).FirstOrDefault();
                    entity.To = item.Email;
                    entity.Type = LetterType.system.ToString();
                    entity.CreateTime = DateTime.Now;
                    letterList.Add(entity);
                }
                db.Letters.AddRange(letterList);
                db.SaveChanges();

                return Content("<Script>alert('发布消息成功!');location.href='/Admin/PublishNews'</Script>");
            }

            model.Code = "";
            return View(model);
        }

        // GET: /Admin/PublishNews
        // 发布私信
        [HttpGet]
        public ActionResult ReleaseLetter()
        {
            var data = db.Letters.Where(p => p.Type == LetterType.system.ToString() && p.From.Email == p.To && p.To == User.Identity.Name).ToList();
            var model = db.Users.Where(p => p.Email == User.Identity.Name).FirstOrDefault().Letters.ToList();
            foreach (var item in data)
            {
                model.RemoveAll(p => p.Sub == item.Sub && p.Content == item.Content);
            }
            ViewBag.LetterData = model.OrderByDescending(p => p.Id);
            return View();
        }

        // POST: /Admin/PublishNews
        // 发布私信
        // model: 私信信息
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReleaseLetter(ReleaseLetter model)
        {
            if (ModelState.IsValid)
            {
                if (CheckVerifyCode(model.Code))
                {
                    ModelState.AddModelError("Code", "验证码错误!");
                    model.Code = "";
                    return View(model);
                }

                if (db.Users.Where(p => p.Email == model.To).Count() > 0 && model.To != User.Identity.Name)
                {
                    Letter entity = new Letter();
                    entity.To = model.To;
                    entity.From = db.Users.Where(p => p.Email == User.Identity.Name).FirstOrDefault();
                    entity.Sub = model.Sub;
                    entity.Content = model.Content;
                    entity.CreateTime = DateTime.Now;
                    entity.Type = LetterType.system.ToString();

                    db.Letters.Add(entity);
                    db.SaveChanges();

                    return Content("<Script>alert('发送成功!');location.href='/Admin/ReleaseLetter';</Script>");
                }
                ModelState.AddModelError("To", "找不到该用户!");
                model.Code = "";
                return View(model);
            }
            model.Code = "";
            return View(model);
        }

        // POST: /Admin/Reply
        // 回复
        [HttpGet]
        public ActionResult Reply(int id)
        {
            if (db.Letters.Find(id) != null) 
            {
                ViewBag.LetterId = id;
                return View();
            }
            return Content("<Script>alert('操作失败!请重试.'); location.href='/Admin/ContactList'</Script>");
        }

        // POST: /Admin/Reply
        // 回复
        // model: 数据
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reply(Reply model)
        {
            if (ModelState.IsValid)
            {
                if (CheckVerifyCode(model.Code))
                {
                    ModelState.AddModelError("Code", "验证码错误!");
                    model.Code = "";
                    return View(model);
                }

                var letter = db.Letters.Find(model.LetterId);

                if (letter == null)
                {
                    ModelState.AddModelError("", "操作失败!请重试.");
                    model.Code = "";
                    return View(model);
                }

                Letter entity = new Letter();
                entity.Sub = model.Sub;
                entity.Content = model.Content;
                entity.CreateTime = DateTime.Now;
                entity.From = db.Users.Where(p => p.Email == User.Identity.Name).FirstOrDefault();
                entity.To = letter.From.Email;
                entity.Type = LetterType.system.ToString();

                db.Letters.Add(entity);
                db.SaveChanges();

                letter.Reply = model.Sub + "#####" + model.Content;
                letter.IsRead = true;
                db.Entry<Letter>(letter).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return Content("<Script>alert('回复成功!'); location.href='/Admin/ContactList'</Script>");
            }
            model.Code = "";
            return View(model);
        }

        // POST: /Admin/DeleteContact
        // 删除联系反馈
        // id: 该信息Id
        [HttpPost]
        public ActionResult DeleteContact(int id)
        {
            var model = db.Letters.Find(id);
            if (model != null)
            {
                db.Letters.Remove(model);
                db.SaveChanges();
                return Json(true);
            }
            return Json(false);
        }

        // GET: /Admin/WebLog
        // 网站日志
        public ActionResult WebLog(int page = 1, string type = "all")
        {
            var entity = from x in db.Logs
                         select x;
            if (!string.IsNullOrWhiteSpace(type) && type != "all")
            {
                entity = entity.Where(p => p.Type == type);
            }
            var model = entity.OrderByDescending(p => p.Id).ToPagedList(page, 60);
            foreach (var item in model)
            {
                item.IsRead = true;
            }
            db.SaveChanges();
            return View(model);
        }

        /// <summary>
        /// 获得网站版本信息
        /// </summary>
        /// <returns>网站列表</returns>
        private List<WebHistory> GetHistory()
        {
            List<WebHistory> data = new List<WebHistory>();
            string history = db.WebConfigs.FirstOrDefault().History;
            foreach (var item in history.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var historyList = new WebHistory();
                var historyArray = item.Trim(new char[] { '(', ')' }).Split(new string[] { "#####" }, StringSplitOptions.RemoveEmptyEntries);

                historyList.Version = historyArray[0];
                historyList.Content = historyArray[1];
                historyList.CreateTime = DateTime.Parse(historyArray[2]);
                data.Add(historyList);
            }

            return data;
        }

        /// <summary>
        /// 保存网站历史
        /// </summary>
        /// <param name="model">网站历史数据</param>
        /// <returns></returns>
        public bool SaveHistory(List<WebHistory> model)
        {
            List<string> historyList = new List<string>();
            foreach (var item in model)
            {
                historyList.Add(String.Format("({0}#####{1}#####{2})", item.Version, item.Content, item.CreateTime.ToString()));
            }
            var entity = db.WebConfigs.FirstOrDefault();
            entity.History = String.Join("|", historyList);

            try
            {
                db.Entry<WebConfig>(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 校验邮箱
        /// </summary>
        /// <param name="to">邮箱</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CheckEmail(string to)
        {
            if (db.LoginUsers.Where(p => p.Email == to && p.Role == "Administrator").Count() > 0)
            {
                return Json(false);
            }

            if (db.Users.Where(p => p.Email == to).Count() > 0)
            {
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }

        /// <summary>
        /// 处理错误信息
        /// </summary>
        /// <param name="model">校验模型信息</param>
        /// <returns></returns>
        public string GetErrorMessage(ModelStateDictionary model)
        {
            string rel = "";
            foreach (var key in model.Keys)
            {
                foreach (var error in model[key].Errors)
                {
                    rel += error.ErrorMessage + ", ";
                }
            }
            return rel.TrimEnd(new char[] { ',', ' ' });
        }

        /// <summary>
        /// 校验版本
        /// </summary>
        /// <param name="version">版本</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CheckVersion(string version)
        {
            if ((from x in GetHistory() where x.Version == version select x).Count() > 0)
            {
                return Json(false);
            }
            else
            {
                return Json(true);
            }
        }

        /// <summary>
        /// 校验版本
        /// </summary>
        /// <param name="model">版本信息</param>
        /// <returns></returns>
        public bool CheckVersion(WebHistory model)
        {
            if ((from x in GetHistory() where x.Version == model.Version select x).Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
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