using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShareWeb.Models
{
    /// <summary>
    /// 站内信数据模型
    /// </summary>
    public class Letter
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("主题")]
        [Required(ErrorMessage ="{0}为必填项!")]
        [StringLength(25, ErrorMessage ="{0}应该在{2}-{1}之间!", MinimumLength =2)]
        public string Sub { get; set; }

        [DisplayName("类别")]
        [Required(ErrorMessage = "{0}为必填项!")]
        public string Type { get; set; }

        [DisplayName("内容")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(5000, ErrorMessage = "{0}应该在{2}-{1}之间!", MinimumLength = 5)]
        public string Content { get; set; }

        [DisplayName("接收者")]
        [Required(ErrorMessage = "{0}为必填项!")]
        [DataType(DataType.EmailAddress, ErrorMessage ="{0}应为邮件地址!")]
        public string To { get; set; }

        [DisplayName("发送者")]
        [Required(ErrorMessage = "{0}为必填项!")]
        public virtual User From { get; set; }

        [DisplayName("创建时间")]
        [Required(ErrorMessage = "{0}为必填项!")]
        [DataType(DataType.DateTime)]
        public DateTime CreateTime { get; set; }

        [DisplayName("是否已读")]
        [Required(ErrorMessage = "{0}为必填项!")]
        public bool IsRead { get; set; }
        
        [DisplayName("是否重要")]
        [Required(ErrorMessage = "{0}为必填项!")]
        public bool IsInportant { get; set; }

        [DisplayName("回复")]
        [Description("格式(主题#####内容)")]
        [StringLength(2000, ErrorMessage = "{0}应该在{2}-{1}之间!", MinimumLength = 2)]
        public string Reply { get; set; }
    }

    public class CooperationModel
    {
        [DisplayName("主题")]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(25, ErrorMessage = "{0}应该在{2}-{1}之间!", MinimumLength = 2)]
        public string Sub { get; set; }

        [DisplayName("内容")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(5000, ErrorMessage = "{0}应该在{2}-{1}之间!", MinimumLength = 5)]
        public string Content { get; set; }

        [DisplayName("验证码")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0}为必填项!")]
        [StringLength(4, ErrorMessage = "{0}应该为{1}位!", MinimumLength = 4)]
        [Remote("CheckCode", "Account", ErrorMessage = "验证码错误!", HttpMethod = "POST")]
        public string Code { get; set; }
    }

    public class Reply
    {
        [Required(ErrorMessage = "{0}为必填项!")]
        public int LetterId { get; set; }

        [DisplayName("主题")]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(25, ErrorMessage = "{0}应该在{2}-{1}之间!", MinimumLength = 2)]
        public string Sub { get; set; }

        [DisplayName("内容")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(5000, ErrorMessage = "{0}应该在{2}-{1}之间!", MinimumLength = 5)]
        public string Content { get; set; }

        [DisplayName("验证码")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0}为必填项!")]
        [StringLength(4, ErrorMessage = "{0}应该为{1}位!", MinimumLength = 4)]
        [Remote("CheckCode", "Account", ErrorMessage = "验证码错误!", HttpMethod = "POST")]
        public string Code { get; set; }
    }

    public class PublishNews
    {
        [DisplayName("内容")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(5000, ErrorMessage = "{0}应该在{2}-{1}之间!", MinimumLength = 5)]
        public string Content { get; set; }

        [DisplayName("验证码")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0}为必填项!")]
        [StringLength(4, ErrorMessage = "{0}应该为{1}位!", MinimumLength = 4)]
        [Remote("CheckCode", "Account", ErrorMessage = "验证码错误!", HttpMethod = "POST")]
        public string Code { get; set; }
    }

    public class ReleaseLetter
    {
        [DisplayName("接收者")]
        [Required(ErrorMessage = "{0}为必填项!")]
        [DataType(DataType.EmailAddress, ErrorMessage = "{0}应为邮件地址!")]
        [Remote("CheckEmail", "Admin", ErrorMessage = "无该用户!", HttpMethod = "POST")]
        public string To { get; set; }

        [DisplayName("主题")]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(25, ErrorMessage = "{0}应该在{2}-{1}之间!", MinimumLength = 2)]
        public string Sub { get; set; }

        [DisplayName("内容")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(5000, ErrorMessage = "{0}应该在{2}-{1}之间!", MinimumLength = 5)]
        public string Content { get; set; }

        [DisplayName("验证码")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0}为必填项!")]
        [StringLength(4, ErrorMessage = "{0}应该为{1}位!", MinimumLength = 4)]
        [Remote("CheckCode", "Account", ErrorMessage = "验证码错误!", HttpMethod = "POST")]
        public string Code { get; set; }
    }

    /// <summary>
    /// 信件类型
    /// </summary>
    public enum LetterType
    {
        system, //系统消息
        contact,//联系
        feedBack//反馈
    }
}