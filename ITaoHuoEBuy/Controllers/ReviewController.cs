using System;
using System.Linq;
using System.Web.Mvc;
using EBuy.Models;
using EBuy.Repository;
using WebMatrix.WebData;
using EBuy.Filters;

namespace EBuy.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class ReviewController : Controller
    {
        private EBuyContext db = new EBuyContext();

        //
        // GET: /Review/List
        [ChildActionOnly]
        public ActionResult List(int goodsId = 0)
        {
            var reviews = from review in db.Reviews
                          select review;
            if (goodsId!=0)
                reviews = reviews.Where(review => review.GoodsId.Equals(goodsId));

            if (reviews.Count() == 0)
                ViewBag.Title = "暂无评论";
            else
                ViewBag.Title = "评论列表";
            return PartialView("_ListPartial", reviews);
        }

        //
        // GET: /Review/Create
        [ChildActionOnly]
        public ActionResult Create(int goodsId)
        {
            NewReviewModel reviewmodel = new NewReviewModel { GoodsId = goodsId };
            return PartialView("_CreatePartial", reviewmodel);
        }

        //
        // POST: /Review/Create
        [Authorize]
        [HttpPost]
        public ActionResult Create(NewReviewModel reviewmodel)
        {
            if (ModelState.IsValid)
            {
                ReviewModel review = new ReviewModel
                {
                    GoodsId = reviewmodel.GoodsId,
                    UserId = WebSecurity.CurrentUserId,
                    ReleaseTime = DateTime.Now,
                    CommentTitle = reviewmodel.CommentTitle,
                    CommentDetail = reviewmodel.CommentDetail
                };
                db.Reviews.Add(review);
                db.SaveChanges();
                return RedirectToAction("Details", "Good", new { id = review.GoodsId});
            }

            return View(reviewmodel);
        }

        //
        // GET: /Review/Delete/5

        public ActionResult Delete(int id = 0)
        {
            ReviewModel reviewmodel = db.Reviews.Find(id);
            if (reviewmodel == null)
            {
                return HttpNotFound();
            }
            return View(reviewmodel);
        }

        //
        // POST: /Review/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            ReviewModel reviewmodel = db.Reviews.Find(id);
            db.Reviews.Remove(reviewmodel);
            db.SaveChanges();
            return RedirectToAction("Details", "Good", new { id = reviewmodel.GoodsId });
        }

        public ActionResult Manage()
        {
            ViewBag.Title = "评论管理";
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }        
    }
}