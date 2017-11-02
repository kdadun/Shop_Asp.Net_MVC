using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sklep_Internetowy.Models
{
    public class Contact
    {
        public int Id { get; set; }
        [DisplayName("Imię i nazwisko ")]
        [Required(ErrorMessage = "Musisz podać swoje imię i nazwisko")]
        public string FirstNameAndLastName { get; set; }
        [DisplayName("Numer telefonu ")]
        [Required(ErrorMessage = "Musisz wprowadzić numer telefonu")]
        [RegularExpression(@"(\+\d{2})*[\d\s-]+",
            ErrorMessage = "Błędny format numeru telefonu")]
        public int PhoneNumber { get; set; }
        [Required]
        [DisplayName("Treść wiadomości ")]
        public string MessageContact { get; set; }
    }
}