using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EBuy.Models;

namespace EBuy.Controllers
{
    /// <summary>
    /// 提示页面
    /// </summary>
    public class PromptController : Controller
    {
        //
        // GET: /Notice/
        //显示系统提示信息
        public ActionResult Notice(Notice notice)
        {
            return View(notice);
        }

        //显示系统错误
        public ActionResult Error(Error error)
        {
            error.Details = Server.UrlDecode(error.Details);
            error.Cause = Server.UrlDecode(error.Cause);
            error.Solution = Server.UrlDecode(error.Solution);
            return View(error);
        }
        
    }
}
