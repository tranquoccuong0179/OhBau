using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Request.Doctor
{
    public class DoctorRequest
    {
        [Required(ErrorMessage = "Full Name is required")]
        [RegularExpression(@"^[\p{L}\s]+$", ErrorMessage = "Full Name can only contain letters and spaces.")]
        [DataType(DataType.Text)]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Date of Birth is required")]
        public DateOnly DOB { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        [MaxLength(50, ErrorMessage = "Gender cannot be longer than 50 characters.")]
        [RegularExpression(@"^(Male|Female|Other)$", ErrorMessage = "Gender must be Male, Female, or Other.")]
        public string Gender { get; set; }


        [Required(ErrorMessage ="Content is required")]
        public string Content {  get; set; }

        [Required(ErrorMessage = "Address is required")]
        [RegularExpression(@"^[a-zA-Z0-9\s,.-]{5,100}$", 
        ErrorMessage = "Address must be between 5 and 100 characters, and can include letters, numbers, spaces, commas, periods, and hyphens.")]
        [DataType(DataType.Text)]
        public string Address { get; set; }

        public bool Active { get; set; } = true;

        public DateTime CreateAt {  get; set; } = DateTime.Now;

    }
}
