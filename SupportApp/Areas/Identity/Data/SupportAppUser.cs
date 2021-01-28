using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace SupportApp.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the SupportAppUser class
    public class SupportAppUser : IdentityUser
    {
    }
    
    public class SupportAppRole : IdentityRole
    {
        
    }
}
