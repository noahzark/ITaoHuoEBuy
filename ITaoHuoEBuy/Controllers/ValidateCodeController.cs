using System.Web.Mvc;
using EBuy.Models;

namespace EBuy.Controllers
{

    /// <summary>
    /// 验证码控制器
    /// </summary>
    public class ValidateCodeController : Controller
    {
        //
        // GET: /ValidateCode/
        /// <summary>
        /// 生成验证码图片并将验证码保存在session中
        /// </summary>
        /// <returns></returns>
        public ActionResult GetValidateCode()
        {
            //生成4位验证码字符串
            string code = ValidateCodeModel.CreateValidateCode(4);
            //把验证码存入Session
            Session["ValidateCode"] = code;
            //创建验证码图片
            byte[] bytes = ValidateCodeModel.CreateGraphic(code);
            //把验证码图片发给浏览器
            return File(bytes, @"image/jpeg");
        }

    }
}
