using System.Web.Mvc;

namespace EBuy.Controllers
{
    /// <summary>
    /// 后台管理控制器
    /// </summary>
    [Authorize(Users = "admin")]//只有管理员能够访问
    public class AdminController : Controller
    {
        //
        // GET: /Admin/

        public ActionResult Index()
        {
            ViewBag.Title = "后台管理";
            return View();
        }

    }
}
