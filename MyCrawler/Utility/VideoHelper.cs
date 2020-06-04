using System;
using System.IO;
using System.Net;

namespace MyCrawler.Utility
{
    public class VideoHelper
    {
        public static string Save(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new Exception("视频下载路径不能为空");
            }
            string path = "";

            HttpWebRequest videoRequest = WebRequest.Create(url) as HttpWebRequest;
            HttpWebResponse res;
            try
            {
                res = (HttpWebResponse)videoRequest.GetResponse();
            }
            catch (WebException ex)
            {
                res = (HttpWebResponse)ex.Response;
            }

            string deerory = Constant.VideoPath;
            string fileName = $"{DateTime.Now.ToString("HHmmssffff")}.mp4";
            path = deerory + fileName;

            if (res.StatusCode == HttpStatusCode.OK)
            {
                Stream stream = res.GetResponseStream();
                // 先创建文件
                Stream sos = new System.IO.FileStream(path, System.IO.FileMode.Create);
                byte[] video = new byte[1024];
                int total = stream.Read(video, 0, video.Length);
                while (total > 0)
                {
                    //之后再输出内容
                    sos.Write(video, 0, total);
                    total = stream.Read(video, 0, video.Length);
                }
                stream.Close();
                stream.Dispose();
                sos.Close();
                sos.Dispose();
            }
            return path;
        }
    }
}
