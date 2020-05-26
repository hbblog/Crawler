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
    public class WarnRepository //: IRepository<Commodity>
    {
        private Logger logger = new Logger(typeof(WarnRepository));
        public void SaveWarn(TencentCategoryEntity category, string msg)
        {
            StreamWriter sw = null;
            try
            {
                string recordFileName = string.Format("warn/{0}/{1}/{2}.txt", category.CategoryLevel, category.ParentCode, category.Id);
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
                sw.WriteLine(msg);
                sw.WriteLine(JsonConvert.SerializeObject(JsonConvert.SerializeObject(category)));
            }
            catch (Exception ex)
            {
                logger.Error("SaveWarn出现异常" + ex.Message);
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
