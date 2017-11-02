using Sklep_Internetowy.Infrastuctures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sklep_Internetowy.Controllers
{
    public class RandomProductController : Controller
    {
        public ShopContext db = new ShopContext();

        public ActionResult Index()
        {
            var product = db.Products.OrderBy(g => Guid.NewGuid()).Take(2).ToList();
            return View(product);
        }
    }
}