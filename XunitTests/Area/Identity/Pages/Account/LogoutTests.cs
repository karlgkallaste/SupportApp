using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SupportApp.Areas.Identity;
using SupportApp.Areas.Identity.Data;
using SupportApp.Areas.Identity.Pages.Account;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace XunitTests.Area.Identity.Pages.Account
{
    public class LogoutTests
    {
        private Mock<ISupportAppSignInManager> _signInManagerMock;
        private LogoutModel _sut;
        private ClaimsPrincipal _user;

        [SetUp]
        public void Setup()
        {
            _signInManagerMock = new Mock<ISupportAppSignInManager>(MockBehavior.Strict);
            _user = new ClaimsPrincipal();

            _sut = new LogoutModel(_signInManagerMock.Object, Mock.Of<ILogger<LogoutModel>>())
            {
                PageContext = new PageContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = _user,
                    },
                }
            };
        }

        [Test]
        public async Task Logout_Sings_User_Out_And_Redirects_To_Home_Index()
        {
            _user.AddIdentity(new ClaimsIdentity("pwd"));
            
            _signInManagerMock.Setup(m => m.SignOutAsync())
                .Returns(Task.CompletedTask);
            
            var result = (LocalRedirectResult) await _sut.OnPost("/");
            
            // Assert
            _signInManagerMock.Verify(m => m.SignOutAsync(), Times.Once);
            
        }
        private class TestAuthHandlerType : IAuthenticationHandler
        {
            public Task<AuthenticateResult> AuthenticateAsync()
            {
                throw new System.NotImplementedException();
            }

            public Task ChallengeAsync(AuthenticationProperties properties)
            {
                throw new System.NotImplementedException();
            }

            public Task ForbidAsync(AuthenticationProperties properties)
            {
                throw new System.NotImplementedException();
            }

            public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
            {
                throw new System.NotImplementedException();
            }
        }
    }
    
}