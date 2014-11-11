using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System;

namespace EBuy.Models
{
    /// <summary>
    /// 用户表单模型
    /// </summary>
    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }
    }

    /// <summary>
    /// 修改密码模型
    /// </summary>
    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "当前密码")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessageResourceType = typeof(Resources.Language),
            ErrorMessageResourceName = "ErrorPasswordLength", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "新密码")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认新密码")]
        [Compare("NewPassword", ErrorMessage = "新密码和确认密码不匹配。")]
        public string ConfirmPassword { get; set; }
    }

    /// <summary>
    /// 登录模型
    /// </summary>
    public class LoginModel
    {
        [Required]
        [Display(Name = "StrUserName",
            ResourceType = typeof(Resources.Language))]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "StrPassWord",
            ResourceType = typeof(Resources.Language))]
        public string Password { get; set; }

        [Required]
        [Display(Name = "StrValidateCode",
            ResourceType = typeof(Resources.Language))]
        public string ValidateCode { get; set; }

        [Display(Name = "CheckRemember",
            ResourceType = typeof(Resources.Language))]
        public bool RememberMe { get; set; }
    }

    /// <summary>
    /// 注册模型
    /// </summary>
    public class RegisterModel
    {
        [Required]
        [Display(Name = "StrUserName",
            ResourceType = typeof(Resources.Language))]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessageResourceType = typeof(Resources.Language),
            ErrorMessageResourceName = "ErrorPasswordLength", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "StrPassWord",
            ResourceType = typeof(Resources.Language))]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "StrConfirmPassWord",
            ResourceType = typeof(Resources.Language))]
        [Compare("Password", ErrorMessage = "密码和确认密码不匹配。")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "StrValidateCode",
            ResourceType = typeof(Resources.Language))]
        public string ValidateCode { get; set; }
    }

}
