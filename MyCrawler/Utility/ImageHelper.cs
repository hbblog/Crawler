using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyCrawler.Utility
{
    public class ImageHelper
    { 
        public static string ImgSave(string url)
        { 
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new Exception("图片下载路径不能为空");
            }
            string path = "";

            HttpWebRequest imgRequest = WebRequest.Create(url) as HttpWebRequest;
            HttpWebResponse res;
            try
            {
                res = (HttpWebResponse)imgRequest.GetResponse();
            }
            catch (WebException ex)
            {
                res = (HttpWebResponse)ex.Response;
            }
            if (res.StatusCode == HttpStatusCode.OK)
            {
                System.Drawing.Image downImage = System.Drawing.Image.FromStream(res.GetResponseStream());

                string deerory = Constant.ImagePath;
                string fileName = $"{DateTime.Now.ToString("HHmmssffff")}.jpg";

                if (!System.IO.Directory.Exists(deerory))
                {
                    System.IO.Directory.CreateDirectory(deerory);
                }
                downImage.Save(deerory + fileName);
                downImage.Dispose();
                path = deerory + fileName;
            }
            return path;
        }


        public static void DeleteDir(string file)
        { 
            try
            { 
                //判断文件夹是否还存在
                if (Directory.Exists(file))
                {
                    DirectoryInfo fileInfo = new DirectoryInfo(file);
                    fileInfo.Attributes = FileAttributes.Normal & FileAttributes.Directory;
                    File.SetAttributes(file, System.IO.FileAttributes.Normal);

                    foreach (string f in Directory.GetFileSystemEntries(file))
                    { 
                        if (File.Exists(f))
                        {
                            //如果有子文件删除文件
                            File.Delete(f);
                            Console.WriteLine(f);
                        }
                        else
                        {
                            //循环递归删除子文件夹
                            DeleteDir(f);
                        } 
                    } 
                    //删除空文件夹 
                    Directory.Delete(file); 
                } 
            }
            catch (Exception ex) // 异常处理
            {
                Console.WriteLine(ex.Message.ToString());// 异常信息
            }

        }
    }
}
