using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeChatTest.Models
{
    public class ResultJson
    {
        /*
          
         */
        public int id { get; set; }
        public string userName { get; set; }
        public string headUrl { get; set; }
        public bool isFocus { get; set; }
        public string newsTitle { get; set; }
        public bool isVideo { get; set; }
        public string videoUrl { get; set; }
        public string newsText { get; set; }
        public string newAbstrack { get; set; }
        public string imageUrl { get; set; }
        public bool isOriginal { get; set; }
        public string createTime { get; set; }
    }
}