using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using EBuy.Filters;
using EBuy.Models;
using EBuy.Repository;

namespace EBuy.Controllers
{

    [Authorize]
    [InitializeSimpleMembership]
    public class CartController : Controller
    {
        private EBuyContext db = new EBuyContext();

        //
        // GET: /Cart/Show
        public ActionResult Show()
        {
            SortedSet<GoodsInCartModel> Set = new SortedSet<GoodsInCartModel>();
            HttpCookie oCookie = Request.Cookies["ShoppingCart"];
            oCookie.Expires = DateTime.Now.AddYears(3);
            string ShoppingCartStr = oCookie.Value.ToString();
            string[] arrCookie = ShoppingCartStr.Split(new char[] { ',' });
            //查看cookie中是否有该产品
            for (int i = 0; i < arrCookie.Length; i++)
            {
                int id = Convert.ToInt32(arrCookie[i].Trim().Remove(arrCookie[i].IndexOf(':')));//获得Id

                string name = db.Goods.Find(id).GoodsName;

                string quantityStr = arrCookie[i].Trim().Substring(arrCookie[i].Trim().IndexOf(':') + 1);//得到数量
                int quantity = Convert.ToInt32(quantityStr);

                GoodsInCartModel good = new GoodsInCartModel(id, name, quantity);

                Set.Add(good);
            }

            return View(Set);
        }

        //
        // POST: /Good/AddToCart/6
        [HttpPost]
        public ActionResult AddToCart(int id)
        {
            int Quantity = 1;
            if (Request.Cookies["ShoppingCart"] == null)
            {
                //如果Cookie中没有则新建一个购物车
                HttpCookie oCookie = new HttpCookie("ShoppingCart");
                oCookie.Expires = DateTime.Now.AddYears(3);
                oCookie.Value = id.ToString() + ":" + Quantity;
                Response.Cookies.Add(oCookie);
            }
            else
            {
                bool bExists = false;
                HttpCookie oCookie = Request.Cookies["ShoppingCart"];
                oCookie.Expires = DateTime.Now.AddYears(3);
                string ShoppingCartStr = oCookie.Value.ToString();
                string[] arrCookie = ShoppingCartStr.Split(new char[] { ',' });
                //查看cookie中是否有该产品
                string newCookie = "";
                for (int i = 0; i < arrCookie.Length; i++)
                {
                    if (arrCookie[i].Trim().Remove(arrCookie[i].IndexOf(':')) == id.ToString().Trim())
                    {
                        bExists = true;
                        string OldQuantity = arrCookie[i].Trim().Substring(arrCookie[i].Trim().IndexOf(':') + 1);//得到数量
                        OldQuantity = (Convert.ToInt32(OldQuantity) + Quantity).ToString();
                        arrCookie[i] = arrCookie[i].Trim().Remove(arrCookie[i].IndexOf(':')) + ":" + OldQuantity;
                    }
                    newCookie = newCookie + "," + arrCookie[i];
                }
                //如果没有该产品
                if (!bExists)
                {
                    oCookie.Value = oCookie.Value + "," + id.ToString() + ":" + Quantity.ToString();
                }
                else
                {
                    oCookie.Value = newCookie.Substring(1);
                }
                Response.Cookies.Add(oCookie);
            }
            return RedirectToAction("Show");
        }

        //
        // POST: /Cart/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                string nowQuantity = collection.Get("item.Quantity");
                HttpCookie oCookie = Request.Cookies["ShoppingCart"];
                if (oCookie == null)
                {
                    oCookie.Expires = DateTime.Now.AddYears(3);
                    string ShoppingCartStr = oCookie.Value.ToString();
                    string[] arrCookie = ShoppingCartStr.Split(new char[] { ',' });
                    //如果Cookie中有商品则删除
                    string newCookie = "";
                    for (int i = 0; i < arrCookie.Length; i++)
                    {
                        string goodsId = arrCookie[i].Trim().Remove(arrCookie[i].IndexOf(':'));
                        if (goodsId == id.ToString().Trim())
                            newCookie = newCookie + "," + goodsId + ":" + nowQuantity;
                        else
                            newCookie = newCookie + "," + arrCookie[i];
                    }
                    Response.Cookies.Add(oCookie);
                }

                return RedirectToAction("Show");
            }
            catch
            {
                return RedirectToAction("Show");
            }
        }

        //
        // POST: /Cart/Delete/5

        [HttpPost]
        public ActionResult Delete(int id)
        {
            HttpCookie oCookie = Request.Cookies["ShoppingCart"];
            if (oCookie == null)
            {
                oCookie.Expires = DateTime.Now.AddYears(3);
                string ShoppingCartStr = oCookie.Value.ToString();
                string[] arrCookie = ShoppingCartStr.Split(new char[] { ',' });
                //如果Cookie中有商品则删除
                string newCookie = "";
                for (int i = 0; i < arrCookie.Length; i++)
                {
                    if (arrCookie[i].Trim().Remove(arrCookie[i].IndexOf(':')) == id.ToString().Trim())
                        continue;
                    newCookie = newCookie + "," + arrCookie[i];
                }
                Response.Cookies.Add(oCookie);
            }

            return RedirectToAction("Show");
        }
    }
}
