using ShareWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShareWeb.Controllers
{
    #if DEBUG
        //[RequireHttps]
    #else
        [RequireHttps]
    #endif
    public class BaseController : Controller
    {
        protected ShareWebContext db = new ShareWebContext();

        /// <summary>
        /// 校验验证码
        /// </summary>
        /// <param name="code">验证码</param>
        /// <returns></returns>
        public bool CheckVerifyCode(string code)
        {
            if (Session["share_session_verifycode"] == null || Session["share_session_verifycode"].ToString() != Md5.GetMD5(code.ToLower(), "verifycode"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}