using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using SupportApp.Areas.Identity.Data;

namespace SupportApp.Areas.Identity
{
    public interface ISupportAppSignInManager
    {
        Task<IEnumerable<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync();
        Task SignInAsync(SupportAppUser user, bool isPersistent);
        Task<SignInResult> PasswordSignInAsync(string userName, string password,
            bool isPersistent, bool lockoutOnFailure);

        Task SignOutAsync();
    }
    public class SupportAppSignInManager : ISupportAppSignInManager
    {
        private readonly SignInManager<SupportAppUser> _signInManager;

        public SupportAppSignInManager(SignInManager<SupportAppUser> signInManager)
        {
            _signInManager = signInManager;
            _signInManager.SignOutAsync();
        }

        public async Task<IEnumerable<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync()
        {
            return await _signInManager.GetExternalAuthenticationSchemesAsync();
        }

        public async Task SignInAsync(SupportAppUser user, bool isPersistent)
        {
            await _signInManager.SignInAsync(user, isPersistent);
        }

        public async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            return await _signInManager.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}