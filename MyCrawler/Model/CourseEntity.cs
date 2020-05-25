using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCrawler.Model
{
    public class CourseEntity : BaseModel
    {
        /// <summary>
        /// 作者姓名
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// 头像图片网络地址
        /// </summary>
        public string HeadImgUrlWeb { get; set; }
        /// <summary>
        /// 头像图片本地地址
        /// </summary>
        public string HeadImgUrlDisk { get; set; }
        /// <summary>
        /// 作者性别
        /// </summary>
        public int Gender { get; set; }
        /// <summary>
        /// 作者年龄
        /// </summary>
        public int Age { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 点赞数
        /// </summary>
        public int UpCount { get; set; }
        /// <summary>
        /// 评论数
        /// </summary>
        public int CommentCount { get; set; }
        /// <summary>
        /// 0笑话 1图片 2视频
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 配文图片
        /// </summary>
        public string ContentImg { get; set; }
        /// <summary>
        /// 配文视频
        /// </summary>
        public string ContentVideo { get; set; }
    }
}
