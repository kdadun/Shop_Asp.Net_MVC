using Postal;
using Sklep_Internetowy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sklep_Internetowy.ViewModel
{
    public class OrderConfirmationEmail : Email
    {
        public string To { get; set; }
        public decimal Cost { get; set; }
        public int OrderNumber { get; set; }
        public string FullAddress { get; set; }
        public List<OrderIteam> OrderItems { get; set; }
        public byte[] Image { get; set; }
    }

    public class OrderShippedEmail : Email
    {
        public string To { get; set; }
        public int OrderId { get; set; }
        public string FullAddress { get; set; }
    }
}