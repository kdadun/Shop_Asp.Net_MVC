using Sklep_Internetowy.Infrastuctures;
using Sklep_Internetowy.Models;
using Sklep_Internetowy.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
namespace Sklep_Internetowy.Controllers
{
    public class ProductsController : Controller
    {
        private ShopContext db = new ShopContext();

      public ActionResult ListProducts(string search)
    {
        IEnumerable<Product> model;
        if (!string.IsNullOrEmpty(search))
        {
             model = db.Products.Where(p => p.Name.Contains(search)).ToList();
        }
        else
            model=db.Products.ToList();

        return View(model);
    }


        public ActionResult ListOfProductsForAdmin(int? page, int? categoryId = null)
        {


            IEnumerable<Product> products;
            if (categoryId.HasValue)
            {
                if (db.Categories.Any(c => c.Id == categoryId.Value))
                {
                    products = db.Products.Where(p => p.CategoryId == categoryId.Value);
                    return View(products.ToList().ToPagedList(page ?? 1, 3));
                }
            }

            return View(db.Products.ToList().ToPagedList(page ?? 1, 3));
        }


        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
           

            return View(product);
        }
    
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            var model = new CreateProductViewModel();
            model.Categories = db.Categories.ToList().Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "Id,Name,Description,DetailedDescription,Price,NameFileIcon,IsNewProduct,PromotionalProduct,CategoryId,NameFileImage,Image")] Product product, HttpPostedFileBase productImg)
        {
            if (ModelState.IsValid)
            {
                if (productImg != null)
                {
                    product.Image = new byte[productImg.ContentLength];
                    productImg.InputStream.Read(product.Image, 0, productImg.ContentLength);
                }
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("ListOfProductsForAdmin");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }


        public ActionResult Edit(int? id)
        {
            //var model = new CreateProductViewModel() { };
            //model.Categories = db.Categories.ToList().Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() });

            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }


            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,CategoryId,DetailedDescription,Price,IsNewProduct,PromotionalProduct,NameFileImage,Image")] Product product, HttpPostedFileBase productImg)
        {
            if (ModelState.IsValid)
            {
                if (productImg != null)
                {
                    product.Image = new byte[productImg.ContentLength];
                    productImg.InputStream.Read(product.Image, 0, productImg.ContentLength);
                }
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ListOfProductsForAdmin");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("ListOfProductsForAdmin");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    
    }
}