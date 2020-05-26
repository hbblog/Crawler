using Service;
using System.Collections.Generic;
using System.Web.Mvc;
using WeChatTest.Models;

namespace WeChatTest.Controllers
{
    public class QBHappyController : Controller
    {
        // GET: QBHappy
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">0笑话   1趣图     2视频</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public JsonResult Index(int type, int pageIndex)
        {
            int pageSize = 10;
            List<VM_QiuBaiHappy> list = SqlHelper.QueryList<VM_QiuBaiHappy>($"SELECT * FROM (SELECT ROW_NUMBER() over (order by id desc) rownumber,* FROM QiuBaiHappy WHERE Type = {type}) a WHERE rownumber BETWEEN {pageIndex} AND {pageSize}");
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}