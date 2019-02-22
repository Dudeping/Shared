using ShareWeb.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ShareWeb.Controllers
{
    public class AccountController : BaseController
    {
        // GET: /Account/Login
        // 登录
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        // 登录处理
        // model:登录数据
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Password = "";
                model.Code = "";
                return View(model);
            }

            if (CheckVerifyCode(model.Code))
            {
                ModelState.AddModelError("Code", "验证码不正确!");

                LogHelper.Log(model.Email, LogType.error.ToString(), "登录失败", "验证码错误,客户端可能禁用了JavaScript。", IpHelper.GetIp());
                model.Password = model.Code = "";
                return View(model);
            }

            var password = Md5.GetMD5(model.Password, model.Email);
            var entity = db.LoginUsers.Where(p => p.Email == model.Email && p.Password == password).FirstOrDefault();

            if (entity != null)
            {
                if (entity.IsDelete == true)
                {
                    LogHelper.Log(model.Email, LogType.login.ToString(), "登录失败", "该用户已被删除，但是仍在登录！", IpHelper.GetIp());
                    ModelState.AddModelError("Email", "该用户已被删除!");
                    model.Code = "";
                    return View(model);
                }

                string role = db.LoginUsers.Where(p => p.Email == model.Email).FirstOrDefault().Role;
                FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                    1,
                    model.Email,
                    DateTime.Now,
                    DateTime.Now.AddYears(1),
                    true,
                    role
                   );
                string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                System.Web.HttpCookie authCookie = new System.Web.HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                System.Web.HttpContext.Current.Response.Cookies.Add(authCookie);

                LogHelper.Log(model.Email, LogType.login.ToString(), "登录成功", "登陆成功！", IpHelper.GetIp());
                return Redirect("/");
            }
            else
            {
                if (db.TempRegisters.Where(p => p.Email == model.Email && p.Password == password).Count() == 1)
                {
                    ModelState.AddModelError("", "账户尚未激活, 请前往注册所用的邮箱激活账户!");
                    model.Code = "";
                    LogHelper.Log(model.Email, LogType.login.ToString(), "登录失败", "账户尚未激活！", IpHelper.GetIp());
                    return View();
                }

                ModelState.AddModelError("", "用户名或密码错误!");
                model.Code = "";
                LogHelper.Log(model.Email, LogType.login.ToString(), "登陆失败", "用户名或密码错误！", IpHelper.GetIp());
                return View(model);
            }
        }

        // GET: /Account/Register
        // 申请上传
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        // 认证
        // model: 认证材料
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                if (CheckVerifyCode(model.Code))
                {
                    ModelState.AddModelError("Code", "验证码不正确!");

                    LogHelper.Log(model.Email, LogType.error.ToString(), "注册失败", "验证码错误！客户端可能禁用了JavaScript。", IpHelper.GetIp());
                    model.Code = "";
                    return View(model);
                }

                if (!LogingHelper.Login(model.SNo, model.EduPassword).Contains(model.SNo))
                {
                    ModelState.AddModelError("", "学号或课程平台密码错误!");
                    model.Code = "";
                    LogHelper.Log(model.Email, LogType.login.ToString(), "注册失败", "验证身份失败！学号或课程平台密码错误。", IpHelper.GetIp());
                    return View(model);
                }

                // 删除已存在的该学号信息
                var entity = db.TempRegisters.Where(p => p.SNo == model.SNo);
                if ( entity.Count() > 0)
                {
                    db.TempRegisters.RemoveRange(entity);
                    db.SaveChanges();
                }

                TempRegister register = new TempRegister();
                register.SNo = model.SNo;
                register.Email = model.Email;
                register.Password = Md5.GetMD5(model.Password, model.Email);
                register.Code = Guid.NewGuid().ToString();
                register.SendTime = DateTime.Now;

                //发送激活邮件
                var url = new UriBuilder(Request.Url)
                {
                    Path = Url.Action("Activation", "Account", new { id = register.Code }),
                    Query = ""
                };
                string emailBody = "点击 <a href='" + url + "'> 此处 </a> 激活账户, 若不能跳转，请将以下链接复制到浏览器地址栏进行跳转!<br/><br/>" + url;
                if (!SeedEmail(model.Email, "激活邮件", emailBody))
                {
                    model.Code = "";
                    ModelState.AddModelError("", "发送激活邮件失败,请重新填写!");
                    LogHelper.Log(model.Email, LogType.login.ToString(), "注册失败", "发送激活邮件失败！", IpHelper.GetIp());
                    return View(model);
                }

                db.TempRegisters.Add(register);
                db.SaveChanges();

                LogHelper.Log(model.Email, LogType.login.ToString(), "注册成功", "注册成功！已发送激活邮件，Code："+register.Code, IpHelper.GetIp());
                return Content("<Script>alert('注册成功!请按右边温馨提示内容到注册邮箱查看激活邮件并按照指定步骤激活,激活链接24小时内有效!'); location.href='/'</Script>");
            }
            model.Code = "";
            return View(model);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="email">接收邮箱</param>
        /// <param name="sub">邮件主题</param>
        /// <param name="Content">邮件内容</param>
        /// <returns></returns>
        public bool SeedEmail(string email, string sub, string Content)
        {
            var mailconfig = (EmailConfigurationProvider)ConfigurationManager.GetSection("EmailConfigurationProvider");

            MailMessage msg = new MailMessage();
            msg.To.Add(email);
            msg.Subject = sub;
            msg.From = new MailAddress(mailconfig.Account, mailconfig.Name);
            msg.SubjectEncoding = System.Text.Encoding.UTF8;//邮件标题编码
            msg.Body = Content;
            msg.BodyEncoding = System.Text.Encoding.UTF8;//邮件内容编码
            msg.IsBodyHtml = true;
            msg.Priority = MailPriority.High;
            SmtpClient client = new SmtpClient();
            client.Credentials = new NetworkCredential(mailconfig.Account, mailconfig.Password);
            client.Host = mailconfig.Server;
            client.EnableSsl = mailconfig.IsSSL;
            object userState = msg;

            try
            {
                client.Send(msg);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 激活账号
        /// </summary>
        /// <param name="id">激活码</param>
        /// <returns></returns>
        public ActionResult Activation(string id)
        {
            if (id == null)
            {
                LogHelper.Log("游客", LogType.error.ToString(), "激活失败", "激活失败！Code为空，可能存在攻击。", IpHelper.GetIp());
                return View("Error");
            }
            var model = db.TempRegisters.Where(p => p.Code == id).FirstOrDefault();

            if (model == null)
            {
                LogHelper.Log("游客", LogType.login.ToString(), "激活失败", "激活码不存在，存在重复点击激活连接，Code："+id, IpHelper.GetIp());
                return Content("<Script>alert('激活码不存在!请重新注册.'); location.href='/Account/Register';</Script>");
            }

            //存在, 并没有过期
            if (model!=null && model.SendTime.AddDays(1) > DateTime.Now)
            {
                User user = new Models.User();
                LoginUser entity = new LoginUser();

                user.Email = entity.Email = model.Email;
                user.SNo = model.SNo;
                user.Code = "1234";
                entity.Role = "User";
                entity.IsDelete = false;
                entity.Password = model.Password;
                entity.LockingTime = user.UpdowningTime = DateTime.Parse("1990-01-01");
                entity.LockTimes = user.UpdownTimes = 0;

                Letter letter = new Letter();
                letter.To = model.Email;
                letter.Type = LetterType.system.ToString();
                letter.From = db.Users.Where(x => x.Email == (db.LoginUsers.Where(p => p.Role == "Administrator").FirstOrDefault().Email)).FirstOrDefault();
                letter.Sub = "欢迎注册本网站";
                letter.Content = "尊敬的" + model.Email + "(" + model.SNo + "),你好！欢迎注册本网站, 现在你可以进行在本站上传分享了!";
                letter.CreateTime = DateTime.Now;

                db.Users.Add(user);
                db.Letters.Add(letter);
                db.LoginUsers.Add(entity);
                db.TempRegisters.Remove(model);

                db.SaveChanges();

                LogHelper.Log(model.Email, LogType.login.ToString(), "激活成功", "激活账号成功！Code："+id, IpHelper.GetIp());
                return Content("<Script>alert('账户激活成功!请登录.');location.href='/Account/Login';</Script>");
            }
            else
            {
                db.TempRegisters.Remove(model);
                db.SaveChanges();
                LogHelper.Log(model.Email, LogType.login.ToString(), "激活失败", "激活失败，激活码已过期，Code："+id, IpHelper.GetIp());
                return Content("<Script>alert('激活码已过期!请重新注册.');location.href='/Account/Register';</Script>");
            }
        }

        // GET: /Account/ForgetPassword
        // 忘记密码
        [HttpGet]
        public ActionResult ForgetPassword()
        {
            return View();
        }

        // POST: /Account/ForgetPassword
        // 验证
        // model: 注册邮件地址
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgetPassword(ForgetPassword model)
        {
            if (!ModelState.IsValid)
            {
                model.Code = "";
                return View(model);
            }

            if (CheckVerifyCode(model.Code))
            {
                ModelState.AddModelError("Code", "验证码不正确!");

                LogHelper.Log(model.Email, LogType.error.ToString(), "忘记密码失败", "验证码错误,客户端可能禁用了JavaScript。", IpHelper.GetIp());
                model.Code = "";
                return View(model);
            }

            if (db.TempRegisters.Where(p => p.Email == model.Email).Count() >0)
            {
                ModelState.AddModelError("", "该账号尚未激活,请前往找到激活邮件进行激活,或者重新注册!");
                model.Email = model.Code = "";
                LogHelper.Log(model.Email, LogType.login.ToString(), "忘记密码失败", "该账号尚未激活！", IpHelper.GetIp());
                return View(model);
            }

            var entity = db.LoginUsers.Where(p => p.Email == model.Email).FirstOrDefault();

            if(entity != null)
            {
                if (entity.IsDelete == true)
                {
                    ModelState.AddModelError("Email", "该用户已被删除!");
                    model.Code = "";
                    LogHelper.Log(model.Email, LogType.login.ToString(), "忘记密码失败", "该账号已被删除！", IpHelper.GetIp());
                    return View(model);
                }

                entity.Code = Guid.NewGuid().ToString();
                db.SaveChanges();

                string url = new UriBuilder(Request.Url)
                {
                    Path = Url.Action("ResetPassword", "Account", new { id = entity.Code}),
                    Query = ""
                }.ToString();
                string emailBody = "点击 <a href='" + url + "'> 此处 </a> 重置密码！ 若链接不能跳转, 请将以下链接复制到浏览器地址栏进行跳转.<br/>" + url;

                if (!SeedEmail(model.Email, "重置密码", emailBody))
                {
                    ModelState.AddModelError("", "发送重置密码邮件失败!请重新提交申请.");
                    return View(model);
                }

                LogHelper.Log(model.Email, LogType.login.ToString(), "忘记密码成功", "邮件已发送！", IpHelper.GetIp());
                return Content("<Script>alert('重置密码链接已经发动到该邮箱中,请前往查收并按照要求进行操作!');location.href='/';</Script>");
            }
            else
            {
                //虽说邮件不存在,但是也给出同样的提示,确保网站更加安全.
                LogHelper.Log(model.Email, LogType.login.ToString(), "忘记密码失败", "没有该账户！", IpHelper.GetIp());
                return Content("<Script>alert('重置密码链接已经发动到该邮箱中,请前往查收并按照要求进行操作!');location.href='/';</Script>");
            }
        }

        // GET: /Account/ResetPassword
        // 重置密码
        [HttpGet]
        public ActionResult ResetPassword(string id)
        {
            if (id == null)
            {
                LogHelper.Log("游客", LogType.error.ToString(), "重置密码失败", "Code为空，可能存在攻击！", IpHelper.GetIp());
                return View("Error");
            }

            if(db.LoginUsers.Where(p => p.Code == id).Count() > 0)
            {
                ViewBag.rCode = id;
                return View();
            }
            else
            {
                LogHelper.Log("游客", LogType.login.ToString(), "重置密码失败", "找不到该Code！Code："+id, IpHelper.GetIp());
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        // POST: /Account/ResetPassword
        // 重置密码
        // model:旧密码和新密码
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPassword model, string rCode)
        {
            ViewBag.rCode = rCode;

            if (ModelState.IsValid)
            {
                if (CheckVerifyCode(model.Code))
                {
                    ModelState.AddModelError("Code", "验证码不正确!");

                    model.Code = "";
                    LogHelper.Log("游客", LogType.error.ToString(), "重置密码失败", "验证码错误！客户端可能禁用了JavaScript。", IpHelper.GetIp());
                    return View(model);
                }

                if (rCode == null)
                {
                    LogHelper.Log("游客", LogType.login.ToString(), "重置密码失败", "Code为空！而且是在提交密码时，推测可能存在攻击！", IpHelper.GetIp());
                    return View("Error");
                }
                var entity = db.LoginUsers.Where(p => p.Code == rCode).FirstOrDefault();

                if (entity == null)
                {
                    LogHelper.Log("游客", LogType.login.ToString(), "重置密码失败", "Code不存在！应该是重复使用了链接，但是出现在提交密码时，推测可能存在攻击！", IpHelper.GetIp());
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                else
                {
                    entity.Code = "";
                    entity.Password = Md5.GetMD5(model.Password, entity.Email);
                    db.Entry<LoginUser>(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    LogHelper.Log(entity.Email, LogType.login.ToString(), "重置密码成功", "重置密码成功！", IpHelper.GetIp());
                    return Content("<Script>alert('密码修改成功!请登录.');location.href='/Account/Login';</Script>");
                }
            }
            model.Code = "";
            return View(model);
        }

        // GET: /Acccount/ChangePassword
        // 更改密码
        [HttpGet]
        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        // POST: /Account/ChangePassword
        // 处理更改密码
        // model:密码数据
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePassword model)
        {
            if (ModelState.IsValid)
            {
                if (CheckVerifyCode(model.Code))
                {
                    ModelState.AddModelError("Code", "验证码不正确!");

                    LogHelper.Log(User.Identity.Name, LogType.error.ToString(), "更改密码失败", "验证码错误！客户端可能禁用了JavaScript。", IpHelper.GetIp());
                    model.Code = "";
                    return View(model);
                }

                string email = User.Identity.Name;
                string oldPassword = Md5.GetMD5(model.OldPassword, email);
                var entity = db.LoginUsers.Where(p => p.Email == email && p.Password == oldPassword).FirstOrDefault();
                if (entity != null)
                {
                    entity.Password = Md5.GetMD5(model.Password, email);
                    db.Entry<LoginUser>(entity).State = EntityState.Modified;
                    db.SaveChanges();
                    LogHelper.Log(User.Identity.Name, LogType.login.ToString(), "更改密码成功", "更改密码成功！", IpHelper.GetIp());
                    return RedirectToAction("Logout");
                }
                else
                {
                    ModelState.AddModelError("OldPassword", "旧密码错误!");
                    model.Code = "";
                    LogHelper.Log(User.Identity.Name, LogType.login.ToString(), "更改密码失败", "旧密码错误！", IpHelper.GetIp());
                    return View(model);
                }
            }
            model.Code = "";
            return View();
        }

        // POST: /Account/CheckPassword
        // 校验旧密码
        [HttpPost]
        [Authorize]
        public ActionResult CheckOldPassword(string OldPassword)
        {
            string email = User.Identity.Name;
            string oldPassword = Md5.GetMD5(OldPassword, email);
            var entity = db.LoginUsers.Where(p => p.Email == email && p.Password == oldPassword).FirstOrDefault();
            if (entity != null)
            {
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }

        // GET:/Account/EditNews
        // 更改个人信息
        [HttpGet]
        [Authorize]
        public ActionResult EditNews()
        {
            string email = User.Identity.Name;
            return View(db.Users.Where(p => p.Email == email).FirstOrDefault());
        }

        // POST:/Account/EditNews
        // 更改个人信息
        // model:提交的个人信息
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult EditNews([Bind(Exclude = "SNo,UpdownTimes,UpdowningTime")] User model)
        {
            if (ModelState.IsValid)
            {
                if (CheckVerifyCode(model.Code))
                {
                    ModelState.AddModelError("Code", "验证码不正确!");
                    model.Code = "";
                    LogHelper.Log(User.Identity.Name, LogType.error.ToString(), "修改个人信息失败", "验证码错误！客户端可能禁用了JavaScript。", IpHelper.GetIp());
                    return View(model);
                }

                var entity = db.Users.Find(model.Id);

                if ( entity != null)
                {
                    entity.Blog = model.Blog;
                    entity.GitHub = model.GitHub;
                    entity.Introduce = model.Introduce;
                    entity.MicroBlog = model.MicroBlog;
                    entity.NickName = model.NickName;
                    entity.QQ = model.QQ;
                    entity.Tel = model.Tel;
                    entity.WeChat = model.WeChat;
                    entity.Code = model.Code;

                    db.Entry<User>(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    LogHelper.Log(User.Identity.Name, LogType.login.ToString(), "修改个人信息成功", "修改个人信息成功！", IpHelper.GetIp());
                    return Content("<Script>alert('修改信息成功!'); location.href='/Account/EditNews';</Script>");
                }
                else
                {
                    ModelState.AddModelError("", "提交信息异常,请重新提交!");
                    model.Code = "";
                    LogHelper.Log(User.Identity.Name, LogType.error.ToString(), "修改个人信息失败", "提交信息异常，客户端可能存在修改Id的攻击行为！", IpHelper.GetIp());
                    return View(model);
                }
            }
            model.Code = "";
            return View(model);
        }

        // POST: /Account/Logout
        // 注销登录
        [Authorize]
        public ActionResult Logout()
        {
            Session.Abandon();
            Session.Clear();

            FormsAuthentication.SignOut();

            LogHelper.Log(User.Identity.Name, LogType.logout.ToString(), "注销", "注销成功！", IpHelper.GetIp());
            return Redirect("/");
        }

        // GET: /Account/GetAuthCode
        // 获得验证码
        public ActionResult GetAuthCode()
        {
            return File(new VerifyCode().GetVerifyCode(), @"image/Gif");
        }

        // POST: /Account/CheckSNo
        // 校验学号
        [HttpPost]
        public ActionResult CheckSNo(string sno)
        {
            if(db.Users.Where(p => p.SNo == sno).Count() > 0)
            {
                return Json(false);
            }
            else
            {
                return Json(true);
            }
        }

        // POST: /Account/CheckEmail
        // 校验邮箱
        [HttpPost]
        public ActionResult CheckEmail(string email)
        {
            if(db.LoginUsers.Where(p => p.Email == email).Count() > 0)
            {
                return Json(false);
            }
            else
            {
                return Json(true);
            }
        }

        // POST: Account/CheckCode
        // 校验验证码
        [HttpPost]
        public ActionResult CheckCode(string code)
        {
            if (CheckVerifyCode(code))
            {
                return Json(false);
            }
            else
            {
                return Json(true);
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