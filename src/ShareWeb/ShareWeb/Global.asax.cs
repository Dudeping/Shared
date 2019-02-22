using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace ShareWeb
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //应用程序入口
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        //添加角色
        public Global()
        {
            BeginRequest += MvcApplication_BeginRequest;
            AuthorizeRequest += new EventHandler(MvcApplication_AuthorizeRequest);
        }

        void MvcApplication_AuthorizeRequest(object sender, EventArgs e)
        {
            var id = Context.User.Identity as FormsIdentity;
            if (id != null && id.IsAuthenticated)
            {
                var roles = id.Ticket.UserData.Split(',');
                Context.User = new GenericPrincipal(id, roles);
            }
        }

        private void MvcApplication_BeginRequest(object sender, EventArgs e)
        {
            #if DEBUG
            #else
            // 301永久定向
            if (!Request.IsSecureConnection)
            {
                var newUrl = new UriBuilder(Request.Url)
                {
                    Port = 443,
                    Scheme = "HTTPS"
                }.ToString();

                Response.RedirectPermanent(newUrl);
            }
            #endif
        }
    }
}
