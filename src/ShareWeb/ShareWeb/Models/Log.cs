using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShareWeb.Models
{
    /// <summary>
    /// 网站日志类
    /// </summary>
    public class Log
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("用户")]
        [Required(ErrorMessage = "{0}为必填项!")]
        public string User { get; set; }

        [DisplayName("IP地址")]
        [Required(ErrorMessage = "{0}为必填项!")]
        public string Ip { get; set; }

        [DisplayName("创建日期")]
        [DataType(DataType.DateTime)]
        [Required(ErrorMessage = "{0}为必填项!")]
        public DateTime CreateTime { get; set; }

        [DisplayName("类型")]
        [Required(ErrorMessage = "{0}为必填项!")]
        public string Type { get; set; }

        [DisplayName("标题")]
        [Required(ErrorMessage ="{0}为必填项")]
        public string Title { get; set; }

        [DisplayName("内容")]
        [Required(ErrorMessage = "{0}为必填项!")]
        [StringLength(2000, ErrorMessage = "{0}不能超过{1}位!")]
        public string Content { get; set; }

        [DisplayName("是否已读")]
        [Required(ErrorMessage = "{0}为必填项!")]
        public bool IsRead { get; set; }
    }

    /// <summary>
    /// 日志类型
    /// </summary>
    public enum LogType
    {
        look,   //浏览
        error,  //错误
        add,    //添加
        edit,   //编辑
        delete, //删除
        login,  //登录
        logout, //登出
        download//下载
    }
}