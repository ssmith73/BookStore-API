using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_API.Data
{
    public class UserDTO
    {
        //Enforce that an email address is required and that 
        //it must be a valid email address
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]

        public string EmailAddress { get; set; }
        [Required]
        [StringLength(15, ErrorMessage = "Password is limited to 15 characters")]
        [MinLength(8, ErrorMessage = "Password requires a minimum of {1} characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
