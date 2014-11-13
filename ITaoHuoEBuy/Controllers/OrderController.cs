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
        // GET: /Order/Create

        public ActionResult Create()
        {
            return View();
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
                int id = Convert.ToInt32(arrCookie[i].Trim().Remove(arrCookie[i].IndexOf(':')));//获得Id

                GoodModel goodModel = db.Goods.Find(id);

                string quantityStr = arrCookie[i].Trim().Substring(arrCookie[i].Trim().IndexOf(':') + 1);//得到数量
                int quantity = Convert.ToInt32(quantityStr);

                OrderModel ordermodel = new OrderModel()
                {
                    CustomId = WebSecurity.CurrentUserId,
                    OrderDate = DateTime.Now,
                    GoodShortcut = GoodModel.ToByteArray(goodModel),
                    GoodsAmount = quantity,
                };

                goodModel.GoodsSold += quantity;

                db.Entry(goodModel).State = EntityState.Modified;

                db.Order.Add(ordermodel);
            }

            db.SaveChanges();

            return View("Create");
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

        //
        // GET: /Order/Delete/5

        public ActionResult Delete(int id = 0)
        {
            OrderModel ordermodel = db.Order.Find(id);
            if (ordermodel == null)
            {
                return HttpNotFound();
            }
            return View(ordermodel);
        }

        //
        // POST: /Order/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            OrderModel ordermodel = db.Order.Find(id);
            db.Order.Remove(ordermodel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}