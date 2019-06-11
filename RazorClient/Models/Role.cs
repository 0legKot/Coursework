using Microsoft.AspNetCore.Identity;

namespace ConsulService.Models
{
    public class Role:IdentityRole<string>
    {
        public Role() : base()
        {
        }

        public Role(string name) : base(name)
        {
        }
    }
}