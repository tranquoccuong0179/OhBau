using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using OhBau.Model.Entity;
using OhBau.Model.Enum;
using OhBau.Model.Payload.Request.Major;

namespace OhBau.Model.Payload.Request.Doctor
{
    public class CreateDoctorRequest
    {
        [Required(ErrorMessage = "Phone is required")]
        [RegularExpression(@"^(0|\+84)(3[2-9]|5[6|8|9]|7[0|6-9]|8[1-5]|9[0-9])[0-9]{7}$",
        ErrorMessage = "Invalid Vietnamese phone number")]
        [DataType(DataType.PhoneNumber)]
        [MaxLength(10,ErrorMessage = "Phone number must not exceed 10 characters")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[\w\.-]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Email is invalid")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,32}$", 
        ErrorMessage = "Password must be at least 8 characters long and contain a mix of upper/lowercase letters, numbers, and special characters.")]
        [MinLength(8, ErrorMessage = "Password must be 8 characters or more."),MaxLength(32, ErrorMessage = "Password must be from 8 to 32 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public bool Active {  get; set; } = true;

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public string Role { get;} = RoleEnum.DOCTOR.ToString();

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public DateTime CreateAt { get; set; } = DateTime.Now;
        
        public CreateMajorRequest CreateMajorRequest { get; set; }

        public DoctorRequest DoctorRequest { get; set; }




    }
}
