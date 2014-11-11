using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EBuy.Controllers
{
    public class ImageController : Controller
    {
        //用于显示商品图片的Controller
        public string GoodsImage(string picName)
        {
            var path = Url.Action("sample.gif", "images");
            if (!String.IsNullOrEmpty(picName))
                if (System.IO.File.Exists(Server.MapPath("~/Images/Upload/"+picName)))
                    path = Url.Action(picName, "images/upload");
            return path;
        }

    }
}
