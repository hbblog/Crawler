using MyCrawler.Model;
using MyCrawler.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MyCrawler.DataService
{
    public class CourseRepository //: IRepository<Commodity>
    {
        private Logger logger = new Logger(typeof(CourseRepository));

        public void SaveList(List<CourseEntity> commodityList)//不分表
        {
            if (commodityList == null || commodityList.Count == 0) return;

            SqlHelper.InsertList(commodityList, "QiuBaiHappy");

            Console.WriteLine($"保存完毕，共{commodityList.Count}条数据");
        }


        //public void SaveList(List<CourseEntity> commodityList)   //分表之后， 以相对均匀的数据量存储到 不同的数据表中  这是为了提高数据库的性能  其实是降低每一张表的  数据操作压力  如果爬虫爬取的  是亿万级数据，这种分表的方式是有必要的  
        //{
        //    if (commodityList == null || commodityList.Count == 0) return;
        //    IEnumerable<IGrouping<string, CourseEntity>> group = commodityList.GroupBy<CourseEntity, string>(c => GetTableName(c));

        //    foreach (var data in group)
        //    {
        //        SqlHelper.InsertList<CourseEntity>(data.ToList(), data.Key);
        //    }
        //}

        //private string GetTableName(CourseEntity commodity)
        //{
        //    return string.Format("Tencent_Subject_{0}", (commodity.CourseId % 30 + 1).ToString("000"));
        //}

        /// <summary>
        /// 保存文本记录
        /// </summary>
        /// <param name="commodityList"></param>
        /// <param name="category"></param>
        /// <param name="page"></param>
        public void SaveList(List<CourseEntity> commodityList, TencentCategoryEntity category, int page)
        {
            StreamWriter sw = null;
            try
            {
                string recordFileName = string.Format($"{category.CategoryLevel}/{category.ParentCode}/{category.Id}/{page}.txt");
                string totolPath = Path.Combine(Constant.DataPath, recordFileName);
                if (!Directory.Exists(Path.GetDirectoryName(totolPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(totolPath));
                    sw = File.CreateText(totolPath);
                }
                else
                {
                    sw = File.AppendText(totolPath);
                }
                sw.WriteLine(JsonConvert.SerializeObject(commodityList));
            }
            catch (Exception e)
            {
                logger.Error("CommodityRepository.SaveList出现异常", e);
            }
            finally
            {
                if (sw != null)
                {
                    sw.Flush();
                    sw.Close();
                    sw.Dispose();
                }
            }
        }
    }
}
