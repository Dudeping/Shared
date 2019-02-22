using ShareDemo.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

/// <summary>
/// 第一个用XML搭载网站的程序
/// 记得修改配置文件的xml地址
/// </summary>
namespace ShareDemo.Controllers
{
    public class HomeController : Controller
    {
        private string myCon = ConfigurationManager.ConnectionStrings["ShareDemoContext"].ConnectionString;
        // GET: Home
        public ActionResult Index(int id = 1)
        {
            //读取信息
            XDocument xdoc = XDocument.Load(myCon);
            XElement root = xdoc.Element("Root");
            var exList = root.Elements("Item");

            var exItem = from x in exList
                         where x.Element("Id").Value.ToString() == id.ToString()
                         select x;
            if(exItem.Count() == 0)
            {
                //一般用户胡乱修改URL就会直接报错或者找不到
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            //正常情况下就只有一个
            else if(exItem.Count() == 1)
            {
                //取出第一个元素
                XElement xe = exItem.FirstOrDefault();
                //定义一个返回的对象
                ShareModel model = new ShareModel();
                model.Name = xe.Element("Name").Value;
                model.Decription = xe.Element("Decription").Value;

                //链接
                var linkList = xe.Elements("Link");
                //实例化链接对象
                model.Link = new List<LinkModel>();
                foreach (var item in linkList)
                {
                    model.Link.Add(new LinkModel { LinkText = item.Element("LinkText").Value, LinkSrc = item.Element("LinkSrc").Value });
                }
                return View(model);
            }
            else
            {
                //多个的时候返回服务器错误
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }
    }
}