using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace EBuy.Models
{
    /// <summary>
    /// 商品数据模型
    /// </summary>
    [Table("Goods")]//存在数据库的Goods表里
    [Serializable]//可序列化的-表示该类的对象可以转换成一个字节数组
    public class GoodModel
    {
        //空的GoodModel的构造函数，先创建一个空的GoodModel对象，之后再由Controller设置其属性
        public GoodModel()
        { }

        //带参数的GoodModel构造函数，创建GoodModel的同时记录一些Controller传入的数据（即用户填写在newGood里的数据）
        public GoodModel( int userId, string picName, NewGoodsModel newGood)
        {
            UserId = userId; //给商品模型加上发布者的UserId
            ReleaseTime = DateTime.Now; //给商品模型加上系统当前时间（发布时间）
            UpdateTime = DateTime.Now;
            GoodsIcon = picName; //给商品模型加上已上传的商品图标名

            //把用户填写在NewGoodsModel里的把信息保存下来
            GoodsName = newGood.GoodsName;
            GoodsPrice = newGood.GoodsPrice;
            GoodsAmount = newGood.GoodsAmount;
            GoodsSummary = newGood.GoodsSummary;
            GoodsDetail = newGood.GoodsDetail;

            //初始化商品数量信息
            GoodsAdded = 0;
            GoodsReduced = 0;
            GoodsSold = 0;
            GoodsReturned = 0;
        }

        [Key]//主键，数据库索引ID，类似于身份证号，学号
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]//该属性由数据库自动生成
        public int GoodsId { get; set; }//让系统自动生成

        [Required]
        [Display(Name = "发布者ID")]//以后显示在View中该属性的标题
        public int UserId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "上架日期")]//在网页上显示的标题为：上架日期
        public DateTime ReleaseTime { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "最近更新日期")]
        public DateTime UpdateTime { get; set; }

        [Required]
        [Display(Name = "商品图片")]
        public string GoodsIcon { get; set; }

        [Required]
        //{0} - 自己的名字，这里为"商品名称"
        //{1} - 第一个参数的值，这里为 100
        //{2} - 第二个参数的值，这里为 4
        //所以最终显示出来的错误提示信息就是“商品名称必须至少包含4个字符，且不能超过100个字符。”
        [StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符，且不能超过 {1} 个字符。", MinimumLength = 4)]
        [Display(Name = "商品名称")]
        public string GoodsName { get; set; }

        [Required]
        [Range(1.00, 10000.00)]
        [Display(Name = "商品价格")]
        public decimal GoodsPrice { get; set; }

        [StringLength(255, ErrorMessage = "{0} 必须不能超过 {1} 个字符。")]
        [Display(Name = "商品简介")]
        public string GoodsSummary { get; set; }

        [Required]
        [StringLength(1023, ErrorMessage = "{0} 必须不能超过 {1} 个字符。")]
        [Display(Name = "商品描述")]
        public string GoodsDetail { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        [Display(Name = "商品初始数量")]
        public int GoodsAmount { get; set; } //一开始新增商品时填入的商品数

        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "补充商品数量")]
        public int GoodsAdded { get; set; } //编辑商品时，增加的商品数量

        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "下架商品数量")]
        public int GoodsReduced { get; set; } //编辑商品时，减少的商品数量

        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "售出商品数量")]
        public int GoodsSold { get; set; } //在网站上被购买的数量

        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "销售退货商品数量")]
        public int GoodsReturned { get; set; } //在网站上售出后被退回的商品数量

        /// <summary>
        /// 计算目前的商品库存数量
        /// </summary>
        public int NowGoodsAmount()
        {
            return GoodsAmount
                + GoodsAdded - GoodsReduced
                - GoodsSold + GoodsReturned;
        }

        /// <summary>
        /// 计算商品的实际销售量
        /// </summary>
        public int RealSaleAmount()
        {
            return GoodsSold - GoodsReturned;
        }

        /// <summary>
        /// 将商品模型序列化成二进制数据以便存入数据库中
        /// </summary>
        /// <param name="goodModel">商品模型</param>
        /// <returns>序列化后的商品模型</returns>
        public static byte[] ToByteArray(GoodModel goodModel)
        {
            BinaryFormatter b = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            b.Serialize(stream, goodModel);
            return stream.ToArray();
        }

        /// <summary>
        /// 将数据库中存储的二进制数据转换成类的对象
        /// </summary>
        /// <param name="GoodShortcut">序列化的商品模型</param>
        /// <returns>实际商品模型</returns>
        public static GoodModel FromByteArray(byte[] GoodShortcut)
        {
            MemoryStream stream = new MemoryStream(GoodShortcut);
            BinaryFormatter b = new BinaryFormatter();
            GoodModel goods = b.Deserialize(stream) as GoodModel;
            return goods;
        }
    }

    //这个是用来“临时”存放用户填写新商品数据的类
    //没有[Table("xxx")]说明他并不对应数据库中的某个表，就不是存在数据库里的
    public class NewGoodsModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符，且不能超过 {1} 个字符。", MinimumLength = 4)]
        [Display(Name = "商品名称")]
        /// 商品名称
        public string GoodsName { get; set; }

        [Required]
        [Range(1.00, 10000.00)]
        [Display(Name = "商品价格")]
        public decimal GoodsPrice { get; set; }

        [Required]
        [Range(1, 10000)]
        [Display(Name = "商品数量")]
        public int GoodsAmount { get; set; }

        [StringLength(255, ErrorMessage = "{0} 必须不能超过 {1} 个字符。")]
        [Display(Name = "商品简介")]
        public string GoodsSummary { get; set; }

        [Required]
        [StringLength(1023, ErrorMessage = "{0} 必须不能超过 {1} 个字符。")]
        [Display(Name = "商品描述")]
        public string GoodsDetail { get; set; }
    }

    public class GoodAmount
    {
        [Key]
        public int GoodsId { get; set; }

        [Required]
        [Range(-10000, 10000)]
        public int GoodsAmount { get; set; }
    }

    //这个类实现了IComparable接口，意思是GoodsInCartModel 能和GoodsInCartModel（<>尖括号里的内容）比
    //下面的CompareTo方法告诉系统该怎么比
    public class GoodsInCartModel : IComparable<GoodsInCartModel>
    {
        public GoodsInCartModel(int id, String name, int quantity)
        {
            GoodsId = id;
            Name = name;
            Quantity = quantity;
        }

        [Key]
        public int GoodsId { get; set; }

        [Required]
        [Display(Name = "商品名称")]
        public string Name { get; set; } //为了缓解数据库压力，把商品

        [Required]
        [Display(Name = "商品数量")]
        public int Quantity { get; set; }

        //CompareTo方法告诉系统具体怎么排序，返回值小于0排前面，大于0排后面
        //GoodsId - anotherGood.GoodsId 比如这个货物id是2，要比较的是3
        //这时返回值就小于零，所以这个货物排前面
        public int CompareTo(GoodsInCartModel anotherGood)
        {
            return GoodsId - anotherGood.GoodsId;
        }
    }

}