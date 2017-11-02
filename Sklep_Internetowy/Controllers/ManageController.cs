using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Sklep_Internetowy.Models;
using Sklep_Internetowy.ViewModel;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sklep_Internetowy.Infrastuctures;
using System.Collections.Generic;
using System.Data.Entity;
namespace Sklep_Internetowy.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ShopContext db = new ShopContext();

        public ManageController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }
        public ManageController()
        {

        }

        public ActionResult ListOrder()
        {


            var result = from s in db.Orders.ToList()
                         group s by
                             new { Year = s.dateCreated.Year, Month = s.dateCreated.Month, Day = 1 }
                             into g
                             select new OrderViewModel
                             {
                                 dateCreated = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                                 TotalPrice = g.Sum(x => x.totalPrice)
                             };
            return Json(result, JsonRequestBehavior.AllowGet);



        }
        public ActionResult ListOrder2()
        {
            var result = from s in db.Orders.ToList()
                         group s by
                             new { Year = s.dateCreated.Year, Month = s.dateCreated.Month, Day = 1 }
                             into g
                             select new Order
                             {
                                 dateCreated = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                                 totalPrice = g.Sum(x => x.totalPrice)
                             };

            return PartialView("_SumOrderByMonth", result.ToList());
        }
        public ActionResult ListOrdersToExcel()
        {
            return View(db.Orders.ToList());

        }
        public ActionResult ExportToExcel()
        {
            var gv = new GridView();
            gv.DataSource = db.Orders.Select(r => new
            {
                FirstName = r.firstName,
                LastName = r.lastName,
                Adress = r.address,
                Email = r.email,
                PhoneNumber = r.phoneNumber,
                TotalCost = r.totalPrice,
                OrderState = r.orderState,
                DateCreated=r.dateCreated
            }).ToList();
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=ListaZamówień.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            return View();
        }


        public ActionResult Test()
        {
            return View();
        }

        public ActionResult ContactForm()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ContactForm([Bind(Include = "Id,FirstNameAndLastName,PhoneNumber,MessageContact")]Contact contact)
        {
            if (ModelState.IsValid)
            {
                db.Contacts.Add(contact);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(contact);
        }
        public ActionResult ListCustomerRequests()
        {
            var request = db.Contacts.ToList();
            return View(request);
        }
        public ActionResult OrderList()
        {
            bool isAdmin = User.IsInRole("Admin");
            ViewBag.UserIsAdmin = isAdmin;
            IEnumerable<Order> userOrders;


            if (isAdmin)
            {
                userOrders = db.Orders.Include("OrderIteams").
                    OrderByDescending(o => o.dateCreated).ToArray();
            }

            else
            {
                var userId = User.Identity.GetUserId();
                userOrders = db.Orders.Where(o => o.UserId == userId).Include("OrderIteams").
                    OrderByDescending(o => o.dateCreated).ToArray();
            }

            return View(userOrders);
        }
        public OrderState ChangeOrderState(Order order)
        {
            Order orderToModify = db.Orders.Find(order.orderId);
            orderToModify.orderState = order.orderState;
            db.SaveChanges();

            return order.orderState;
        }


        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            LinkSuccess,
            Error
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

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeProfile([Bind(Prefix = "PersonalData")]PersonalData personalData)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                user.PersonalData = personalData;
                //wprowadzamy zmiany w profilu użytkownika
                var result = await UserManager.UpdateAsync(user);

                AddErrors(result);
            }
            //formularz nie prawidłowo wypełniony
            if (!ModelState.IsValid)
            {
                TempData["ViewData"] = ViewData;
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword([Bind(Prefix = "ChangePasswordViewModel")]ChangePasswordViewModel model)
        {

            if (!ModelState.IsValid)
            {
                TempData["ViewData"] = ViewData;
                return RedirectToAction("Index");
            }

            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInAsync(user, isPersistent: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);


            if (!ModelState.IsValid)
            {
                TempData["ViewData"] = ViewData;
                return RedirectToAction("Index");
            }

            var message = ManageMessageId.ChangePasswordSuccess;
            return RedirectToAction("Index", new { Message = message });
        }



        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            //komunikaty walidacyjny
            if (TempData["ViewData"] != null)
            {
                ViewData = (ViewDataDictionary)TempData["ViewData"];
            }

            if (User.IsInRole("Admin"))
                ViewBag.UserIsAdmin = true;
            else
                ViewBag.UserIsAdmin = false;

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();

            var model = new ManageCredentialsViewModel
            {
                Message = message,
                HasPassword = this.HasPassword(),
                CurrentLogins = userLogins,
                OtherLogins = otherLogins,
                ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1,
                PersonalData = user.PersonalData
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword([Bind(Prefix = "SetPasswordViewModel")]SetPasswordViewModel model)
        {
            
            if (!ModelState.IsValid)
            {
                TempData["ViewData"] = ViewData;
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInAsync(user, isPersistent: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);

                if (!ModelState.IsValid)
                {
                    TempData["ViewData"] = ViewData;
                    return RedirectToAction("Index");
                }
            }

            var message = ManageMessageId.SetPasswordSuccess;
            return RedirectToAction("Index", new { Message = message });
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("password-error", error);
            }
        }
        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.TwoFactorCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, await user.GenerateUserIdentityAsync(UserManager));
        }
    }
}