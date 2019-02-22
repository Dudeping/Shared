using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShareWeb.Models
{
    /// <summary>
    /// 用户类
    /// </summary>
    public class User
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("学号")]
        [StringLength(9, ErrorMessage ="{0}应该小于{1}位!")]
        public string SNo { get; set; }

        [DisplayName("昵称")]
        [StringLength(20, ErrorMessage ="{0}应该在{2}-{1}位之间!", MinimumLength =2)]
        public string NickName { get; set; }

        [DisplayName("简介")]
        [DataType(DataType.MultilineText)]
        [StringLength(2000, ErrorMessage ="{0}应该在{2}-{1}位之间!", MinimumLength =20)]
        public string Introduce { get; set; }

        [DisplayName("QQ")]
        public string QQ { get; set; }

        [DisplayName("电子邮件地址")]
        [Required(ErrorMessage ="{0}为必填项!")]
        [StringLength(255, ErrorMessage ="{0}不能超过{1}位!")]
        [DataType(DataType.EmailAddress, ErrorMessage ="{0}格式不正确!")]
        public string Email { get; set; }

        [DisplayName("电话号码")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(20, ErrorMessage ="{0}不能超过{1}位!")]
        public string Tel { get; set; }

        [DisplayName("微信号")]
        [StringLength(30, ErrorMessage ="{0}不能超过{1}位!")]
        public string WeChat { get; set; }

        [DisplayName("微博地址")]
        [DataType(DataType.Url, ErrorMessage ="{0}应为URL值")]
        [StringLength(50, ErrorMessage ="{0}不能超过{1}位!")]
        public string MicroBlog { get; set; }

        [DisplayName("博客地址")]
        [DataType(DataType.Url, ErrorMessage = "{0}应为URL值")]
        [StringLength(50, ErrorMessage = "{0}不能超过{1}位!")]
        public string Blog { get; set; }

        [DisplayName("GitHub地址")]
        [DataType(DataType.Url, ErrorMessage = "{0}应为URL值")]
        [StringLength(50, ErrorMessage = "{0}不能超过{1}位!")]
        public string GitHub { get; set; }

        //上传次数和上传时间用于限制用户每日上传的次数和两次上传所间隔的时间

        [DisplayName("上传次数")]
        [Range(0, 10, ErrorMessage ="{0}输入错误!")]
        public int UpdownTimes { get; set; }

        [DisplayName("上次上传的时间")]
        [DataType(DataType.DateTime)]
        public DateTime UpdowningTime { get; set; }

        [NotMapped]
        [DisplayName("验证码")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0}为必填项!")]
        [StringLength(4, ErrorMessage = "{0}应该为{1}位!", MinimumLength = 4)]
        [Remote("CheckCode", "Account", ErrorMessage = "验证码错误!", HttpMethod = "POST")]
        public string Code { get; set; }

        public virtual ICollection<Letter> Letters { get; set; }

        public virtual ICollection<ShareModel> ShareModels { get; set; }
    }

    /// <summary>
    /// 登录类
    /// 每建立一个用户，相应的要建立一个登录类，减少登录时信息的查询量,提高查询速度
    /// </summary>
    public class LoginUser
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("电子邮件地址")]
        [Required(ErrorMessage ="{0}为必填项!")]
        [StringLength(255, ErrorMessage ="{0}不能超过{1}位!")]
        [DataType(DataType.EmailAddress, ErrorMessage ="{0}格式不正确!")]
        public string Email { get; set; }

        [DisplayName("密码")]
        [Required(ErrorMessage ="{0}为必填项!")]
        [StringLength(40, ErrorMessage ="{0}应该在{2}-{1}之间!", MinimumLength =6)]
        public string Password { get; set; }

        [DisplayName("角色")]
        [Description("角色为Admin即为站长, 否则为空")]
        public string Role { get; set; }

        //锁定次数和锁定时间用于登录时密码错误次数过多而锁定账户一定的时间时使用

        [DisplayName("错误次数")]
        [Range(0, 3, ErrorMessage ="{0}输入错误!")]
        public int LockTimes { get; set; }

        [DisplayName("上次锁定时间")]
        [DataType(DataType.DateTime)]
        public DateTime LockingTime { get; set; }

        //用于找回密码
        [DisplayName("找回密码code")]
        public string Code { get; set; }

        [DisplayName("是否删除")]
        [Required(ErrorMessage = "{0}为必填项!")]
        public bool IsDelete { get; set; }
    }
    
    /// <summary>
    /// 临时表，用于存放还没有激活的用户信息
    /// </summary>
    public class TempRegister
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("学号")]
        [StringLength(9, ErrorMessage ="{0}不能超过{1}位!")]
        public string SNo { get; set; }

        [DisplayName("邮件地址")]
        [Required(ErrorMessage ="{0}为必填项!")]
        [DataType(DataType.EmailAddress, ErrorMessage ="{0}格式不正确!")]
        public string Email { get; set; }

        [DisplayName("密码")]
        [Required(ErrorMessage ="{0}位必填项!")]
        [StringLength(50, ErrorMessage ="{0}应该在{2}-{1}位之间!")]
        public string Password { get; set; }

        [DisplayName("激活码")]
        [Required(ErrorMessage = "{0}位必填项!")]
        public string Code { get; set; }

        [DisplayName("激活码发送时间")]
        [Description("过期时间为1天")]
        public DateTime SendTime { get; set; }
    }

    /// <summary>
    /// 登录数据模型
    /// </summary>
    public class LoginModel
    {
        [DisplayName("电子邮件地址")]
        [Required(ErrorMessage ="{0}为必填项!")]
        [StringLength(255, ErrorMessage ="{0}不能超过{1}位!")]
        [DataType(DataType.EmailAddress,ErrorMessage ="{0}格式不正确!")]
        public string Email { get; set; }

        [DisplayName("密码")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(50, ErrorMessage ="{0}应该在{2}-{1}位之间!", MinimumLength =6)]
        public string Password { get; set; }

        [DisplayName("验证码")]
        [Required(AllowEmptyStrings =false,ErrorMessage = "{0}为必填项!")]
        [StringLength(4, ErrorMessage = "{0}应该为{1}位!", MinimumLength = 4)]
        [Remote("CheckCode", "Account", ErrorMessage = "验证码错误!", HttpMethod = "POST")]
        public string Code { get; set; }
    }

    /// <summary>
    /// 注册数据模型
    /// </summary>
    public class RegisterModel
    {
        [DisplayName("学号/工号")]
        [StringLength(9, ErrorMessage = "{0}应该小于{1}位!")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0}为必填项!")]
        [Remote("CheckSNo", "Account", ErrorMessage ="该学号已用于注册，无法重复使用!", HttpMethod ="POST")]
        public string SNo { get; set; }

        [DisplayName("课程平台密码")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage ="{0}为必填项!")]
        [StringLength(20, ErrorMessage ="{0}不能超过{1}位!")]
        public string EduPassword { get; set; }

        [DisplayName("电子邮件地址")]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(255, ErrorMessage = "{0}不能超过{1}位!")]
        [DataType(DataType.EmailAddress, ErrorMessage = "{0}格式不正确!")]
        [Remote("CheckEmail", "Account", ErrorMessage = "该邮箱已注册,请直接登录!", HttpMethod = "POST")]
        public string Email { get; set; }

        [DisplayName("设置密码")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(40, ErrorMessage = "{0}应该在{2}-{1}之间!", MinimumLength = 6)]
        public string Password { get; set; }

        [DisplayName("确认密码")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(40, ErrorMessage = "{0}应该在{2}-{1}之间!", MinimumLength = 6)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage ="密码与确认密码不匹配!")]
        public string RePassword { get; set; }

        [DisplayName("验证码")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0}为必填项!")]
        [StringLength(4, ErrorMessage = "{0}应该为{1}位!", MinimumLength = 4)]
        [Remote("CheckCode", "Account", ErrorMessage = "验证码错误!", HttpMethod = "POST")]
        public string Code { get; set; }
    }

    /// <summary>
    /// 忘记密码数据模型
    /// </summary>
    public class ForgetPassword
    {
        [DisplayName("电子邮件地址")]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(255, ErrorMessage = "{0}不能超过{1}位!")]
        [DataType(DataType.EmailAddress, ErrorMessage = "{0}格式不正确!")]
        public string Email { get; set; }

        [DisplayName("验证码")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0}为必填项!")]
        [StringLength(4, ErrorMessage = "{0}应该为{1}位!", MinimumLength = 4)]
        [Remote("CheckCode", "Account", ErrorMessage ="验证码错误!", HttpMethod ="POST")]
        public string Code { get; set; }
    }

    /// <summary>
    /// 重置密码数据模型
    /// </summary>
    public class ResetPassword
    {
        [DisplayName("设置密码")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(40, ErrorMessage = "{0}应该在{2}-{1}之间!", MinimumLength = 6)]
        public string Password { get; set; }

        [DisplayName("确认密码")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(40, ErrorMessage = "{0}应该在{2}-{1}之间!", MinimumLength = 6)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "密码与确认密码不匹配!")]
        public string RePassword { get; set; }

        [DisplayName("验证码")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0}为必填项!")]
        [StringLength(4, ErrorMessage = "{0}应该为{1}位!", MinimumLength = 4)]
        [Remote("CheckCode", "Account", ErrorMessage = "验证码错误!", HttpMethod = "POST")]
        public string Code { get; set; }
    }

    public class ChangePassword
    {
        [DisplayName("旧密码")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(40, ErrorMessage = "{0}应该在{2}-{1}之间!", MinimumLength = 6)]
        [Remote("CheckOldPassword", "Account", ErrorMessage ="旧密码不正确", HttpMethod ="POST")]
        public string OldPassword { get; set; }

        [DisplayName("新密码")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(40, ErrorMessage = "{0}应该在{2}-{1}之间!", MinimumLength = 6)]
        public string Password { get; set; }

        [DisplayName("确认密码")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(40, ErrorMessage = "{0}应该在{2}-{1}之间!", MinimumLength = 6)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "新密码与确认密码不匹配!")]
        public string RePassword { get; set; }

        [DisplayName("验证码")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0}为必填项!")]
        [StringLength(4, ErrorMessage = "{0}应该为{1}位!", MinimumLength = 4)]
        [Remote("CheckCode", "Account", ErrorMessage = "验证码错误!", HttpMethod = "POST")]
        public string Code { get; set; }
    }
    /// <summary>
    /// 邮箱配置
    /// </summary>
    public class EmailConfigurationProvider : ConfigurationSection
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return this["name"].ToString(); }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("account", IsRequired = true)]
        public string Account
        {
            get { return this["account"].ToString(); }
            set { this["account"] = value; }
        }

        [ConfigurationProperty("password", IsRequired = true)]
        public string Password
        {
            get { return this["password"].ToString(); }
            set { this["password"] = value; }
        }

        [ConfigurationProperty("server", IsRequired = true)]
        public string Server
        {
            get { return this["server"].ToString(); }
            set { this["server"] = value; }
        }

        [ConfigurationProperty("port", IsRequired = true)]
        public int Port
        {
            get { return Int32.Parse(this["port"].ToString()); }
            set { this["port"] = value; }
        }

        [ConfigurationProperty("isSSL", IsRequired = true)]
        public bool IsSSL
        {
            get { return Boolean.Parse(this["isSSL"].ToString()); }
            set { this["isSSL"] = value; }
        }
    }
}