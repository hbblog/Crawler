using System;
using System.Collections.Generic;
using System.Web.Mvc;
using WeChatTest.Models;

namespace WeChatTest.Controllers
{
    public class HomeController : Controller
    {
        public JsonResult Index()
        {
            /*
             [{
      id: 1,
      userName: "HB",
      headUrl: "/pages/resource/头像.jpg",
      isFocus: true,
      newsTitle: "只争朝夕，不负韶华",
      isVideo: true,
      videoUrl: "/pages/resource/test.mp4",
      newsText: null,
      newAbstrack: null,
      imageUrl: null,
      isOriginal: true,
      createTime: "2020-04-29T09:45:15",
    }, {
      id: 2,
      userName: "HB",
      headUrl: "/pages/resource/title1.jpg",
      isFocus: false,
      newsTitle: "",
      isVideo: false,
      videoUrl: null,
      newsText: "尼日尔东下回合，欧拉函数那地方的省份许我天若有情拉看法是在考虑现代化反好",
      newAbstrack: "简介",
      imageUrl: "/pages/resource/title1.jpg",
      isOriginal: false,
      createTime: "2020-04-29T10:45:15",
    }]
             */

            List<ResultJson> list = new List<ResultJson>();
            list.Add(new ResultJson()
            {
                id = 1,
                userName = "HB",
                headUrl = "/pages/resource/头像.jpg",
                isFocus = true,
                newsTitle = "只争朝夕，不负韶华",
                isVideo = true,
                videoUrl = "/pages/resource/test.mp4",
                newsText = null,
                newAbstrack = null,
                imageUrl = null,
                isOriginal = true,
                createTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
            list.Add(new ResultJson()
            {
                id = 2,
                userName = "HB",
                headUrl = "/pages/resource/title1.jpg",
                isFocus = false,
                newsTitle = "",
                isVideo = false,
                videoUrl = null,
                newsText = "尼日尔东下回合，欧拉函数那地方的省份许我天若有情拉看法是在考虑现代化反好",
                newAbstrack = "简介",
                imageUrl = "/pages/resource/title1.jpg",
                isOriginal = false,
                createTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}