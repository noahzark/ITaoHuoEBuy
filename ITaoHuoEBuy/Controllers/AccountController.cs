using System;
using System.Web.Mvc;
using System.Web.Security;
using EBuy.Filters;
using EBuy.Models;
using WebMatrix.WebData;

namespace EBuy.Controllers
{
    /// <summary>
    /// 账户管理器，包含登录、注册、验证码功能
    /// </summary>
    [Authorize]//必须登录才能访问
    [InitializeSimpleMembership]//初始化系统的账户管理
    public class AccountController : Controller
    {
        //
        // GET: /Account/Login
        /// <summary>
        /// 登录页面
        /// </summary>
        /// <param name="returnUrl">登陆成功后返回的Url</param>
        [AllowAnonymous]//允许游客访问
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl; //登录成功后的返回地址
            ViewBag.Title = Resources.PageLanguage.TitleLogin; //登录页面标题
            return View();
        }

        //
        // POST: /Account/Login
        /// <summary>
        /// 登录操作
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl">登陆成功后返回的Url</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]//启用系统自带的防止伪造请求功能
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            //非正常情况下进行错误的处理，错误原因见Cause
            if (Session["ValidateCode"] == null || Session["ValidateCode"].ToString() == "")
            {
                Error _e = new Error
                {
                    Title = "验证码不存在",
                    Details = "在用户登录时，服务器端的验证码为空，或向服务器提交的验证码为空",
                    Cause = Server.UrlEncode("<li>你登录时在登录页面停留的时间过久页面已经超时</li><li>您绕开客户端验证向服务器提交数据</li>"),
                    Solution = Server.UrlEncode("返回<a href='" + Url.Action("Login", "Account") + "'>登录</a>页面，刷新后重新登录")
                };
                return RedirectToAction("Error", "Prompt", _e);
            }

            //校验验证码
            if (Session["ValidateCode"].ToString() == model.ValidateCode)
            {
                //检查登录信息
                if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
                {
                    //返回到登录前的本地页面
                    return RedirectToLocal(returnUrl);
                }
                else //登录信息检查失败提示用户名密码错误
                    ModelState.AddModelError("", "提供的用户名或密码不正确。");
            }
            else //验证码校验失败提示验证码错误
                ModelState.AddModelError("", @Resources.Language.ErrorWrongValidateCode);

            ViewBag.ReturnUrl = returnUrl; //登录成功后的返回地址
            ViewBag.Title = Resources.PageLanguage.TitleLogin; //登录页面标题
            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        //
        // POST: /Account/LogOff
        /// <summary>
        /// 注销操作
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register
        /// <summary>
        /// 注册页面
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Register()
        {
            ViewBag.Title = Resources.PageLanguage.TitleRegister;
            return View();
        }

        //
        // POST: /Account/Register
        /// <summary>
        /// 注册操作
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (Session["ValidateCode"] == null || Session["ValidateCode"].ToString() == "")
            {
                Error _e = new Error
                {
                    Title = "验证码不存在",
                    Details = "在用户登录时，服务器端的验证码为空，或向服务器提交的验证码为空",
                    Cause = Server.UrlEncode("<li>你登录时在登录页面停留的时间过久页面已经超时</li><li>您绕开客户端验证向服务器提交数据</li>"),
                    Solution = Server.UrlEncode("返回<a href='" + Url.Action("Login", "Account") + "'>登录</a>页面，刷新后重新登录")
                };
                return RedirectToAction("Error", "Prompt", _e);
            }

            if (Session["ValidateCode"].ToString() == model.ValidateCode)
            {
                if (ModelState.IsValid)
                {
                    // 尝试注册用户
                    try
                    {
                        //注册
                        WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                        //登录
                        WebSecurity.Login(model.UserName, model.Password);
                        //回主页
                        Notice _n = new Notice
                        {
                            Title = "恭喜", //提示框标题
                            Details = model.UserName + " 注册成功！", //提示框内容
                            DelayTime = 5, //提示框停留时间
                            NavigationName = "首页", //提示框返回页面标题
                            NavigationUrl = Url.Action("Index", "Home") //提示框返回的页面
                        };
                        return RedirectToAction("Notice", "Prompt", _n);
                    }
                    catch (MembershipCreateUserException e)
                    {
                        ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                    }
                }
            }
            else
                ModelState.AddModelError("", @Resources.Language.ErrorWrongValidateCode);

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        public ActionResult Manage()
        {
            ViewBag.HasLocalPassword = true;
            ViewBag.ReturnUrl = Url.Action("Manage");
            ViewBag.Title = "管理帐户";

            return View();
        }

        //
        // GET: /Account/Manage
        /// <summary>
        /// 管理界面
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public ActionResult ChangePassword(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "已更改你的密码。"
                : message == ManageMessageId.SetPasswordSuccess ? "已设置你的密码。"
                : message == ManageMessageId.RemoveLoginSuccess ? "已删除外部登录。"
                : "";
            //Local Password是给使用QQ/微博账号登录用的，暂时没有发挥作用
            //ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = true;
            ViewBag.ReturnUrl = Url.Action("Manage");
            ViewBag.Title = "修改密码";
            return View();
        }

        //
        // POST: /Account/Manage
        /// <summary>
        /// 更改密码操作
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(LocalPasswordModel model)
        {
            bool hasLocalAccount = //OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                true;
            ViewBag.HasLocalPassword = hasLocalAccount;
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // 在某些失败方案中，ChangePassword 将引发异常，而不是返回 false。
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        ModelState.AddModelError("", "当前密码不正确或新密码无效。");
                    }
                }
            }
            /*
            {
                // 用户没有本地密码，因此将删除由于缺少
                // OldPassword 字段而导致的所有验证错误
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("", e);
                    }
                }
            }*/

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        #region 帮助程序
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                //防止跨站攻击，如果returnUrl为外部网址，则跳转至本网站首页
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // 请参见 http://go.microsoft.com/fwlink/?LinkID=177550 以查看
            // 状态代码的完整列表。
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "用户名已存在。请输入其他用户名。";

                case MembershipCreateStatus.DuplicateEmail:
                    return "该电子邮件地址的用户名已存在。请输入其他电子邮件地址。";

                case MembershipCreateStatus.InvalidPassword:
                    return "提供的密码无效。请输入有效的密码值。";

                case MembershipCreateStatus.InvalidEmail:
                    return "提供的电子邮件地址无效。请检查该值并重试。";

                case MembershipCreateStatus.InvalidAnswer:
                    return "提供的密码取回答案无效。请检查该值并重试。";

                case MembershipCreateStatus.InvalidQuestion:
                    return "提供的密码取回问题无效。请检查该值并重试。";

                case MembershipCreateStatus.InvalidUserName:
                    return "提供的用户名无效。请检查该值并重试。";

                case MembershipCreateStatus.ProviderError:
                    return "身份验证提供程序返回了错误。请验证您的输入并重试。如果问题仍然存在，请与系统管理员联系。";

                case MembershipCreateStatus.UserRejected:
                    return "已取消用户创建请求。请验证您的输入并重试。如果问题仍然存在，请与系统管理员联系。";

                default:
                    return "发生未知错误。请验证您的输入并重试。如果问题仍然存在，请与系统管理员联系。";
            }
        }
        #endregion

    }
}
