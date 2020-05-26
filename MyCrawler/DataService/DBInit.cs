using MyCrawler.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

 

namespace MyCrawler.DataService
{
    /// <summary>
    /// 数据库结构初始化
    /// 改进下：直接判断表是否存在，而不是等着异常
    /// </summary>
    public class DBInit
    {
        private static Logger logger = new Logger(typeof(DBInit));

        /// <summary>
        /// 谨慎使用  会全部删除数据库并重新创建！
        /// </summary>
        public static void InitTencentSubjectTable()
        {
            #region Delete
            try
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 1; i < 31; i++)
                {
                    sb.AppendFormat("DROP TABLE [dbo].[Tencent_Subject_{0}];", i.ToString("000"));
                }
                SqlHelper.ExecuteNonQuery(sb.ToString());
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("因为它不存在，或者您没有所需的权限。"))
                {
                    logger.Warn("初始化数据库InitTencent_SubjectTable删除的时候，原表不存在");
                }
                else
                {
                    logger.Error("初始化数据库InitTencent_SubjectTable失败" + ex.Message);
                    throw ex;
                }
            }
            #endregion Delete

            #region Create
            try
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 1; i < 31; i++)
                {
                    sb.AppendFormat(@"CREATE TABLE [dbo].[Tencent_Subject_{0}](
	                                    [Id] [int] IDENTITY(1,1) NOT NULL,
	                                    [CourseId] [bigint] NULL,
	                                    [CategoryId] [int] NULL,
	                                    [Title] [nvarchar](500) NULL,
	                                    [Price] [decimal](18, 2) NULL,
	                                    [Url] [varchar](1000) NULL,
	                                    [ImageUrl] [varchar](1000) NULL,
                             CONSTRAINT [PK_Tencent_Subject_{0}] PRIMARY KEY CLUSTERED 
                            (
                            	[Id] ASC
                            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                            ) ON [PRIMARY];", i.ToString("000"));
                }
                SqlHelper.ExecuteNonQuery(sb.ToString());
            }
            catch (Exception ex)
            {
                logger.Error("InitInitTencent_SubjectTable创建异常" + ex.Message);
                throw ex;
            }
            #endregion Create
        }

        /// <summary>
        /// 谨慎使用  会全部删除数据库并重新创建！
        /// </summary>
        public static void InitCategoryTable()
        {
            #region Delete
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("DROP TABLE [dbo].[Tencent_Category];");
                SqlHelper.ExecuteNonQuery(sb.ToString());
            }
            catch (Exception ex)
            {
                if (ex.Message.Equals("无法对 表 'dbo.Tencent_Category' 执行 删除，因为它不存在，或者您没有所需的权限。"))
                {
                    logger.Warn("初始化数据库InitCategoryTable删除的时候，原表不存在");
                }
                else
                {
                    logger.Error("初始化数据库InitTencent_CategoryTable失败" + ex.Message);
                    throw ex;
                }
            }
            #endregion Delete

            #region Create
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat(@"CREATE TABLE [dbo].[Tencent_Category](
	                                    [Id] [int] IDENTITY(1,1) NOT NULL,
	                                    [Code] [varchar](100) NULL,
	                                    [ParentCode] [varchar](100) NULL,
	                                    [CategoryLevel] [int] NULL,
	                                    [Name] [nvarchar](50) NULL,
	                                    [Url] [varchar](1000) NULL,
	                                    [State] [int] NULL,
                                      CONSTRAINT [PK_Tencent_Category] PRIMARY KEY CLUSTERED 
                                     (
                                     	[Id] ASC
                                     )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                     ) ON [PRIMARY];");

                SqlHelper.ExecuteNonQuery(sb.ToString());
            }
            catch (Exception ex)
            {
                logger.Error("初始化数据库InitTencent_Category Table 创建失败" + ex.Message);
                throw ex;
            }
            #endregion Create

        }
    }
}
