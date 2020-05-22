using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeChatTest.Models;

namespace WeChatTest.Controllers
{
    public class QBHappyController : Controller
    {
        // GET: QBHappy
        public JsonResult Text()
        {
            List<VM_QiuBaiHappy> list = SqlHelper.QueryList<VM_QiuBaiHappy>("SELECT * FROM QiuBaiHappy");
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}