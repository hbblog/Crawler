using HtmlAgilityPack;//HtmlAgilityPack
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MyCrawler.Utility;
using MyCrawler.DataService;
using MyCrawler.Model;
using System.Threading;

namespace MyCrawler
{
    /// <summary>
    /// 商品抓取
    /// http://www.w3school.com.cn/xpath/index.asp XPATH语法
    /// 
    /// 1 HtmlAgilityPack还挺方便
    /// 2 订制，不同网站都要订制；
    ///   同一网站基本不需要升级
    /// </summary>
    public class CourseSearch : ISearch
    {
        private Logger logger = new Logger(typeof(CourseSearch));
        private WarnRepository warnRepository = new WarnRepository();
        private CourseRepository courseRepository = new CourseRepository();
        private TencentCategoryEntity category = null;

        public CourseSearch(TencentCategoryEntity _category)
        {
            category = _category;
        } 
        //请大家思考一下：如果需要爬虫获取腾讯课堂所有的类目信息，如何获取呢?
        //   如果数据量在非常大（百万级数据）的情况下，如何提高爬虫的效率！
         
        // 现在是11：29   大家可以提提问，老师在线解答一下你们的问题！

        public void Crawler()
        {
            try
            {
                if (string.IsNullOrEmpty(category.Url))
                {
                    warnRepository.SaveWarn(category, string.Format("Url为空,Name={0} Level={1} Url={2}", category.Name, category.CategoryLevel, category.Url));
                    return;
                }
                {
                    #region 分页获取  
                    //ImageHelper.DeleteDir(Constant.ImagePath);
                    GetPageCourseData();
                    #endregion
                }
            }
            catch (Exception ex)
            {
                logger.Error("CrawlerMuti出现异常", ex);
                warnRepository.SaveWarn(category, string.Format("出现异常,Name={0} Level={1} Url={2}", category.Name, category.CategoryLevel, category.Url));
            }
        }


        #region 分页抓取

        private void GetPageCourseData()
        {
            //1. 确定总页数
            //2. 分别抓取每一页的数据
            //3. 分析  过滤  清洗
            //4. 入库 

            category.Url = $"{Constant.TencentClassUrl}{category.Url}";

            string strHtml = HttpHelper.DownloadUrl(category.Url);
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(strHtml);

            //Xpath
            string pagePath = "/html/body/section[1]/div/div[@class='sort-page']/a[@class='page-btn']";
            HtmlNodeCollection pageNodes = document.DocumentNode.SelectNodes(pagePath);

            int pageCount = 1;
            if (pageNodes != null)
            {
                pageCount = pageNodes.Select(a => int.Parse(a.InnerText)).Max();
            }
            List<CourseEntity> courseList = new List<CourseEntity>();

            for (int pageIndex = 1; pageIndex <= pageCount; pageIndex++)
            {
                Console.WriteLine($"******************************当前是第{pageIndex}页数据************************************");
                string pageIndexUrl = $"{category.Url}&page={pageIndex}";
                List<CourseEntity> courseEntities = GetPageIndeData(pageIndexUrl);
                courseList.AddRange(courseEntities);
            }
            courseRepository.SaveList(courseList);


        }

        private List<CourseEntity> GetPageIndeData(string url)
        {
            //获取li标签里面的数据 
            // 先获取所有的Li 
            //  然后循环获取li中的有效数据
            string strHtml = HttpHelper.DownloadUrl(url);
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(strHtml);
            string liPath = "/html/body/section[1]/div/div[@class='market-bd market-bd-6 course-list course-card-list-multi-wrap js-course-list']/ul/li";
            HtmlNodeCollection liNodes = document.DocumentNode.SelectNodes(liPath);

            List<CourseEntity> courseEntities = new List<CourseEntity>();
            foreach (var node in liNodes)
            {
                CourseEntity courseEntity = GetLiData(node);
                courseEntities.Add(courseEntity);
            } 
            return courseEntities;
        }

        /// <summary>
        /// 当我们把这些数据获取到以后，那就应该保存起来
        /// </summary>
        /// <param name="node"></param>
        private CourseEntity GetLiData(HtmlNode node)
        {
            CourseEntity courseEntity = new CourseEntity();
            //从这里开始 
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(node.OuterHtml);
            string aPath = "//*/a[1]";
            HtmlNode classANode = document.DocumentNode.SelectSingleNode(aPath);
            string aHref = classANode.Attributes["href"].Value;
            courseEntity.Url = aHref;

            Console.WriteLine($"课程Url:{aHref}");

            string Id = classANode.Attributes["data-id"].Value;

            Console.WriteLine($"课程Id:{Id}");

            courseEntity.CourseId = long.Parse(Id);

            string imgPath = "//*/a[1]/img";
            HtmlNode imgNode = document.DocumentNode.SelectSingleNode(imgPath);
            string imgUrl = imgNode.Attributes["src"].Value;
            courseEntity.ImageUrl = imgUrl;

            Console.WriteLine($"ImageUrl:{imgUrl}");
             
            string namePaths = "//*/h4/a[1]";
            HtmlNode nameNode = document.DocumentNode.SelectSingleNode(namePaths);
            string name = nameNode.InnerText;

            courseEntity.Title = name;

            Console.WriteLine($"课程名称:{name}");

            courseEntity.Price = new Random().Next(100, 10000);  //关于腾讯课堂上的课程价格抓取 这是一个进阶内容  通过普通方式搞不了（他有一个自己的算法） 
            return courseEntity;

        }
        #endregion



    }
}
