using HtmlAgilityPack;//HtmlAgilityPack
using MyCrawler.DataService;
using MyCrawler.Model;
using MyCrawler.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

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
                logger.Error("CrawlerMuti出现异常" + ex.Message);
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
            //string pagePath = "/html/body/div[1]/div/div[2]/ul[@class='pagination']/li/a/span[@class='page-numbers']";
            HtmlNodeCollection pageNodes = document.DocumentNode.SelectNodes(category.PageXPath);

            int pageCount = 1;
            if (pageNodes != null)
            {
                pageCount = pageNodes.Select(a => int.Parse(a.InnerText.Trim(new char[] { '\r', '\n' }))).Max();
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
            //string liPath = "/html/body/div[1]/div/div[2]/div";
            HtmlNodeCollection liNodes = document.DocumentNode.SelectNodes(category.NodeXPath);

            List<CourseEntity> courseEntities = new List<CourseEntity>();
            switch (category.Type)
            {
                case "text":
                    var textEntity = SqlHelper.QueryList<CourseEntity>("select top 1 * from QiuBaiHappy where type=0 order by id desc").FirstOrDefault();
                    foreach (var node in liNodes)
                    {
                        CourseEntity courseEntity = GetTextData(node);
                        if (textEntity.Content == courseEntity.Content)
                        {
                            break;
                        }
                        courseEntities.Add(courseEntity);
                    }
                    break;
                case "imgrank":
                    var imgEntity = SqlHelper.QueryList<CourseEntity>("select top 1 * from QiuBaiHappy where type=1 order by id desc").FirstOrDefault();
                    foreach (var node in liNodes)
                    {
                        CourseEntity courseEntity = GetImgData(node);
                        if (imgEntity != null && imgEntity.Content == courseEntity.Content)
                        {
                            break;
                        }
                        courseEntities.Add(courseEntity);
                    }
                    break;
                case "video":
                    var vdieoEntity = SqlHelper.QueryList<CourseEntity>("select top 1 * from QiuBaiHappy where type=2 order by id desc").FirstOrDefault();
                    foreach (var node in liNodes)
                    {
                        CourseEntity courseEntity = GetVideoData(node);
                        if (vdieoEntity != null && vdieoEntity.Content == courseEntity.Content)
                        {
                            break;
                        }
                        courseEntities.Add(courseEntity);
                    }
                    break;
            }
            return courseEntities;
        }

        #region text页面 当我们把这些数据获取到以后，那就应该保存起来
        /// <summary>
        /// text页面 当我们把这些数据获取到以后，那就应该保存起来
        /// </summary>
        /// <param name="node"></param>
        private CourseEntity GetTextData(HtmlNode node)
        {
            CourseEntity courseEntity = new CourseEntity();
            courseEntity.Type = 0;
            courseEntity.ContentImg = "";
            courseEntity.ContentVideo = "";
            //从这里开始 
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(node.OuterHtml);

            string xPathHeadImgUrl = "//*/a[1]/img";
            HtmlNode tempNode = document.DocumentNode.SelectSingleNode(xPathHeadImgUrl);
            courseEntity.HeadImgUrlWeb = tempNode != null ? tempNode.Attributes["src"].Value : "";
            //图片保存到本地
            string path = tempNode != null ? ImageHelper.ImgSave("http:" + courseEntity.HeadImgUrlWeb.Split(new char[] { '?' }, StringSplitOptions.RemoveEmptyEntries)[0]) : "";
            courseEntity.HeadImgUrlDisk = tempNode != null ? path.Replace(@"E:\study\WeChatApplet\pages", "..") : "";

            courseEntity.Author = tempNode != null ? tempNode.Attributes["alt"].Value : "";

            string xPathGender = "//*/div[1]/div";
            tempNode = document.DocumentNode.SelectSingleNode(xPathGender);
            courseEntity.Gender = tempNode != null ? (tempNode.Attributes["class"].Value.Contains("women") ? 0 : 1) : 0;
            courseEntity.Age = tempNode != null ? int.Parse(tempNode.InnerText.Trim(new char[] { '\r', '\n' })) : 18;

            string xPathContent = "//*/a[1]/div/span";
            tempNode = document.DocumentNode.SelectSingleNode(xPathContent);
            courseEntity.Content = tempNode != null ? tempNode.InnerText.Trim(new char[] { '\r', '\n' }) : "";

            string xPathUpCount = "//*/div[2]/span[1]/i";
            tempNode = document.DocumentNode.SelectSingleNode(xPathUpCount);
            courseEntity.UpCount = tempNode != null ? int.Parse(tempNode.InnerText.Trim(new char[] { '\r', '\n' })) : 0;

            string xPathCommentCount = "//*/div[2]/span[2]/a/i";
            tempNode = document.DocumentNode.SelectSingleNode(xPathCommentCount);
            courseEntity.CommentCount = tempNode != null ? int.Parse(tempNode.InnerText.Trim(new char[] { '\r', '\n' })) : 0;

            return courseEntity;

        }
        #endregion


        #region imgrank页面 当我们把这些数据获取到以后，那就应该保存起来
        /// <summary>
        /// imgrank页面 当我们把这些数据获取到以后，那就应该保存起来
        /// </summary>
        /// <param name="node"></param>
        private CourseEntity GetImgData(HtmlNode node)
        {
            CourseEntity courseEntity = new CourseEntity();
            courseEntity.Type = 1;
            courseEntity.ContentVideo = "";
            //从这里开始 
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(node.OuterHtml);

            string xPathHeadImgUrl = "//*/div[1]/a/img";
            HtmlNode tempNode = document.DocumentNode.SelectSingleNode(xPathHeadImgUrl);
            courseEntity.HeadImgUrlWeb = tempNode != null ? tempNode.Attributes["src"].Value : "";
            //图片保存到本地
            string path = tempNode != null ? ImageHelper.ImgSave("http:" + courseEntity.HeadImgUrlWeb.Split(new char[] { '?' }, StringSplitOptions.RemoveEmptyEntries)[0]) : "";
            courseEntity.HeadImgUrlDisk = tempNode != null ? path.Replace(@"E:\study\WeChatApplet\pages", "..") : "";
            courseEntity.Author = tempNode != null ? tempNode.Attributes["alt"].Value : "";


            string xPathGender = "//*/div[1]/div";
            tempNode = document.DocumentNode.SelectSingleNode(xPathGender);
            courseEntity.Gender = tempNode != null ? (tempNode.Attributes["class"].Value.Contains("women") ? 0 : 1) : 0;
            courseEntity.Age = tempNode != null ? int.Parse(tempNode.InnerText.Trim(new char[] { '\r', '\n' })) : 18;

            string xPathContent = "//*/a[1]/div/span";
            tempNode = document.DocumentNode.SelectSingleNode(xPathContent);
            courseEntity.Content = tempNode != null ? tempNode.InnerText.Trim(new char[] { '\r', '\n' }) : "";

            string xPathContentImg = "//*/div[2]/a/img";
            tempNode = document.DocumentNode.SelectSingleNode(xPathContentImg);
            string pathContentImg = tempNode != null ? ImageHelper.ImgSave("http:" + (tempNode != null ? tempNode.Attributes["src"].Value : "")) : "";
            courseEntity.ContentImg = tempNode != null ? pathContentImg.Replace(@"E:\study\WeChatApplet\pages", "..") : "";

            string xPathUpCount = "//*/div[3]/span[1]/i";
            tempNode = document.DocumentNode.SelectSingleNode(xPathUpCount);
            courseEntity.UpCount = tempNode != null ? int.Parse(tempNode != null ? tempNode.InnerText.Trim(new char[] { '\r', '\n' }) : "0") : 0;

            string xPathCommentCount = "//*/div[3]/span[2]/a/i";
            tempNode = document.DocumentNode.SelectSingleNode(xPathCommentCount);
            courseEntity.CommentCount = tempNode != null ? int.Parse(tempNode != null ? tempNode.InnerText.Trim(new char[] { '\r', '\n' }) : "0") : 0;

            return courseEntity;

        }

        #endregion

        #region video页面 当我们把这些数据获取到以后，那就应该保存起来
        /// <summary>
        /// imgrank页面 当我们把这些数据获取到以后，那就应该保存起来
        /// </summary>
        /// <param name="node"></param>
        private CourseEntity GetVideoData(HtmlNode node)
        {
            CourseEntity courseEntity = new CourseEntity();
            courseEntity.Type = 2;
            courseEntity.ContentVideo = "";
            //从这里开始 
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(node.OuterHtml);

            string xPathHeadImgUrl = "//*/div[1]/a/img";
            HtmlNode tempNode = document.DocumentNode.SelectSingleNode(xPathHeadImgUrl);
            courseEntity.HeadImgUrlWeb = tempNode != null ? tempNode.Attributes["src"].Value : "";
            //图片保存到本地
            string path = tempNode != null ? ImageHelper.ImgSave("http:" + courseEntity.HeadImgUrlWeb.Split(new char[] { '?' }, StringSplitOptions.RemoveEmptyEntries)[0]) : "";
            courseEntity.HeadImgUrlDisk = tempNode != null ? path.Replace(@"E:\study\WeChatApplet\pages", "..") : "";
            courseEntity.Author = tempNode != null ? tempNode.Attributes["alt"].Value : "";


            string xPathGender = "//*/div[1]/div";
            tempNode = document.DocumentNode.SelectSingleNode(xPathGender);
            courseEntity.Gender = tempNode != null ? (tempNode.Attributes["class"].Value.Contains("women") ? 0 : 1) : 0;
            courseEntity.Age = tempNode != null ? int.Parse(tempNode.InnerText.Trim(new char[] { '\r', '\n' })) : 18;

            string xPathContent = "//*/a[1]/div/span";
            tempNode = document.DocumentNode.SelectSingleNode(xPathContent);
            courseEntity.Content = tempNode != null ? tempNode.InnerText.Trim(new char[] { '\r', '\n' }) : "";

            string xPathContentVideo = "//*/video[1]/source";
            tempNode = document.DocumentNode.SelectSingleNode(xPathContentVideo);
            string pathContentVideo = tempNode != null ? VideoHelper.Save("http:" + (tempNode != null ? tempNode.Attributes["src"].Value : "")) : "";
            courseEntity.ContentVideo = tempNode != null ? pathContentVideo.Replace(@"E:\study\WeChatApplet\pages", "..") : "";

            string xPathUpCount = "//*/div[2]/span[1]/i";
            tempNode = document.DocumentNode.SelectSingleNode(xPathUpCount);
            courseEntity.UpCount = tempNode != null ? int.Parse(tempNode != null ? tempNode.InnerText.Trim(new char[] { '\r', '\n' }) : "0") : 0;

            string xPathCommentCount = "//*/div[2]/span[2]/a/i";
            tempNode = document.DocumentNode.SelectSingleNode(xPathCommentCount);
            courseEntity.CommentCount = tempNode != null ? int.Parse(tempNode != null ? tempNode.InnerText.Trim(new char[] { '\r', '\n' }) : "0") : 0;

            return courseEntity;

        }

        #endregion
        #endregion



    }
}
