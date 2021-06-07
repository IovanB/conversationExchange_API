using Microsoft.AspNetCore.Identity;
using System.Collections;
using System.Collections.Generic;

namespace API.Entities
{
    public class AppRole: IdentityRole<int>
    {
        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}
