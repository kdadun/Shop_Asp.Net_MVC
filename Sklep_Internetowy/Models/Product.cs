using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sklep_Internetowy.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(60), MinLength(3)]
        [DisplayName("Nazwa produktu")]
        public string Name { get; set; }
        [Required]
        [DisplayName("Opis produktu")]
        public string Description { get; set; }
        [DisplayName("Szczegółowy opis ")]
        public string DetailedDescription { get; set; }
        [DisplayName("Cena")]
        public decimal Price { get; set; }
        [DisplayName("Nowy produkt")]
        public bool IsNewProduct { get; set; }
        [DisplayName("Promocyjny produkt")]
        public bool PromotionalProduct { get; set; }
        public string NameFileImage { get; set; }
        public byte[] Image { get; set; }
        public int? CategoryId { get; set; }
        public virtual Category category { get; set; }
        public virtual ICollection<Review> Rewiews { get; set; }
    }
}