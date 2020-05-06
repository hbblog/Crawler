using MyCrawler.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyCrawler
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary> 
        static void Main()
        {
            //欢迎大家来到朝夕教育的体验课！今天给大家分享网络爬虫。。。。。

            try
            {
                TencentCategoryEntity category = new TencentCategoryEntity()
                {
                    Url = "https://ke.qq.com/course/list/.Net?tuin=a3ff93bc"
                };

                #region 开始抓取数据

               //CrawlerCenter.CrawlerCourse();

              CrawlerCenter.Handler();
                #endregion 
                #region 抓取腾讯课堂类别数据 
                //ISearch search = new CategorySearch();
                //search.Crawler();
                #endregion


                #region 抓取课程
                //ISearch search = new CourseSearch(category);
                //search.Crawler();
                #endregion

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }



        }
    }
}
