using System;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EBuy.Models;
using EBuy.Repository;
using WebMatrix.WebData;

namespace EBuy.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private EBuyContext db = new EBuyContext();

        //
        // GET: /Order/

        public ActionResult Index()
        {
            return View(db.Order.ToList());
        }

        //
        // GET: /Order/Details/5

        public ActionResult Details(int id = 0)
        {
            OrderModel ordermodel = db.Order.Find(id);
            if (ordermodel == null)
            {
                return HttpNotFound();
            }
            return View(ordermodel);
        }

        //
        // POST: /Order/Create

        [HttpPost]
        public ActionResult Create()
        {
            if (Request.Cookies["ShoppingCart"] == null)
            {
                return HttpNotFound();
            }
            HttpCookie oCookie = Request.Cookies["ShoppingCart"];
            oCookie.Expires = DateTime.Now.AddYears(3);
            string ShoppingCartStr = oCookie.Value.ToString();
            string[] arrCookie = ShoppingCartStr.Split(new char[] { ',' });
            for (int i = 0; i < arrCookie.Length; i++)
            {
                int id = CartController.getGoodsId(arrCookie[i]);

                if (id < 0)
                    continue;

                GoodModel goodModel = db.Goods.Find(id);

                if (goodModel == null)
                    continue;

                string quantityStr = arrCookie[i].Trim().Substring(arrCookie[i].Trim().IndexOf(':') + 1);//得到数量
                int quantity = Convert.ToInt32(quantityStr);

                OrderModel ordermodel = new OrderModel()
                {
                    CustomId = WebSecurity.CurrentUserId,
                    OrderDate = DateTime.Now,
                    GoodShortcut = GoodModel.ToByteArray(goodModel),
                    GoodsAmount = quantity,
                    OrderStatus = OrderModel.OrderStatusId.Unpaid,
                };

                goodModel.GoodsSold += quantity;

                db.Entry(goodModel).State = EntityState.Modified;

                db.Order.Add(ordermodel);
            }

            db.SaveChanges();

            Notice _n = new Notice
            {
                Title = "恭喜", //提示框标题
                Details = "下单成功！", //提示框内容
                DelayTime = 5, //提示框停留时间
                NavigationName = "订单管理", //提示框返回页面标题
                NavigationUrl = Url.Action("Index", "Order") //提示框返回的页面
            };
            return RedirectToAction("Notice", "Prompt", _n);
        }

        //
        // GET: /Order/Edit/5

        public ActionResult Edit(int id = 0)
        {
            OrderModel ordermodel = db.Order.Find(id);
            if (ordermodel == null)
            {
                return HttpNotFound();
            }
            return View(ordermodel);
        }

        //
        // POST: /Order/Edit/5

        [HttpPost]
        public ActionResult Edit(OrderModel ordermodel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ordermodel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ordermodel);
        }

        [HttpPost]
        public ActionResult AskCancel(int id)
        {
            OrderModel ordermodel = db.Order.Find(id);

            if (WebSecurity.CurrentUserId == ordermodel.CustomId)
            {
                if (ordermodel.OrderStatus <= OrderModel.OrderStatusId.Finished)
                    ordermodel.OrderStatus = OrderModel.OrderStatusId.WaitCancel;
            }

            return RedirectToAction("Detail", ordermodel);
        }

        //
        // POST: /Order/Cancel/5
        [HttpPost]
        public ActionResult CancelConfirmed(int id)
        {
            OrderModel ordermodel = db.Order.Find(id);
            GoodModel goodmodel = GoodModel.FromByteArray(ordermodel.GoodShortcut);

            //验证当前操作用户为订单中商品的发布者
            if (WebSecurity.CurrentUserId == goodmodel.UserId)
            {
                if (ordermodel.OrderStatus == OrderModel.OrderStatusId.WaitCancel)
                    ordermodel.OrderStatus = OrderModel.OrderStatusId.Cancelled;
                db.SaveChanges();
            }
            
            return RedirectToAction("Detail", ordermodel);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}