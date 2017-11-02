using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Sklep_Internetowy.Infrastuctures;
using Sklep_Internetowy.ViewModel;
namespace Sklep_Internetowy.Controllers
{
    public class HomeController : Controller
    {
        public ShopContext db = new ShopContext();

        public ActionResult Index()
        {
            var product = db.Products.Where(p => p.PromotionalProduct).ToList();
            var categories = db.Categories.ToList();

            var vm = new HomeViewModel
            {
                Products = product,
                Categories = categories

            };
            return View(vm);
        }

        public ActionResult CheapestProduct()
        {
            var prod = db.Products.OrderBy(n => n.Price).FirstOrDefault();
            return PartialView("_CheapestProduct", prod);
        }

        public ActionResult ShopRules()
        {
            return View();
        }


    }
}