using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MyCrawler.Utility;
using MyCrawler.Model;
using MyCrawler.DataServices;
using MyCrawler.DataService;

namespace MyCrawler
{
    /// <summary>
    /// http://www.w3school.com.cn/xpath/index.asp XPATH语法
    /// </summary>
    public class CategorySearch : ISearch
    {
        private static Logger logger = new Logger(typeof(CategorySearch));
        private int _Count = 1;//每次都得new一个 重新初始化类别


        /// <summary>
        /// 如果爬虫需要获取腾讯课堂所有的课程数据，需要通过类目来获取
        /// 
        /// 还是 请求获取Html内容   解析过滤信息， 获取有效信息入库
        /// </summary>
        public void Crawler()
        {

            //Console.WriteLine("请输入Y/N进行类别表初始化确认！ Y 删除Tencent_Category表然后重新创建，然后抓取类型数据，N（或者其他）跳过");
            //string input = Console.ReadLine();
            //if (input.Equals("Y", StringComparison.OrdinalIgnoreCase))
            //{
            //    DBInit.InitCategoryTable();
            //}
            //else
            //{
            //    Console.WriteLine("你选择不初始化类别数据");
            //}
            //Console.WriteLine("*****************^_^**********************");

            List<TencentCategoryEntity> categoryList = new List<TencentCategoryEntity>();
            try
            {
                string url = $"{Constant.TencentClassUrl}/course/list/?tuin=7e4f8b7d";
                string html = HttpHelper.DownloadUrl(url);

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                string fristPath = "//*[@id=\"auto-test-1\"]/div[1]/dl/dd";
                HtmlNodeCollection nodeList = doc.DocumentNode.SelectNodes(fristPath);
                if (nodeList == null)
                {

                }
                foreach (HtmlNode node in nodeList)
                {
                    categoryList.AddRange(this.First(node.InnerHtml, null));
                }

                CategoryRepository categoryRepository = new CategoryRepository();
                categoryRepository.Save(categoryList);
            }
            catch (Exception ex)
            {
                logger.Error("CrawlerMuti出现异常", ex);
            }
            finally
            {
                Console.WriteLine($"类型数据初始化完成，共抓取类别{ categoryList?.Count}个");
            }
        }



        /// <summary>
        /// 对每一个一级类进行查找
        /// </summary>
        /// <param name="html"></param>
        /// <param name="code"></param>
        /// <param name="parentCode"></param>
        /// <returns></returns>
        private List<TencentCategoryEntity> First(string html, string parentCode)
        {
            List<TencentCategoryEntity> categoryList = new List<TencentCategoryEntity>();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            string namePath = "//a/h2";
            HtmlNode name = doc.DocumentNode.SelectSingleNode(namePath);
            string codePath = "//a";
            HtmlNode codeNode = doc.DocumentNode.SelectSingleNode(codePath);
            string href = codeNode.Attributes["href"].Value;

            string code = string.Empty;
            if (href != null && href.IndexOf("mt=") != -1)
            {
                href = href.Replace(";", "&");
                code = href.Substring(href.IndexOf("mt=") + 3, 4);
            }
            TencentCategoryEntity category = new TencentCategoryEntity()
            {
                Id = _Count++,
                State = 1,
                CategoryLevel = 1,
                Code = code,
                ParentCode = parentCode
            };
            category.Name = name.InnerText;
            category.Url = href;
            categoryList.Add(category);
            if (name.InnerText != "全部")
            {
                categoryList.AddRange(this.Second($"{Constant.TencentClassUrl}{href}&tuin=7e4f8b7d", code));
            }
            return categoryList;
        }

        /// <summary>
        /// 在一个一级类下面的全部二级类进行查找
        /// </summary>
        /// <param name="html"></param>
        /// <param name="parentCode"></param>
        /// <returns></returns>
        private List<TencentCategoryEntity> Second(string url, string parentCode)
        {
            string html = HttpHelper.DownloadUrl(url);
            List<TencentCategoryEntity> categoryList = new List<TencentCategoryEntity>();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            string path = "//*[@id='auto-test-1']/div[1]/dl/dd";
            HtmlNodeCollection nodeList = doc.DocumentNode.SelectNodes(path);

            foreach (HtmlNode node in nodeList)
            {
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(node.InnerHtml);

                string codePath = "//a";
                HtmlNode codeNode = htmlDocument.DocumentNode.SelectSingleNode(codePath);
                string href = codeNode.Attributes["href"].Value;
                if (!string.IsNullOrWhiteSpace(href))
                {
                    href = href.Replace(";", "&");
                }

                string code = string.Empty;
                if (href != null && href.IndexOf("st=") != -1)
                {
                    href = href.Replace(";", "&");
                    code = href.Substring(href.IndexOf("st=") + 3, 4);
                }
                TencentCategoryEntity category = new TencentCategoryEntity()
                {
                    Id = _Count++,
                    State = 1,
                    CategoryLevel = 2,
                    Code = code,
                    ParentCode = parentCode
                };
                category.Name = codeNode.InnerText;
                category.Url = href;

                categoryList.Add(category);

                if (codeNode.InnerText != "全部")
                {
                    categoryList.AddRange(this.Third($"{Constant.TencentClassUrl}{href}&tuin=7e4f8b7d", code));
                }
            }
            return categoryList;
        }

        /// <summary>
        /// 在一个二级类下的全部三级类里面进行查找
        /// </summary>
        /// <param name="html"></param>
        /// <param name="parentCode"></param>
        /// <returns></returns>
        private List<TencentCategoryEntity> Third(string url, string parentCode)
        {
            string html = HttpHelper.DownloadUrl(url);
            List<TencentCategoryEntity> categoryList = new List<TencentCategoryEntity>();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            string path = "//*[@id='auto-test-1']/div[1]/dl/dd";
            HtmlNodeCollection nodeList = doc.DocumentNode.SelectNodes(path);
            if (nodeList == null)
            {

            }
            foreach (HtmlNode node in nodeList)
            {
                HtmlDocument htmlDocument = new HtmlDocument();

                htmlDocument.LoadHtml(node.InnerHtml);

                string codePath = "//a";
                HtmlNode codeNode = htmlDocument.DocumentNode.SelectSingleNode(codePath);
                string href = codeNode.Attributes["href"].Value;

                string code = string.Empty;
                if (href != null)
                {
                    href = href.Replace(";", "&");
                }
                if (href != null && href.IndexOf("tt=") != -1)
                {
                    href = href.Replace(";", "&");
                    code = href.Substring(href.IndexOf("tt=") + 3, 4);
                }
                TencentCategoryEntity category = new TencentCategoryEntity()
                {
                    Id = _Count++,
                    State = 1,
                    CategoryLevel = 3,
                    Code = code,
                    ParentCode = parentCode
                };
                category.Name = codeNode.InnerText;
                category.Url = href;
                categoryList.Add(category);
            }
            return categoryList;
        }
    }
}
