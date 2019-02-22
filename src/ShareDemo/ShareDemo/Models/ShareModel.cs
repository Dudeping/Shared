using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShareDemo.Models
{
    /// <summary>
    /// 每一页的数据模型
    /// </summary>
    public class ShareModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Decription { get; set; }

        public List<LinkModel> Link { get; set; }
    }

    /// <summary>
    /// 链接的数据模型
    /// </summary>
    public class LinkModel
    {
        public string LinkText { get; set; }

        //保护字段
        private string linkSrc;
        
        //保护字段的属性
        public string LinkSrc
        {
            get { return linkSrc; }
            set
            {
                //如果是一个网络地址，那么就直接写入，如果是一个Id，那么就要稍作拼接
                int result;
                if (Int32.TryParse(value, out result))
                {
                    linkSrc = "/Home/Index/" + value;
                }
                else
                {
                    linkSrc = value;
                }
            }
        }
    }
}