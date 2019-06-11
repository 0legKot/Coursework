using Microsoft.AspNetCore.Identity;

namespace ConsulService.Models
{
    public class User:IdentityUser<string>
    {
        public User() : base()
        {
        }

        public User(string userName) : base(userName)
        {
        }
    }
}