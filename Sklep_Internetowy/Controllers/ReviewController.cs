using Sklep_Internetowy.Infrastuctures;
using Sklep_Internetowy.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sklep_Internetowy.Controllers
{
    public class ReviewController : Controller
    {
        private ShopContext db = new ShopContext();
        public ActionResult Index([Bind(Prefix = "Id")]int productId)
        {

            var product = db.Products.Find(productId);
            if (product != null)
            {
                return View(product);
            }
            return HttpNotFound();
        }
        [Authorize]
        public ActionResult Create(int productId)
        {
            return View();
        }
        [HttpPost]
        [Authorize]
        public ActionResult Create(Review model)
        {
            if (ModelState.IsValid)
            {
                model.NameUser = User.Identity.Name;
                model.DateCreated = DateTime.Now;
                db.Reviews.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = model.ProductId });

            }

            return View(model);

        }
        [Authorize]
        public ActionResult Edit(int id)
        {
            var review = db.Reviews.Find(id);
            if (review.NameUser == User.Identity.Name)
            {
                if (review != null)
                {
                    return View(review);
                }
            }
            return RedirectToAction("ListsOfProduct", "Products");
        }
        [HttpPost]
        [Authorize]
        public ActionResult Edit(Review model)
        {
            model.DateCreated = DateTime.Now;
            if (model.NameUser == User.Identity.Name)
            {
                if (ModelState.IsValid)
                {
                    //db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index", new { id = model.ProductId });
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(FormCollection form)
        {

            var rating = int.Parse(form["Rating"]);

            Review artComment = new Review()
            {
                //ProductId = articleId,
                //Comment = comment,
                Rating = rating,
                DateCreated = DateTime.Now
            };

            db.Reviews.Add(artComment);
            db.SaveChanges();

            return RedirectToAction("Index", "Review");
        }
    
    }
}