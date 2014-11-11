using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EBuy.Models
{
    /// <summary>
    /// 提醒注意
    /// </summary>
    public class Notice
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 详细内容
        /// </summary>
        public string Details { get; set; }
        /// <summary>
        /// 停留时间【-1-不启用】
        /// </summary>
        public int DelayTime { get; set; }
        /// <summary>
        /// 导航名称
        /// </summary>
        public string NavigationName { get; set; }
        /// <summary>
        /// 导航地址
        /// </summary>
        public string NavigationUrl { get; set; }
    }

    /// <summary>
    /// 错误
    /// </summary>
    public class Error
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 详细
        /// </summary>
        public string Details { get; set; }
        /// <summary>
        /// 原因
        /// </summary>
        public string Cause { get; set; }
        /// <summary>
        /// 解决方法
        /// </summary>
        public string Solution { get; set; }
    }
}