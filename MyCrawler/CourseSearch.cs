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
                    //不分页获取
                    //GetPageIndeData(category.Url);
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
            string pagePath = "/html/body/div[1]/div/div[2]/ul[@class='pagination']/li/a/span[@class='page-numbers']";
            HtmlNodeCollection pageNodes = document.DocumentNode.SelectNodes(pagePath);

            int pageCount = 1;
            if (pageNodes != null)
            {
                pageCount = pageNodes.Select(a => int.Parse(a.InnerText)).Max();
            }
            List<CourseEntity> courseList = new List<CourseEntity>();

            for (int pageIndex = 1; pageIndex <= pageCount; pageIndex++)
            {
                Console.WriteLine($"开始抓取第{pageIndex}页数据");
                string pageIndexUrl = $"{category.Url}page/{pageIndex}";
                List<CourseEntity> courseEntities = GetPageIndeData(pageIndexUrl);
                courseList.AddRange(courseEntities);
                Console.WriteLine($"抓取第{pageIndex}页数据完毕");
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
            string liPath = "/html/body/div[1]/div/div[2]/div";
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

            string xPathHeadImgUrl = "//*/a[1]/img";
            HtmlNode tempNode = document.DocumentNode.SelectSingleNode(xPathHeadImgUrl);
            courseEntity.HeadImgUrlWeb = tempNode.Attributes["src"].Value;
            //Console.WriteLine($"HeadImgUrlWeb='{courseEntity.HeadImgUrlWeb}'");
            //图片保存到本地
            courseEntity.HeadImgUrlDisk = ImageHelper.ImgSave("http://" + courseEntity.HeadImgUrlWeb.TrimStart('/').TrimStart('/').Split(new char[] { '?' }, StringSplitOptions.RemoveEmptyEntries)[0]);
            //Console.WriteLine($"HeadImgUrlDisk='{courseEntity.HeadImgUrlDisk}'");

            string xPathAuthor = "//*/a[2]/h2";
            tempNode = document.DocumentNode.SelectSingleNode(xPathAuthor);
            courseEntity.Author = tempNode.InnerText.Trim(new char[] { '\r','\n'});
            //Console.WriteLine($"Author='{courseEntity.Author}'");

            string xPathGender = "//*/div[1]/div";
            tempNode = document.DocumentNode.SelectSingleNode(xPathGender);
            courseEntity.Gender = tempNode.Attributes["class"].Value.Contains("women") ? 0 : 1;
            courseEntity.Age = int.Parse(tempNode.InnerText.Trim(new char[] { '\r', '\n' }));
            //Console.WriteLine($"Gender='{courseEntity.Gender}'");
            //Console.WriteLine($"Age='{courseEntity.Age}'");

            string xPathContent = "//*/a[1]/div/span";
            tempNode = document.DocumentNode.SelectSingleNode(xPathContent);
            courseEntity.Content = tempNode.InnerText.Trim(new char[] { '\r', '\n' }); ;
            //Console.WriteLine($"Content='{courseEntity.Content}'");

            string xPathUpCount = "//*/div[2]/span[1]/i";
            tempNode = document.DocumentNode.SelectSingleNode(xPathUpCount);
            courseEntity.UpCount = int.Parse(tempNode.InnerText.Trim(new char[] { '\r', '\n' }));
            //Console.WriteLine($"UpCount='{courseEntity.UpCount}'");

            string xPathCommentCount = "//*/div[2]/span[2]/a/i";
            tempNode = document.DocumentNode.SelectSingleNode(xPathCommentCount);
            courseEntity.CommentCount = int.Parse(tempNode.InnerText.Trim(new char[] { '\r', '\n' }));
            //Console.WriteLine($"CommentCount='{courseEntity.CommentCount}'");

            return courseEntity;

        }
        #endregion



    }
}
