using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using EBuy.Filters;
using EBuy.Models;
using EBuy.Repository;
using WebMatrix.WebData;
using System.Collections;
using System.Collections.Generic;
using System.Web;

namespace EBuy.Controllers
{

    [Authorize]
    [InitializeSimpleMembership]
    public class GoodController : Controller
    {
        private EBuyContext db = new EBuyContext();

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult SearchPartial()
        {
            ViewBag.Title = "搜索商品";
            return PartialView("_SearchGoodsPartial");
        }
        
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Search(string goodsName)
        {
            //取出所有商品
            var goods = from good in db.Goods
                        select good;
            //搜索字符串不为空的时候
            if (!String.IsNullOrEmpty(goodsName))
            {
                //搜索指定的商品
                goods = goods.Where(s => s.GoodsName.Contains(goodsName));
            }

            //根据搜索结果改变标题
            if (goods.Count() == 0)
                ViewBag.Title = "没有符合条件的商品";
            else
                ViewBag.Title = "商品搜索结果";
            return View("Index", goods);
        }

        public ActionResult Manage()
        {
            ViewBag.Title = "商品管理";
            return View(db.Goods.ToList());
        }

        //
        // GET: /Good/
        [AllowAnonymous]
        public ActionResult Index()
        {
            var goods = db.Goods.ToList();

            if (goods.Count() == 0)
                ViewBag.Title = "暂无商品";
            else
                ViewBag.Title = "商品目录";
            return View(goods);
        }

        //
        // GET: /Good/Details/5
        // 显示商品详情
        public ActionResult Details(int id = 0)
        {
            GoodModel goodsmodel = db.Goods.Find(id);
            if (goodsmodel == null)
                return HttpNotFound();

            ViewBag.Title = "商品详情";
            ViewBag.SellerName = db.UserProfiles.Find(goodsmodel.UserId).UserName;
            ViewBag.NowAmountStr = "库存商品";
            ViewBag.NowAmount = goodsmodel.NowGoodsAmount();
            ViewBag.RealSoldStr = "已售出";
            ViewBag.RealSold = goodsmodel.RealSaleAmount();
            return View(goodsmodel);
        }

        //
        // GET: /Good/Create
        public ActionResult Create()
        {
            //从Session中取出商品图片名，好处就是上传图片后即使点到别的页面，再回来不用再次上传图片
            string picName = (string)Session["picName"];
            //如果Session中没有picName，即之前没有上传过图片，则先跳转到上传图片页面
            if (String.IsNullOrEmpty(picName))
                return RedirectToAction("Index", "Upload");
            ViewBag.PicName = picName;

            ViewBag.Title = "第二步";
            ViewBag.Message = "填写商品信息";
            return View();
        }

        //
        // POST: /Good/Create
        [HttpPost]
        public ActionResult Create(NewGoodsModel goodsmodel)
        {
            string picName = (string)Session["picName"];
            //如果Session中没有商品图片名称，即没有上传图片，那么就跳转到商品上传页面
            if (String.IsNullOrEmpty(picName))
                return RedirectToAction("", "Upload");

            if (ModelState.IsValid)
            {
                //商品模型验证通过后生成存在数据库中的商品模型
                GoodModel good = new GoodModel(
                    WebSecurity.CurrentUserId,
                    picName,
                    goodsmodel
                );

                try
                {
                    //将生成的商品对象保存到数据库中
                    db.Goods.Add(good);
                    db.SaveChanges();
                    Session.Remove("picName");

                    Notice _n = new Notice
                    {
                        Title = "恭喜", //提示框标题
                        Details = "商品 " + goodsmodel.GoodsName + " 添加成功！", //提示框内容
                        DelayTime = 5, //提示框停留时间
                        NavigationName = "商品列表", //提示框返回页面标题
                        NavigationUrl = Url.Action("Index", "Good") //提示框返回的页面
                    };
                    return RedirectToAction("Notice", "Prompt", _n);
                }
                catch (Exception e)
                {

                }                
            }

            return View(goodsmodel);
        }

        //
        // GET: /Good/Edit/5
        public ActionResult Edit(int id = 0)
        {
            GoodModel goodsmodel = db.Goods.Find(id);
            //未找到与id对应的商品时返回404没有找到
            if (goodsmodel == null)
            {
                return HttpNotFound();
            }

            ViewBag.Title = "编辑商品";
            return View(goodsmodel);
        }

        [HttpPost]
        public ActionResult Edit(GoodModel goodsmodel)
        {
            if (ModelState.IsValid)
            {
                goodsmodel.UpdateTime = DateTime.Now;
                db.Entry(goodsmodel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Manage");
            }
            return View(goodsmodel);
        }

        public ActionResult EditAmount(int id = 0)
        {
            GoodModel goodsmodel = db.Goods.Find(id);
            //未找到与id对应的商品时返回404没有找到
            if (goodsmodel == null)
            {
                return HttpNotFound();
            }

            ViewBag.Title = "修改货物数量";
            GoodAmount goodsAmount = new GoodAmount()
            {
                GoodsId = goodsmodel.GoodsId,
            };
            ViewBag.NowAmount = goodsmodel.NowGoodsAmount();
            return View(goodsAmount);
        }

        [HttpPost]
        public ActionResult EditAmount(int GoodsId, int GoodsAmount)
        {
            if (ModelState.IsValid  && GoodsAmount!=0)
            {
                GoodModel goodsmodel = db.Goods.Find(GoodsId);
                //如果根据GoodsId能找到商品，且增补货操作不会导致库存商品小于0
                if (goodsmodel != null && goodsmodel.NowGoodsAmount() + GoodsAmount >= 0)
                {
                    //更新商品修改的数量
                    if (GoodsAmount > 0)
                        goodsmodel.GoodsAdded += GoodsAmount;
                    else
                        goodsmodel.GoodsReduced += -GoodsAmount;
                    //更新商品修改时间
                    goodsmodel.UpdateTime = DateTime.Now;

                    db.Entry(goodsmodel).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Details", new { id = GoodsId });
                }
            }
            Error _e = new Error
            {
                Title = "商品修改错误",
                Details = "非法存取", //获取Exception原因
                Cause = Server.UrlEncode("<li>非法修改商品数据</li>"),
                Solution = Server.UrlEncode("请合理修改商品数据")
            };
            return RedirectToAction("Error", "Prompt", _e);
        }

        //
        // GET: /Good/Delete/5
        public ActionResult Delete(int id = 0)
        {
            GoodModel goodsmodel = db.Goods.Find(id);
            if (goodsmodel == null)
            {
                return HttpNotFound();
            }

            ViewBag.Title = "删除商品";
            return View(goodsmodel);
        }

        //
        // POST: /Good/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            GoodModel goodsmodel = db.Goods.Find(id);
            db.Goods.Remove(goodsmodel);
            //删除完商品后，也把商品图片删除掉
            try
            {
                System.IO.File.Delete(Server.MapPath("~/Images/Upload/" + goodsmodel.GoodsIcon));
            }
            catch (Exception ex)
            {
                //删除失败时处理错误
                Error _e = new Error
                {
                    Title = "商品图片删除错误",
                    Details = ex.Message, //获取Exception原因
                    Cause = Server.UrlEncode("<li>服务器文件系统错误</li>"),
                    Solution = Server.UrlEncode("请将上述错误信息提供给服务器管理员")
                };
                return RedirectToAction("Error", "Prompt", _e);
            }
            //删除图片后，保存修改到数据库
            db.SaveChanges();
            return RedirectToAction("Manage");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
