using PagedList;
using ShareWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShareWeb.Controllers
{
    public class ShowShareController : BaseController
    {
        // GET: /ShowShare/ShareList
        // 分享列表
        // id:分享类别名
        //[OutputCache(CacheProfile = "SqlDependencyCacheShareList")]
        public ActionResult ShareList(string id, int page = 1)
        {
            ViewBag.Title = id;
            if (db.Categories.Where(p => p.Name == id).Count() <= 0)
            {
                return Content("<Script>alert('无该类别!');location.href='/'</Script>");
            }
            return View(db.ShareModels.Where(p => p.Category.Name == id).OrderByDescending(p => p.Id).ToPagedList(page, 6));
        }

        // GET: /ShowShare/SearchList
        // 搜索结果列表
        // searchStr: 搜索关键词
        [HttpGet]
        public ActionResult SearchList(string searchStr, int page = 1)
        {
            if (searchStr == null)
            {
                return View("Error");
            }

            ViewBag.SearchStr = searchStr;
            return View(db.ShareModels.Where(p => p.Name.Contains(searchStr)).OrderByDescending(p => p.Look).ToPagedList(page, 6));
        }

        // POST: /ShowShare/SearchList
        // 搜索结果列表
        // searchStr: 搜索关键词
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchList(string searchStr)
        {
            if (searchStr == null)
            {
                return View("Error");
            }

            ViewBag.SearchStr = searchStr;
            return View(db.ShareModels.Where(p => p.Name.Contains(searchStr)).OrderByDescending(p => p.Look).ToPagedList(1, 6));
        }

        // GET: /ShowShare/ShareDatail
        // 分享详情
        // id:分享名
        public ActionResult ShareDetail(int id)
        {
            var model = db.ShareModels.Find(id);
            if (model == null)
            {
                LogHelper.Log(User.Identity.Name ==""? "游客": User.Identity.Name, LogType.error.ToString(), "分享不存在", "分享不存在，可能已删除，id：" + id.ToString(), IpHelper.GetIp());
                return Content("<Script>alert('无该分享!');location.href='/'</Script>");
            }
            model.Look++;
            model.Category = model.Category;
            model.Editor = model.Editor;
            db.Entry<ShareModel>(model).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            LogHelper.Log(User.Identity.Name == "" ? "游客" : User.Identity.Name, LogType.look.ToString(), "浏览", "浏览：" + model.Name, IpHelper.GetIp());
            return View(model);
        }

        /// <summary>
        /// 添加下载量
        /// </summary>
        /// <param name="id">分享id</param>
        public void AddDownload(int id)
        {
            var model = db.ShareModels.Find(id);
            if (model != null)
            {
                model.Download++;
                model.Category = model.Category;
                model.Editor = model.Editor;
                db.Entry<ShareModel>(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                LogHelper.Log(User.Identity.Name == "" ? "游客" : User.Identity.Name, LogType.download.ToString(), "下载", "下载：" + model.Name, IpHelper.GetIp());
            }
        }

        //析构函数
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