using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sklep_Internetowy.Models
{
    public class Order
    {
        public int orderId { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        [Required(ErrorMessage = "Musisz wprowadzić imię")]
        [StringLength(100)]
        [DisplayName("Imię")]
        public string firstName { get; set; }
        [DisplayName("Nazwisko")]
        [Required(ErrorMessage = "Musisz wprowadzć nazwisko")]
        [StringLength(150)]
        public string lastName { get; set; }
        [DisplayName("Adress")]
        [Required(ErrorMessage = "Nie wprowadzono adresu")]
        [StringLength(150)]
        public string address { get; set; }
        [Required(ErrorMessage = "Musisz wprowadzić numer telefonu")]
        [DisplayName("Numer telefonu")]
        [StringLength(20)]
        [RegularExpression(@"(\+\d{2})*[\d\s-]+",
        ErrorMessage = "Błędny format numeru telefonu.")]
        public string phoneNumber { get; set; }
        [Required(ErrorMessage = "Wprowadź swój adres e-mail.")]
        [DisplayName("Email")]
        [EmailAddress(ErrorMessage = "Błędny format adresu e-mail.")]
        public string email { get; set; }
        public string comment { get; set; }
        [DisplayName("Data utworzenia zamówienia")]
        public DateTime dateCreated { get; set; }
        [DisplayName("Status zamówienia")]
        public OrderState orderState { get; set; }
        [DisplayName("Całkowity koszt zamówienia")]
        public decimal totalPrice { get; set; }

        public List<OrderIteam> OrderIteams { get; set; }
    }
    public enum OrderState{
        
        [Display(Name="Nowe")]
        New,
        [Display(Name = "W trakcie realizacji")]
        InProgress,
        [Display(Name = "Zrealizowane")]
        Completed
}
    }
