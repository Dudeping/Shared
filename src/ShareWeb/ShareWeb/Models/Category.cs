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
    /// 分享的类别类
    /// </summary>
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "名称")]
        [Required(ErrorMessage ="{0}为必填项!")]
        [StringLength(20, ErrorMessage ="{0}应该在{2}-{1}位之间!", MinimumLength =2)]
        public string Name { get; set; }

        [Display(Name = "作者")]
        [DataType(DataType.EmailAddress, ErrorMessage ="{0}应为电子邮件地址!")]
        [Required(ErrorMessage ="{0}为不能为空!")]
        public string Author { get; set; }

        [NotMapped]
        [DisplayName("验证码")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0}为必填项!")]
        [StringLength(4, ErrorMessage = "{0}应该为{1}位!", MinimumLength = 4)]
        [Remote("CheckCode", "Account", ErrorMessage = "验证码错误!", HttpMethod = "POST")]
        public string Code { get; set; }

        public virtual ICollection<ShareModel> ShareModels { get; set; }
    }

    public class CategoryModel
    {
        public int Id { get; set; }

        [Display(Name = "名称")]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(20, ErrorMessage = "{0}应该在{2}-{1}位之间!", MinimumLength = 2)]
        [Remote("CheckCategory","UploadShare", ErrorMessage ="已有该类别!请勿重复添加.", HttpMethod ="POST")]
        public string Name { get; set; }

        [NotMapped]
        [DisplayName("验证码")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0}为必填项!")]
        [StringLength(4, ErrorMessage = "{0}应该为{1}位!", MinimumLength = 4)]
        [Remote("CheckCode", "Account", ErrorMessage = "验证码错误!", HttpMethod = "POST")]
        public string Code { get; set; }
    }
}