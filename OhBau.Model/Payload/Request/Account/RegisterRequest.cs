﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Enum;
using OhBau.Model.Payload.Request.Parent;

namespace OhBau.Model.Payload.Request.Account
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Phone is required")]
        [RegularExpression(@"^(0|\+84)(3[2-9]|5[6|8|9]|7[0|6-9]|8[1-5]|9[0-9])[0-9]{7}$",
        ErrorMessage = "Invalid Vietnamese phone number")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$",
        ErrorMessage = "Password must be at least 8 characters long and contain a mix of upper/lowercase letters, numbers, and special characters.")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[\w\.-]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Email is invalid")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = null!;
        public RoleEnum Role { get; set; }
        public RegisterParentRequest RegisterParentRequest { get; set; } = null!;
    }
}
