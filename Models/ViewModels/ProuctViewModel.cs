using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
namespace Tsql3s2a.Models.ViewModels
{
    public class ProuctViewModel
    {
        public int Productid {  get; set; }

        [Required(ErrorMessage ="Naziv je obavezan")]
        [StringLength(40,ErrorMessage ="Naziv ne moze biti duzi od 40 karaktera")]
        public string Productname { get; set; } = null!;

        [Required(ErrorMessage = "Dobavljac je obavezan")]
        public int Supplierid { get; set; }


        [Required(ErrorMessage = "Kategorija je obavezna")]
        public int Categoryid { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Cena ne moze biti negativna")]
        public decimal Unitprice { get; set; }

        public bool Discontinued { get; set; }


        public IEnumerable<SelectListItem> Suppliers { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();

    }
}
