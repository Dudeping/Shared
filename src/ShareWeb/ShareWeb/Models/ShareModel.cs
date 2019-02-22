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
    /// 分享类
    /// </summary>
    public class ShareModel
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("名称")]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(20, ErrorMessage ="{0}应该在{2}-{1}位之间!", MinimumLength =2)]
        public string Name { get; set; }

        [DisplayName("类别")]
        [Required(ErrorMessage = "{0}为必填项!")]
        public virtual Category Category { get; set; }

        [DisplayName("简介")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(5000, ErrorMessage = "{0}应该在{2}-{1}位之间!", MinimumLength = 2)]
        public string Introduce { get; set; }

        [DisplayName("应用场景")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(5000, ErrorMessage = "{0}应该在{2}-{1}位之间!", MinimumLength = 2)]
        public string Apply { get; set; }

        [DisplayName("开发工具")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(5000, ErrorMessage = "{0}应该在{2}-{1}位之间!", MinimumLength = 2)]
        public string Tool { get; set; }

        [DisplayName("技术亮点")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(5000, ErrorMessage = "{0}应该在{2}-{1}位之间!", MinimumLength = 2)]
        public string Skill { get; set; }

        [DisplayName("部署环境")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(5000, ErrorMessage = "{0}应该在{2}-{1}位之间!", MinimumLength = 2)]
        public string Deploy { get; set; }

        [DisplayName("效果图")]
        [DataType(DataType.Upload)]
        [Required(ErrorMessage = "{0}为必填项!")]
        public string ResultPic { get; set; }

        [DisplayName("示例地址")]
        [DataType(DataType.Url)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(100, ErrorMessage = "{0}不能超过{1}位!")]
        public string DemoAddress { get; set; }

        [DisplayName("源码下载")]
        [DataType(DataType.Upload)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(100, ErrorMessage = "{0}不能超过{1}位!")]
        public string CodeAddress { get; set; }

        [DisplayName("GitHub地址")]
        [DataType(DataType.Url)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(100, ErrorMessage = "{0}不能超过{1}位!")]
        public string GitHubAddress { get; set; }

        [DisplayName("原网址")]
        [DataType(DataType.Url)]
        [StringLength(100, ErrorMessage = "{0}不能超过{1}位!")]
        public string OldAddress { get; set; }

        [DisplayName("作者")]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(20, ErrorMessage = "{0}不能超过{1}位!")]
        public string Author { get; set; }

        [DisplayName("上传者")]
        [Required(ErrorMessage = "{0}为必填项!")]
        public virtual User Editor { get; set; }

        [DisplayName("浏览量")]
        [Required(ErrorMessage = "{0}为必填项!")]
        public int Look { get; set; }

        [DisplayName("下载量")]
        [Required(ErrorMessage = "{0}为必填项!")]
        public int Download { get; set; }

        [DisplayName("创建时间")]
        [DataType(DataType.DateTime)]
        [Required(ErrorMessage ="{0}为必填项!")]
        public DateTime CreateTime { get; set; }
    }

    public class MShare
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("名称")]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(20, ErrorMessage = "{0}应该在{2}-{1}位之间!", MinimumLength = 2)]
        [Remote("CheckShare", "UploadShare", ErrorMessage ="已有该模板,请勿重复上传!", HttpMethod ="Post")]
        public string Name { get; set; }

        [DisplayName("类别")]
        [Required(ErrorMessage = "{0}为必填项!")]
        public string Category { get; set; }

        [DisplayName("简介")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(5000, ErrorMessage = "{0}应该在{2}-{1}位之间!", MinimumLength = 2)]
        public string Introduce { get; set; }

        [DisplayName("应用场景")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(5000, ErrorMessage = "{0}应该在{2}-{1}位之间!", MinimumLength = 2)]
        public string Apply { get; set; }

        [DisplayName("开发工具")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(5000, ErrorMessage = "{0}应该在{2}-{1}位之间!", MinimumLength = 2)]
        public string Tool { get; set; }

        [DisplayName("技术亮点")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(5000, ErrorMessage = "{0}应该在{2}-{1}位之间!", MinimumLength = 2)]
        public string Skill { get; set; }

        [DisplayName("部署环境")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(5000, ErrorMessage = "{0}应该在{2}-{1}位之间!", MinimumLength = 2)]
        public string Deploy { get; set; }

        [DisplayName("示例地址")]
        [DataType(DataType.Url)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(100, ErrorMessage = "{0}不能超过{1}位!")]
        public string DemoAddress { get; set; }

        [DisplayName("GitHub地址")]
        [DataType(DataType.Url)]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(100, ErrorMessage = "{0}不能超过{1}位!")]
        public string GitHubAddress { get; set; }

        [DisplayName("原网址")]
        [DataType(DataType.Url)]
        [StringLength(100, ErrorMessage = "{0}不能超过{1}位!")]
        public string OldAddress { get; set; }

        [DisplayName("模板作者")]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(20, ErrorMessage = "{0}不能超过{1}位!")]
        public string Author { get; set; }

        [NotMapped]
        [DisplayName("验证码")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0}为必填项!")]
        [StringLength(4, ErrorMessage = "{0}应该为{1}位!", MinimumLength = 4)]
        [Remote("CheckCode", "Account", ErrorMessage = "验证码错误!", HttpMethod = "POST")]
        public string Code { get; set; }
    }

    public class ManageShare
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public virtual Category Category { get; set; }
    }
}