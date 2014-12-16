using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace EBuy.Models
{
    /// <summary>
    /// 订单模型
    /// </summary>
    [Table("Orders")]
    public class OrderModel
    {

        public enum OrderStatusId
        {
            Unpaid = 0,
            Paid = 1,
            Sent = 2,
            Finished = 3,
            WaitCancel = 4,
            Cancelled = 5,
        }

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Display(Name = "订单编号")]
        public int OrderId { get; set; } //订单 ID

        [Required]
        [Display(Name = "顾客 ID")]
        public int CustomId { get; set; } //购买者 ID

        [Required]
        [Display(Name = "下单时间")]
        public DateTime OrderDate { get; set; } //购买日期

        [Required]
        [Display(Name = "商品快照")]
        public byte[] GoodShortcut { get; set; } //商品快照，可以使用GoodsModel.FromByteArray反序列化获得GoodsModel对象

        [Required]
        [Display(Name = "购买数量")]
        public int GoodsAmount { get; set; }

        [Required]
        [Display(Name = "订单状态")]
        public int OrderStatus { get; set; }
    }
}