using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ShareWeb.Models
{
    public class ShareWebContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public ShareWebContext() : base("name=ShareWebContext")
        {
        }

        public System.Data.Entity.DbSet<ShareWeb.Models.User> Users { get; set; }

        public System.Data.Entity.DbSet<ShareWeb.Models.Category> Categories { get; set; }

        public System.Data.Entity.DbSet<ShareWeb.Models.Letter> Letters { get; set; }

        public System.Data.Entity.DbSet<ShareWeb.Models.Log> Logs { get; set; }

        public System.Data.Entity.DbSet<ShareWeb.Models.LoginUser> LoginUsers { get; set; }

        public System.Data.Entity.DbSet<ShareWeb.Models.ShareModel> ShareModels { get; set; }

        public System.Data.Entity.DbSet<ShareWeb.Models.WebConfig> WebConfigs { get; set; }

        public System.Data.Entity.DbSet<ShareWeb.Models.TempRegister> TempRegisters { get; set; }
    }
}
