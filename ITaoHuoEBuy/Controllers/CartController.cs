﻿using System;
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

        /// <summary>
        /// 查看当前访问的浏览器中Cookie是否有效
        /// </summary>
        /// <param name="oCookie">浏览器发送过来的Cookie</param>
        /// <returns>有效则返回Ture,无效返回False</returns>
        private bool checkCookie(HttpCookie oCookie)
        {
            if (oCookie == null)
                return false;
            return true;
        }

        /// <summary>
        /// 检查Cookie中存储的购物车信息字符串是否合法
        /// 即是否符合 商品1id:商品1数量,商品2id:商品2数量的格式
        /// </summary>
        /// <param name="goodStr">购物车字符串</param>
        /// <returns>合法返回True，否则返回False</returns>
        public static bool checkGoodInCookie(String goodStr)
        {
            if (String.IsNullOrEmpty(goodStr))
                return false;

            if (!goodStr.Contains(":"))
                return false;

            if (goodStr.Length < goodStr.LastIndexOf(":") + 1)
                return false;

            return true;
        }

        /// <summary>
        /// 获取购物车中商品字符串的商品Id
        /// </summary>
        /// <param name="cartStr">商品字符串</param>
        /// <returns>成功则返回转换后的商品ID，否则返回-1</returns>
        public static int getGoodsId(String cartStr)
        {
            if (checkGoodInCookie(cartStr))
                return Convert.ToInt32(cartStr.Trim().Remove(cartStr.IndexOf(':')));
            return -1;
        }

        //
        // GET: /Cart/Show
        public ActionResult Show()
        {
            SortedSet<GoodsInCartModel> Set = new SortedSet<GoodsInCartModel>();
            HttpCookie oCookie = Request.Cookies["ShoppingCart"];
            if (!checkCookie(oCookie))
            {
                oCookie = new HttpCookie("ShoppingCart");
                oCookie.Expires = DateTime.Now.AddYears(3);
                oCookie.Value = "";
                Response.Cookies.Add(oCookie);

                return View(Set);
            }

            oCookie.Expires = DateTime.Now.AddYears(3);
            string ShoppingCartStr = oCookie.Value.ToString();
            string[] arrCookie = ShoppingCartStr.Split(new char[] { ',' });
            //查看cookie中是否有该产品

            //TODO: Handle cookie error
            for (int i = 0; i < arrCookie.Length; i++)
            {
                if (!checkGoodInCookie(arrCookie[i]))
                    continue;

                int id = getGoodsId(arrCookie[i]);//获得Id

                GoodModel good = db.Goods.Find(id);

                string name = (good != null) ? good.GoodsName : "错误的商品id";

                string quantityStr = arrCookie[i].Trim().Substring(arrCookie[i].Trim().IndexOf(':') + 1);//得到数量
                int quantity = Convert.ToInt32(quantityStr);

                GoodsInCartModel goodInCart = new GoodsInCartModel(id, name, quantity);

                Set.Add(goodInCart);
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
                    int goodsId = getGoodsId(arrCookie[i]);
                    if (goodsId == id)
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
                if (oCookie != null)
                {
                    oCookie.Expires = DateTime.Now.AddYears(3);
                    string ShoppingCartStr = oCookie.Value.ToString();
                    string[] arrCookie = ShoppingCartStr.Split(new char[] { ',' });
                    //如果Cookie中有商品则删除
                    string newCookie = "";
                    for (int i = 0; i < arrCookie.Length; i++)
                    {
                        int goodsId = getGoodsId(arrCookie[i]);

                        if (collection.Get("Remove") != null && goodsId == id)
                            continue;

                        if (goodsId == id)
                            newCookie += "," + goodsId + ":" + nowQuantity;
                        else
                            newCookie += "," + arrCookie[i];
                    }
                    oCookie.Value = newCookie.Substring(1);;
                    Response.Cookies.Add(oCookie);
                }

                return RedirectToAction("Show");
            }
            catch
            {
                return RedirectToAction("Show");
            }
        }
    }

}