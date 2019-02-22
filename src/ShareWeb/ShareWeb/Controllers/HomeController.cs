using ShareWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShareWeb.Controllers
{
    public class HomeController : BaseController
    {
        // GET: /Home/Index
        // 网站前台首页
        public ActionResult Index()
        {
            var model = db.WebConfigs.FirstOrDefault();
            if (model == null)
            {
                return DefaultSystem();
            }
            var data = (from x in db.Users
                        where x.Email == (from p in db.LoginUsers where p.Role == "Administrator" select p.Email).FirstOrDefault()
                        select new { x.NickName, x.Introduce }).ToList();
            ViewBag.NickName = data.FirstOrDefault().NickName;
            ViewBag.Introduce = data.FirstOrDefault().Introduce;
            ViewBag.PublishNews = db.Letters.Where(p => p.Type == LetterType.system.ToString() && p.From.Email == p.To && p.To == db.LoginUsers.Where(x => x.Role == "Administrator").FirstOrDefault().Email).OrderByDescending(p => p.Id).ToList().OrderByDescending(p => p.Id);
            return View(model);
        }

        // GET: /Home/DefaultSystem
        // 初始化网站
        public ActionResult DefaultSystem()
        {
            // 初始化用户
            if (db.Users.Count() == 0)
            {
                User user = new Models.User();
                LoginUser entity = new LoginUser();

                entity.Email = user.Email = "2512303162@qq.com";
                user.SNo = "00000000";
                user.UpdowningTime = DateTime.Now;
                user.UpdownTimes = 0;
                user.Code = "1233";

                entity.Password = Md5.GetMD5("Dudeping.2016sicau", entity.Email);
                entity.Role = "Administrator";
                entity.LockingTime = DateTime.Parse("1990-01-01");
                entity.LockTimes = 0;

                db.Users.Add(user);
                db.LoginUsers.Add(entity);

                db.SaveChanges();
            }

            // 初始化网站配置
            var model = db.WebConfigs.FirstOrDefault();
            if (model == null)
            {
                model = new WebConfig();
                model.Introduce = "此处是初始化的网站信息数据,请速到后台修改!";
                model.Use = "此处是初始化的网站信息数据,请速到后台修改!";
                model.Method = "此处是初始化的网站信息数据,请速到后台修改!";
                model.Point = "此处是初始化的网站信息数据,请速到后台修改!";
                model.History = "(测试版本#####此处是初始化的网站信息数据,请速到后台修改!#####" + DateTime.Now.AddDays(2).ToShortDateString() + ")|(未完成版本#####此处是初始化的网站信息数据,请速到后台修改!#####" + DateTime.Now.ToShortDateString() + ")";
                model.Name = "杜德平";
                model.OldAddress = "四川泸州";
                model.NowAddress = "四川雅安";
                model.Job = "学生";
                model.LikeBook = "《山居笔记》,《文化苦旅》";
                model.LikeMusic = "《小幸运》";
                db.WebConfigs.Add(model);
                db.SaveChanges();
            }

            // 初始化类别
            var category = db.Categories.Where(p => p.Name == "其他").FirstOrDefault();
            if (category == null)
            {
                category = new Category();
                category.Name = "其他";
                category.Author = "2512303162@qq.com";
                category.Code = "1343";
                db.Categories.Add(category);
                db.SaveChanges();
            }

            return Content("<Script>alert('初始化系统成功!');location.href='/';</Script>");
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