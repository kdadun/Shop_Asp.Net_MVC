using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sklep_Internetowy.Models
{
    public class PersonalData
    {
        [DisplayName("Imię")]
        public string firstName { get; set; }

        [DisplayName("Nazwisko")]
        public string lastName { get; set; }

        [DisplayName("Adres zamieszkania")]
        public string address { get; set; }

        [DisplayName("Miasto")]
        public string city { get; set; }

        [DisplayName("Telefon")]
        [RegularExpression(@"(\+\d{2})*[\d\s-]+",
            ErrorMessage = "Błędny format numeru telefonu.")]
        public string phoneNumber { get; set; }

        [DisplayName("Email")]
        [EmailAddress(ErrorMessage = "Błędny format adresu e-mail.")]
        public string emailForShipping { get; set; }
    }
}