using Sklep_Internetowy.Infrastuctures;
using Sklep_Internetowy.Models;
using Sklep_Internetowy.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
namespace Sklep_Internetowy.Controllers
{
    public class CartController : Controller
    {

        private ShoppingCart shopingCart;
        private SessionManager sesionManager { get; set; }
        private ShopContext db = new ShopContext();

        public CartController()
        {
            this.sesionManager = new SessionManager();
            this.shopingCart = new ShoppingCart(this.sesionManager, this.db);
        }
        public ActionResult ListOfOrders()
        {
            var carIteams = shopingCart.GetCart();
            var totalPrice = shopingCart.GetCartTotalPrice();

            CartViewModel vm = new CartViewModel
            {
                CartItems = carIteams,
                TotalPrice = totalPrice
            };
            return View(vm);
        }
        [Authorize]
        public ActionResult AddToCart(int id)
        {
            shopingCart.AddToCart(id);
            return RedirectToAction("ListOfOrders");
        }
        public ActionResult RemoveFromCart(int productId)
        {
            ShoppingCart shoppingCartManager = new ShoppingCart(this.sesionManager, this.db);

            int itemCount = shoppingCartManager.RemoveFromCart(productId);
            //informacja ile jest aktualnie elementów w koszyku
            int cartItemsCount = shoppingCartManager.GetCartItemsCount();
            //całkowity koszt 
            decimal cartTotal = shoppingCartManager.GetCartTotalPrice();


            var result = new RemoveFromCartViewModel
            {
                RemoveItemId = productId,
                RemovedItemCount = itemCount,
                CartTotal = cartTotal,
                CartItemsCount = cartItemsCount
            };

            return Json(result);
        }
        public int GetCartIteamsCount()
        {
            return shopingCart.GetCartItemsCount();
        }

        public async Task<ActionResult> Checkout()
        {
            if (Request.IsAuthenticated)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

                var order = new Order
                {
                    firstName = user.PersonalData.firstName,
                    lastName = user.PersonalData.lastName,
                    address = user.PersonalData.address,
                    email = user.PersonalData.emailForShipping,
                    phoneNumber = user.PersonalData.phoneNumber
                };

                return View(order);
            }
            else
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Checkout", "Cart") });
        }
        [HttpPost]
        public async Task<ActionResult> Checkout(Order orderdetails)
        {
            if (ModelState.IsValid)
            {


                // Pobierz użytkownika
                var userId = User.Identity.GetUserId();

                // Zapisz zamówienie
                ShoppingCart shoppingCart = new ShoppingCart(this.sesionManager, this.db);
                var newOrder = shoppingCart.CreateOrder(orderdetails, userId);

                // Z aktualizuj dane użytkownika
                var user = await UserManager.FindByIdAsync(userId);
                TryUpdateModel(user.PersonalData);
                await UserManager.UpdateAsync(user);

                // Wyłołaj metodę pusty koszyk
                shoppingCart.EmptyCart();

                var order = db.Orders.Include("OrderIteams")
                    .Include("OrderIteams.Product").SingleOrDefault(o => o.orderId == newOrder.orderId);
                OrderConfirmationEmail email = new OrderConfirmationEmail();
                email.To = order.email;
                email.Cost = order.totalPrice;
                email.OrderNumber = order.orderId;
                email.FullAddress = string.Format("{0} {1}, {2}", order.firstName, order.lastName, order.address);
                email.OrderItems = order.OrderIteams;
                email.Send();

                return RedirectToAction("OrderConfirmation");
            }
            else
                return View(orderdetails);
        }
        public ActionResult OrderConfirmation()
        {
            return View();
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
    }
}