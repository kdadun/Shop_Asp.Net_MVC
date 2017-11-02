using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sklep_Internetowy.ViewModel
{
    public class OrderViewModel
    {
        public DateTime dateCreated { get; set; }
        public decimal TotalPrice { get; set; }
    }
}