using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SupportApp.Areas.Identity.Data;

namespace SupportApp.Areas.Identity
{
    public class ApplicationUserClaimsPrincipalFactory: UserClaimsPrincipalFactory<SupportAppUser>
    {
        public ApplicationUserClaimsPrincipalFactory(UserManager<SupportAppUser> userManager, IOptions<IdentityOptions> optionsAccessor) : base(userManager, optionsAccessor)
        {
        }
    }
}