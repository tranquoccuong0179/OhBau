using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace OhBau.Model.Payload.Request.Doctor
{
    public class DoctorRequest
    {
        [Required(ErrorMessage = "Full Name is required")]
        [RegularExpression(@"^[\p{L}\s]+$", ErrorMessage = "Full Name can only contain letters and spaces.")]
        [DataType(DataType.Text)]
        public string FullName { get; set; }

        public string? Avatar { get; set; }

        [Required(ErrorMessage = "Date of Birth is required")]
        public DateOnly DOB { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        [MaxLength(50, ErrorMessage = "Gender cannot be longer than 50 characters.")]
        [RegularExpression(@"^(Male|Female|Other)$", ErrorMessage = "Gender must be Male, Female, or Other.")]
        public string Gender { get; set; }

        [Required(ErrorMessage ="Content is required")]
        public string Content {  get; set; }

        [Required(ErrorMessage = "Address is required")]
        [RegularExpression(@"^[\p{L}0-9\s,.\-]{5,500}$",
        ErrorMessage = "Address must be between 5 and 500 characters, and can include letters (including Vietnamese), numbers, spaces, commas, periods, and hyphens.")]
        [DataType(DataType.Text)]
        public string Address { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public bool Active { get; set; } = true;
        
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public DateTime CreateAt {  get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Medical profile is required")]
        public string MedicalProfile { get; set; }

        [Required(ErrorMessage = "CareerPath profile is required")]
        public string CareerPath { get; set; }

        [Required(ErrorMessage = "OutStanding profile is required")]
        public string OutStanding { get; set; }

        [Required(ErrorMessage = "Experence profile is required")]
        public string Experence { get; set; }

        [Required(ErrorMessage = "Experence profile is required")]
        public string Focus { get; set; }
    }
}
