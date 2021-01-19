using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SupportApp.Areas.Identity.Data;

namespace SupportApp.Areas.Identity
{
    public class ApplicationUserClaimsPrincipalFactory: UserClaimsPrincipalFactory<ApplicationUser>
    {
        public ApplicationUserClaimsPrincipalFactory(UserManager<ApplicationUser> userManager, IOptions<IdentityOptions> optionsAccessor) : base(userManager, optionsAccessor)
        {
        }
    }
}