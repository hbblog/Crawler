using MyCrawler.Model;
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

            try
            {
                #region 抓取笑话
                //TencentCategoryEntity categoryText = new TencentCategoryEntity()
                //{
                //    Url = "https://www.qiushibaike.com/text/",
                //    Type = "text",
                //    PageXPath = "/html/body/div[1]/div/div[2]/ul[@class='pagination']/li/a/span[@class='page-numbers']",
                //    NodeXPath = "/html/body/div[1]/div/div[2]/div"
                //};
                #endregion
                #region 抓取趣图
                TencentCategoryEntity categoryImg = new TencentCategoryEntity()
                {
                    ////*[@id="qiushi_tag_123079374"]/div[2]/a
                    Url = "https://www.qiushibaike.com/imgrank/",
                    Type = "imgrank",
                    PageXPath = "/html/body/div[1]/div/div[2]/ul/li/a/span[@class='page-numbers']",
                    NodeXPath = "/html/body/div[1]/div/div[2]/div"
                };
                #endregion



                ISearch searchImg = new CourseSearch(categoryImg);
                searchImg.Crawler();

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }



        }
    }
}
