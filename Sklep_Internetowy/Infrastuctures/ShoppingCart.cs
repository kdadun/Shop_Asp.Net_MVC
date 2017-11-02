using Sklep_Internetowy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sklep_Internetowy.Infrastuctures
{
    public class ShoppingCart
    {
        private ISessionManager session;
        private ShopContext db;
        public const string CartSessionKey = "SessionKey";
        public ShoppingCart(ISessionManager session, ShopContext db)
        {
            this.session = session;
            this.db = db;
        }
        public List<CartItem> GetCart()
        {
            List<CartItem> cart;
            //sprawdzam czy mam zapisaną pozycję koszyka w sesji 
            if (session.Get<List<CartItem>>(CartSessionKey) == null)
            {

                cart = new List<CartItem>();
            }
            else
            {    //zwracamy istniejącą listę pozycji koszyka 
                cart = session.Get<List<CartItem>>(CartSessionKey) as List<CartItem>;
            }
            return cart;
        }

        //metoda dodająca produkt do koszyka 
        public void AddToCart(int productId)
        {
            var cart = this.GetCart();
            //szukam czy dany produkt jest już w koszyku,jeśli jest to zwiększam jego ilość  
            var cartItem = cart.Find(c => c.Product.Id == productId);

            if (cartItem != null)
                cartItem.Quantity++;
            else
            {
                var productToAdd = db.Products.Where(a => a.Id == productId).SingleOrDefault();
                if (productToAdd != null)
                {
                    var newCartItem = new CartItem()
                    {
                        Product = productToAdd,
                        Quantity = 1,
                        TotalPrice = productToAdd.Price
                    };

                    cart.Add(newCartItem);
                }
            }
            //zapisuje stan listy do sesji  
            session.Set(CartSessionKey, cart);
        }

        //metoda usuwająca produkt z koszyka 
        public int RemoveFromCart(int productId)
        {
            var cart = this.GetCart();
            var cartItem = cart.Find(n => n.Product.Id == productId);
            if (cartItem != null)
            {
                if (cartItem.Quantity > 1)
                {
                    cartItem.Quantity--;
                    return cartItem.Quantity;
                }
                else
                {
                    cart.Remove(cartItem);
                }

            }
            return 0;
        }
        public decimal GetCartTotalPrice()
        {
            var cart = this.GetCart();
            return cart.Sum(n => (n.Quantity * n.Product.Price));

        }
        public int GetCartItemsCount()
        {
            var cart = this.GetCart();
            int count = cart.Sum(n => n.Quantity);
            return count;
        }
        public Order CreateOrder(Order newOrder, string userId)
        {
            var cart = this.GetCart();

            newOrder.dateCreated = DateTime.Now;
            newOrder.UserId = userId;

            this.db.Orders.Add(newOrder);

            //sprawdzamy czy są jakieś pozycje zamówienia 
            if (newOrder.OrderIteams == null)
                newOrder.OrderIteams = new List<OrderIteam>();

            decimal cartTotal = 0;

            foreach (var cartItem in cart)
            {
                var newOrderItem = new OrderIteam()
                {
                    ProductId = cartItem.Product.Id,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Product.Price
                };

                cartTotal += (cartItem.Quantity * cartItem.Product.Price);

                newOrder.OrderIteams.Add(newOrderItem);
            }

            newOrder.totalPrice = cartTotal;

            this.db.SaveChanges();

            return newOrder;
        }

        //metoda czyszcząca koszyk po zrealizowaniu zamówienia 
        public void EmptyCart()
        {
            session.Set<List<CartItem>>(CartSessionKey, null);
        }
    }
}