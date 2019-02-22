using ShareWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace ShareWeb.Controllers
{
    public class CooperationController : BaseController
    {
        // GET: Cooperation/Contact
        // 联系站长
        [HttpGet]
        public ActionResult Contact()
        {
            var data = db.Users.Where(p => p.Email == (db.LoginUsers.Where(x => x.Role == "Administrator").FirstOrDefault().Email)).FirstOrDefault();
            ViewBag.WebMaster = data;

            return View();
        }

        // POST: /Cooperation/Contact
        // 联系站长--站内信
        // model: 信件内容
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(CooperationModel model)
        {
            if (ModelState.IsValid)
            {
                if (CheckVerifyCode(model.Code))
                {
                    ModelState.AddModelError("Code", "验证码不正确!");

                    model.Code = "";
                    return View(model);
                }

                Letter entity = new Letter();

                entity.Sub = model.Sub;
                entity.Content = model.Content;
                entity.CreateTime = DateTime.Now;
                entity.Type = LetterType.contact.ToString();
                entity.IsRead = false;
                entity.IsInportant = false;
                entity.From = db.Users.Where(p => p.Email == User.Identity.Name).FirstOrDefault();
                entity.To = db.LoginUsers.Where(x => x.Role == "Administrator").FirstOrDefault().Email;

                db.Letters.Add(entity);
                db.SaveChanges();

                return Content("<Script>alert('留言成功!'); location.href='/Cooperation/Contact'</Script>");
            }

            model.Code = "";
            return View(model);
        }

        // GET: /Cooperation/FeeBack
        // 问题反馈
        [HttpGet]
        public ActionResult FeedBack()
        {
            return View();
        }

        // POST: /Cooperation/FeeBack
        // 问题反馈--站内信
        // model:信件内容
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FeedBack(CooperationModel model)
        {
            if (ModelState.IsValid)
            {
                if (CheckVerifyCode(model.Code))
                {
                    ModelState.AddModelError("Code", "验证码不正确!");

                    model.Code = "";
                    return View(model);
                }

                Letter entity = new Letter();

                entity.Sub = model.Sub;
                entity.Content = model.Content;
                entity.CreateTime = DateTime.Now;
                entity.Type = LetterType.feedBack.ToString();
                entity.IsRead = false;
                entity.IsInportant = false;
                entity.To = db.LoginUsers.Where(x => x.Role == "Administrator").FirstOrDefault().Email;
                entity.From = db.Users.Where(p => p.Email == User.Identity.Name).FirstOrDefault();

                db.Letters.Add(entity);
                db.SaveChanges();

                return Content("<Script>alert('反馈已收到!谢谢.'); location.href='/Cooperation/FeedBack'</Script>");
            }
            model.Code = "";
            return View(model);
        }

        // GET: /Cooperation/SystemNews
        // 系统消息
        [Authorize]
        public ActionResult SystemNews()
        {
            return View(db.Letters.Where(p => p.To == User.Identity.Name).ToList());
        }

        // POST: /Cooperation/TabIsLook
        // 标记已读
        // id: 消息Id
        [HttpPost]
        [Authorize]
        public ActionResult TabIsLook(int id)
        {
            var model = db.Letters.Find(id);
            if (model != null)
            {
                model.IsRead = true;
                model.From = model.From;
                db.Entry<Letter>(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return Json(true);
            }
            return Json(false);
        }

        // POST: /Cooperation/TabIsInportant
        // 标记为重要消息
        // id: 信息Id
        [HttpPost]
        [Authorize]
        public ActionResult TabIsInportant(int id)
        {
            var model = db.Letters.Find(id);
            if (model != null)
            {
                model.IsInportant = true;
                model.IsRead = true;
                model.From = model.From;
                db.Entry<Letter>(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return Json(true);
            }
            return Json(false);
        }

        // POST: /Cooperation/TabNotInportant
        //移出重要消息
        // id: 信息Id
        [HttpPost]
        [Authorize]
        public ActionResult TabNotInportant(int id)
        {
            var model = db.Letters.Find(id);
            if (model != null)
            {
                model.IsInportant = false;
                model.From = model.From;
                db.Entry<Letter>(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return Json(true);
            }
            return Json(false);
        }

        // POST: /Cooperation/DeleteNews
        // 删除消息
        // id: 消息Id
        [HttpPost]
        [Authorize]
        public ActionResult DeleteNews(int id)
        {
            var model = db.Letters.Find(id);
            if (model != null)
            {
                model.From = model.From;
                db.Letters.Remove(model);
                db.SaveChanges();

                return Json(true);
            }
            return Json(false);
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