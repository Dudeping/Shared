using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShareWeb.Models
{
    /// <summary>
    /// 网站配置模型
    /// </summary>
    public class WebConfig
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("网站简介")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(5000, ErrorMessage = "{0}应该在{2}-{1}位之间!", MinimumLength = 2)]
        public string Introduce { get; set; }

        [DisplayName("网站用途")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(5000, ErrorMessage = "{0}应该在{2}-{1}位之间!", MinimumLength = 2)]
        public string Use { get; set; }

        [DisplayName("使用方法")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(5000, ErrorMessage = "{0}应该在{2}-{1}位之间!", MinimumLength = 2)]
        public string Method { get; set; }

        [DisplayName("网站特点")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(5000, ErrorMessage = "{0}应该在{2}-{1}位之间!", MinimumLength = 2)]
        public string Point { get; set; }

        [DisplayName("网站历史")]
        [Required(ErrorMessage = "{0}为必填项!")]
        [Description("格式：(版本 ##### 内容 ##### 日期)|(...)|(...)")]
        [StringLength(2147483646, ErrorMessage = "{0}应该在{2}-{1}位之间!", MinimumLength = 2)]
        public string History { get; set; }

        [DisplayName("姓名")]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(4, ErrorMessage = "{0}应该在{2}-{1}位之间!", MinimumLength = 2)]
        public string Name { get; set; }

        [DisplayName("籍贯")]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(20, ErrorMessage = "{0}应该在{2}-{1}位之间!", MinimumLength = 2)]
        public string OldAddress { get; set; }

        [DisplayName("现居")]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(20, ErrorMessage = "{0}应该在{2}-{1}位之间!", MinimumLength = 2)]
        public string NowAddress { get; set; }

        [DisplayName("职业")]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(20, ErrorMessage = "{0}应该在{2}-{1}位之间!", MinimumLength = 2)]
        public string Job { get; set; }

        [DisplayName("喜欢的书")]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(200, ErrorMessage = "{0}应该在{2}-{1}位之间!", MinimumLength = 2)]
        public string LikeBook { get; set; }

        [DisplayName("喜欢的音乐")]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(200, ErrorMessage = "{0}应该在{2}-{1}位之间!", MinimumLength = 2)]
        public string LikeMusic { get; set; }
    }

    /// <summary>
    /// 站长模型
    /// </summary>
    public class WebMaster
    {
        [DisplayName("姓名")]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(4, ErrorMessage = "{0}应该在{2}-{1}位之间!", MinimumLength = 2)]
        public string Name { get; set; }

        [DisplayName("籍贯")]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(20, ErrorMessage = "{0}应该在{2}-{1}位之间!", MinimumLength = 2)]
        public string OldAddress { get; set; }

        [DisplayName("现居")]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(20, ErrorMessage = "{0}应该在{2}-{1}位之间!", MinimumLength = 2)]
        public string NowAddress { get; set; }

        [DisplayName("职业")]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(20, ErrorMessage = "{0}应该在{2}-{1}位之间!", MinimumLength = 2)]
        public string Job { get; set; }

        [DisplayName("喜欢的书")]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(200, ErrorMessage = "{0}应该在{2}-{1}位之间!", MinimumLength = 2)]
        public string LikeBook { get; set; }

        [DisplayName("喜欢的音乐")]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(200, ErrorMessage = "{0}应该在{2}-{1}位之间!", MinimumLength = 2)]
        public string LikeMusic { get; set; }

        [NotMapped]
        [DisplayName("验证码")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0}为必填项!")]
        [StringLength(4, ErrorMessage = "{0}应该为{1}位!", MinimumLength = 4)]
        [Remote("CheckCode", "Account", ErrorMessage = "验证码错误!", HttpMethod = "POST")]
        public string Code { get; set; }
    }

    /// <summary>
    /// 网站信息模型
    /// </summary>
    public class WebNews
    {
        [DisplayName("网站简介")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(5000, ErrorMessage = "{0}应该在{2}-{1}位之间!", MinimumLength = 2)]
        public string Introduce { get; set; }

        [DisplayName("网站用途")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(5000, ErrorMessage = "{0}应该在{2}-{1}位之间!", MinimumLength = 2)]
        public string Use { get; set; }

        [DisplayName("使用方法")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(5000, ErrorMessage = "{0}应该在{2}-{1}位之间!", MinimumLength = 2)]
        public string Method { get; set; }

        [DisplayName("网站特点")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(5000, ErrorMessage = "{0}应该在{2}-{1}位之间!", MinimumLength = 2)]
        public string Point { get; set; }

        [NotMapped]
        [DisplayName("验证码")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0}为必填项!")]
        [StringLength(4, ErrorMessage = "{0}应该为{1}位!", MinimumLength = 4)]
        [Remote("CheckCode", "Account", ErrorMessage = "验证码错误!", HttpMethod = "POST")]
        public string Code { get; set; }
    }

    /// <summary>
    /// 网站历史模型
    /// </summary>
    public class WebHistory
    {
        [DisplayName("网站版本")]
        [Required(ErrorMessage ="{0}为必填项!")]
        [StringLength(20, ErrorMessage ="{0}应该在{2}-{1}之间!", MinimumLength =2)]
        [Remote("CheckVersion", "Admin", ErrorMessage ="已有该版本!", HttpMethod ="POST")]
        public string Version { get; set; }

        [DisplayName("版本内容")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(2000, ErrorMessage = "{0}应该在{2}-{1}之间!", MinimumLength = 10)]
        public string Content { get; set; }

        [DisplayName("创建时间")]
        [DataType(DataType.DateTime)]
        public DateTime CreateTime { get; set; }

        [NotMapped]
        [DisplayName("验证码")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0}为必填项!")]
        [StringLength(4, ErrorMessage = "{0}应该为{1}位!", MinimumLength = 4)]
        [Remote("CheckCode", "Account", ErrorMessage = "验证码错误!", HttpMethod = "POST")]
        public string Code { get; set; }
    }
}