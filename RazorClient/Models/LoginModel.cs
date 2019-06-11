using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RazorClient.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Enter name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Enter password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public override string ToString()
        {
            return "{\""+$"Name\":\"{Name}\", \"Password\":\"{Password}\"" +"}";
        }
    }
}
