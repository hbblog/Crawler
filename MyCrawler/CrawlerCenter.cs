
using MyCrawler.DataService;
using MyCrawler.DataServices;
using MyCrawler.Model;
using MyCrawler.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCrawler
{
    public class CrawlerCenter
    {
        private static Logger logger = new Logger(typeof(CrawlerCenter));

        /// <summary>
        /// 抓取
        /// </summary>
        public static void Handler()
        {
            Console.WriteLine("请输入Y/N进行类别表初始化确认！ Y 删除Tencent_Category表然后重新创建，然后抓取类型数据，N（或者其他）跳过");
            string input = Console.ReadLine();
            if (input.Equals("Y", StringComparison.OrdinalIgnoreCase))
            {
                DBInit.InitCategoryTable();
                CrawlerCategory();
            }
            else
            {
                Console.WriteLine("你选择不初始化类别数据");
            }
            Console.WriteLine("*****************^_^**********************");


            Console.WriteLine("请输入Y/N进行课程数据初始化确认！ Y 删除全部课程数据表然后重新创建，然后抓取课程数据，N（或者其他）跳过");
            input = Console.ReadLine();
            if (input.Equals("Y", StringComparison.OrdinalIgnoreCase))
            {
                DBInit.InitTencentSubjectTable();
                CrawlerCourse();
            }
            Console.WriteLine("*****************^_^**********************");
        }

        private static void CrawlerCategory()
        {
            Console.WriteLine($"{ DateTime.Now} 腾讯课堂类目开始抓取 - -");
            ISearch search = new CategorySearch();
            search.Crawler();
        }

        /// <summary>
        /// 抓取所有课程
        /// //所有的类目都已经获取到了，那么如果需要获取课程信息，就应该通过每一个类目去获取类目对应的课程数据信息 
        /// </summary>
        public static void CrawlerCourse()
        {
            Console.WriteLine($"{ DateTime.Now} 腾讯课堂课程开始抓取 - -");
            CategoryRepository categoryRepository = new CategoryRepository();

            //先从数据库拿到第三级类目信息
            List<TencentCategoryEntity> categoryList = categoryRepository.QueryListByLevel(3);

            var categoryquery = categoryList.Where(a => !a.Name.Contains("全部"));

            //刚刚在通过类目爬取课程信息的时候，发现效率很低，所以这里需要提升性能


            //多线程提高性能：

            TaskFactory taskFactory = Task.Factory;

            //List<Task> taskList = new List<Task>();
            //foreach (TencentCategoryEntity category in categoryquery)
            //{
            //    ISearch searcher = new CourseSearch(category);
            //    //这样写靠谱吗？ 为什么？  可能会线程过多！
            //    //所以需要控制线程数量；如何控制线程数量呢？  
            //    taskList.Add(taskFactory.StartNew(searcher.Crawler));
            //    if (taskList.Count>10)  //当前最多开启10个线程
            //    {
            //        taskList = taskList.Where(t => !t.IsCompleted && !t.IsCanceled && !t.IsFaulted).ToList();
            //        Task.WaitAny(taskList.ToArray());//表示等待一个线程完成之后继续往后 
            //    } 
            //}  
            //Task.WaitAll(taskList.ToArray()); 

            //同学和我都觉得这样玩有点麻烦！  如何解决一下呢？

            List<Action> actions = new List<Action>();
            foreach (TencentCategoryEntity category in categoryquery)
            {
                ISearch searcher = new CourseSearch(category);
                actions.Add(new Action(searcher.Crawler));
            }
            //需要控制数量 
            ParallelOptions options = new ParallelOptions();
            options.MaxDegreeOfParallelism = 10;  //控制了最大的线程数量是10 个线程
            Parallel.Invoke(options, actions.ToArray()); //Parallel 表示有多少个委托  就开启多少个线程

            Console.WriteLine($"{ DateTime.Now} 腾讯课堂所有课程抓取完毕 - -");


            //如果这一块不理解，说明对多线程还需要再加巩固，如何巩固呢？可以联系我们课堂的助教老师获取相关学习资料（爬虫相关的内容，多线程全集的内容！）

            //今天的课程到这里就不往下延续。。。
            //觉得在本次课上，有收获的同学，可以给老师刷一个数字6 支持一下
        }

        /// <summary>
        /// 清理重复数据
        /// </summary>
        private static void CleanAll()
        {
            try
            {
                Console.WriteLine($"{ DateTime.Now} 开始清理重复数据 - -");
                StringBuilder sb = new StringBuilder();
                for (int i = 1; i < 31; i++)
                {
                    sb.AppendFormat(@"DELETE FROM [dbo].[JD_Commodity_{0}] where productid IN(select productid from [dbo].[JD_Commodity_{0}] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_{0}] group by productid,CategoryId having count(0)>1);", i.ToString("000"));
                }
                #region
                /*
                 DELETE FROM [dbo].[JD_Commodity_001] where productid IN(select productid from [dbo].[JD_Commodity_001] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_001] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_002] where productid IN(select productid from [dbo].[JD_Commodity_002] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_002] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_003] where productid IN(select productid from [dbo].[JD_Commodity_003] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_003] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_004] where productid IN(select productid from [dbo].[JD_Commodity_004] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_004] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_005] where productid IN(select productid from [dbo].[JD_Commodity_005] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_005] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_006] where productid IN(select productid from [dbo].[JD_Commodity_006] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_006] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_007] where productid IN(select productid from [dbo].[JD_Commodity_007] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as IDv from [dbo].[JD_Commodity_007] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_008] where productid IN(select productid from [dbo].[JD_Commodity_008] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_008] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_009] where productid IN(select productid from [dbo].[JD_Commodity_009] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_009] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_010] where productid IN(select productid from [dbo].[JD_Commodity_010] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_010] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_011] where productid IN(select productid from [dbo].[JD_Commodity_011] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_011] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_012] where productid IN(select productid from [dbo].[JD_Commodity_012] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_012] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_013] where productid IN(select productid from [dbo].[JD_Commodity_013] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_013] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_014] where productid IN(select productid from [dbo].[JD_Commodity_014] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_014] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_015] where productid IN(select productid from [dbo].[JD_Commodity_015] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_015] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_016] where productid IN(select productid from [dbo].[JD_Commodity_016] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_016] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_017] where productid IN(select productid from [dbo].[JD_Commodity_017] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_017] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_018] where productid IN(select productid from [dbo].[JD_Commodity_018] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_018] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_019] where productid IN(select productid from [dbo].[JD_Commodity_019] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_019] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_020] where productid IN(select productid from [dbo].[JD_Commodity_020] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_020] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_021] where productid IN(select productid from [dbo].[JD_Commodity_021] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_021] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_022] where productid IN(select productid from [dbo].[JD_Commodity_022] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_022] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_023] where productid IN(select productid from [dbo].[JD_Commodity_023] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_023] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_024] where productid IN(select productid from [dbo].[JD_Commodity_024] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_024] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_025] where productid IN(select productid from [dbo].[JD_Commodity_025] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_025] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_026] where productid IN(select productid from [dbo].[JD_Commodity_026] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_026] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_027] where productid IN(select productid from [dbo].[JD_Commodity_027] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_027] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_028] where productid IN(select productid from [dbo].[JD_Commodity_028] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_028] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_029] where productid IN(select productid from [dbo].[JD_Commodity_029] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_029] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_030] where productid IN(select productid from [dbo].[JD_Commodity_030] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_030] group by productid,CategoryId having count(0)>1);
                 */
                #endregion
                Console.WriteLine("执行清理sql:{0}", sb.ToString());
                SqlHelper.ExecuteNonQuery(sb.ToString());
                Console.WriteLine("{0} 完成清理重复数据 - -", DateTime.Now);
            }
            catch (Exception ex)
            {
                logger.Error("CleanAll出现异常", ex);
            }
            finally
            {
                Console.WriteLine("{0} 结束清理重复数据 - -", DateTime.Now);
            }
        }
    }
}
