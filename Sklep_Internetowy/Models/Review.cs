using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sklep_Internetowy.Models
{
    public class Review
    {
        public int Id { get; set; }
        [DisplayName("Ocena produktu")]
        public int Rating { get; set; }
        [Required]
        [DisplayName("Komentarz ")]
        public string Comment { get; set; }
        [DisplayName("Data utworzenia ")]
        public DateTime DateCreated { get; set; }
        public virtual Product Product { get; set; }
        public int ProductId { get; set; }
        public string NameUser { get; set; }
    }
}