using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using WebMatrix.WebData;
using EBuy.Filters;

namespace EBuy.Controllers
{

    [Authorize]
    [InitializeSimpleMembership]
    public class UploadController : Controller
    {
        //
        // GET: /Upload/
        //上传页面
        public ActionResult Index()
        {
            HttpPostedFileBase file = Request.Files["FileUpload"];
            //如果图片上传成功那么就保存到服务器硬盘里并跳转到填写商品信息页面
            if (file != null)
            {
                //解析文件类型
                string fileType = file.FileName.Substring(file.FileName.LastIndexOf('.'));
                //生成服务器中的图片名称
                string fileName = "uid" + WebSecurity.CurrentUserId + "_time" + DateTime.Now.ToFileTime() + fileType;
                //生成服务中的图片存放路径
                string saveDirectory = Server.MapPath("~/Images/Upload");
                string filePath = Path.Combine(
                    saveDirectory,
                    Path.GetFileName(fileName));
                //如果服务存放图片的文件夹不存在则新建一个
                if (!System.IO.Directory.Exists(saveDirectory))
                    System.IO.Directory.CreateDirectory(saveDirectory);
                //保存图片
                file.SaveAs(filePath);
                //把文件名存入Cookie
                Session.Add("picName",fileName);
                return RedirectToAction("Create","Good");
            }

            //如果没有上传文件/上传失败则跳回到上传页面
            ViewBag.Title = "第一步";
            ViewBag.Message = "上传商品图片";
            return View();
        }

    }
}
