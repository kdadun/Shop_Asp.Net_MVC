using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sklep_Internetowy.ViewModel
{
    public class CreateProductViewModel
    {
        [Required]
        [MaxLength(60), MinLength(3)]
        [Display(Name = "Nazwa")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Opis")]
        public string Description { get; set; }
        [Display(Name = "Opis szczegółowy")]
        public string DetailedDescription { get; set; }
        [Display(Name = "Cena")]
        public decimal Price { get; set; }
        [Display(Name = "Wybierz kategorię")]
        public int CategoryId { get; set; }
        public string NameFileIcon { get; set; }
        [Display(Name = "Nowy produkt")]
        public bool IsNewProduct { get; set; }
        [Display(Name = "Promocyjny produkt")]
        public bool PromotionalProduct { get; set; }
        public string NameFileImage { get; set; }
        public byte[] Image { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }
    }
}