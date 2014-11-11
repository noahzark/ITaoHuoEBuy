using System.Web.Mvc;

namespace EBuy.Controllers
{

    /// <summary>
    /// 主页控制器
    /// </summary>
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "主页";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Title = "联系方式";
            ViewBag.Message = "吴泽园";

            return View();
        }

    }
}
