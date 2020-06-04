using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCrawler.Utility
{
    /// <summary>
    /// 系统配置项
    /// </summary>
    public class Constant
    {
        /// <summary>
        /// 数据文件保存路径
        /// </summary>
        public static string DataPath = ConfigurationManager.AppSettings["DataPath"];

        /// <summary>
        /// 头像图片保存路径
        /// </summary>
        public static string HeadImagePath = ConfigurationManager.AppSettings["HeadImagePath"];

        /// <summary>
        /// 图片保存路径
        /// </summary>
        public static string ContentImagePath = ConfigurationManager.AppSettings["ContentImagePath"];

        /// <summary>
        /// 视频保存路径
        /// </summary>
        public static string VideoPath = ConfigurationManager.AppSettings["VideoPath"];

        /// <summary>
        /// 腾讯课堂入口
        /// </summary>
        public static string TencentClassUrl = ConfigurationManager.AppSettings["TencentClassUrl"];

        /// <summary>
        /// 用作爬虫在获取数据时候记录数量
        /// </summary>
        public static int Count = 1;
    }
}
