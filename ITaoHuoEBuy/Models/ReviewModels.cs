using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EBuy.Models
{
    /// <summary>
    /// 评论数据模型
    /// </summary>
    [Table("Reviews")]
    public class ReviewModel
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int CommentId { get; set; }

        [Required]
        [Display(Name = "评论商品ID")]
        public int GoodsId { get; set; }

        [Required]
        [Display(Name = "评论人ID")]
        public int UserId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "评论日期")]
        public DateTime ReleaseTime { get; set; }

        [Required]
        [StringLength(127, ErrorMessage = "{0} 必须不能超过 {1} 个字符。")]
        [Display(Name = "评论标题")]
        public string CommentTitle { get; set; }

        [Required]
        [StringLength(1023, ErrorMessage = "{0} 必须不能超过 {1} 个字符。")]
        [Display(Name = "评论内容")]
        public string CommentDetail { get; set; }
    }

    public class NewReviewModel
    {
        [Required]
        [Display(Name = "评论商品ID")]
        public int GoodsId { get; set; }

        [Required]
        [StringLength(127, ErrorMessage = "{0} 必须不能超过 {1} 个字符。")]
        [Display(Name = "评论标题")]
        public string CommentTitle { get; set; }

        [Required]
        [StringLength(1023, ErrorMessage = "{0} 必须不能超过 {1} 个字符。")]
        [Display(Name = "评论内容")]
        public string CommentDetail { get; set; }
    }
}