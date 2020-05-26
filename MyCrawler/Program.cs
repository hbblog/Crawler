using MyCrawler.Model;
using MyCrawler.Utility;
using System;

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

            Logger logger = new Logger(typeof(Program));
            try
            {
                #region 抓取笑话
                TencentCategoryEntity categoryText = new TencentCategoryEntity()
                {
                    Url = "https://www.qiushibaike.com/text/",
                    Type = "text",
                    PageXPath = "/html/body/div[1]/div/div[2]/ul[@class='pagination']/li/a/span[@class='page-numbers']",
                    NodeXPath = "/html/body/div[1]/div/div[2]/div"
                };

                ISearch searchText = new CourseSearch(categoryText);
                searchText.Crawler();
                logger.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "抓取段子执行成功\r\n");
                #endregion


                #region 抓取趣图
                TencentCategoryEntity categoryImg = new TencentCategoryEntity()
                {
                    Url = "https://www.qiushibaike.com/imgrank/",
                    Type = "imgrank",
                    PageXPath = "/html/body/div[1]/div/div[2]/ul/li/a/span[@class='page-numbers']",
                    NodeXPath = "/html/body/div[1]/div/div[2]/div"
                };
                #endregion

                ISearch searchImg = new CourseSearch(categoryImg);
                searchImg.Crawler();
                logger.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "抓取趣图执行成功\r\n");


                //Console.ReadLine();
            }
            catch (Exception ex)
            {
                logger.Error(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "执行失败，原因：" + ex.Message + "\r\n");
            }

        }
    }
}
