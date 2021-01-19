using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SupportApp.Areas.Identity.Data;

namespace SupportApp.Areas.Identity
{
    public interface ISupportAppUserManager
    {
        Task<IdentityResult> CreateAsync(SupportAppUser user, string password);
        Task<string> GenerateEmailConfirmationTokenAsync(SupportAppUser user);

        Task<IdentityResult> AddToRoleAsync(SupportAppUser user, string role);
        IdentityOptions Options { get; }
    }

    public class SupportAppUserManager : ISupportAppUserManager
    {
        private readonly UserManager<SupportAppUser> _userManager;

        public SupportAppUserManager(UserManager<SupportAppUser> userManager)
        {
            _userManager = userManager;

        }

        public async Task<IdentityResult> CreateAsync(SupportAppUser user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(SupportAppUser user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<IdentityResult> AddToRoleAsync(SupportAppUser user, string role)
        {
            return await _userManager.AddToRoleAsync(user, role);
        }

        public IdentityOptions Options => _userManager.Options;
        
    }
}