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
using SupportApp.Areas.Identity.Pages.Account;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace XunitTests.Identity.Pages.Account
{
    public class LoginTests
    {
        private Mock<ISupportAppSignInManager> _signInManagerMock;
        private LoginModel _sut;
        private ClaimsPrincipal _user;
        
        [SetUp]
        public void Setup()
        {
            _signInManagerMock = new Mock<ISupportAppSignInManager>(MockBehavior.Strict);
            _user = new ClaimsPrincipal();
            _sut = new LoginModel(_signInManagerMock.Object, Mock.Of<ILogger<LoginModel>>())
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
        public async Task Get_Redirects_Authenticated_User_To_Tickets_Index()
        {
            _user.AddIdentity(new ClaimsIdentity("pwd"));

            // Act
            var result = (RedirectToActionResult) await _sut.OnGetAsync();
            
            // Assert
            result.ActionName.Should().Be("Index");
            result.ControllerName.Should().Be("Tickets");
        }
        
        [Test]
        public async Task Get_Returns_Page_With_ExternalLogins_And_RedirectUrl()
        {
            _user.AddIdentity(new ClaimsIdentity());
            var returnUrl = "some_url";

            var externalLoginSchemes = new[]
            {
                new AuthenticationScheme("auth1", "authscheme1", typeof(TestAuthHandlerType)),
                new AuthenticationScheme("auth2", "authscheme2", typeof(TestAuthHandlerType)),
            };

            _signInManagerMock.Setup(m => m.GetExternalAuthenticationSchemesAsync())
                .ReturnsAsync(externalLoginSchemes);
            
            // Act
            var result = (PageResult) await _sut.OnGetAsync(returnUrl);
            
            // Assert
            _sut.ExternalLogins.Should().BeEquivalentTo(externalLoginSchemes);
            _sut.ReturnUrl.Should().Be(returnUrl);
        }

        [Test]
        public async Task Post_Returns_Page_When_ModelState_Is_Invalid()
        {
            _sut.ModelState.AddModelError("error", "some error");
            // Act
            var result = (PageResult) await _sut.OnPostAsync("return url");
            // Assert does not throw
        }

        [Test]
        public async Task Post_login_with_PasswordSignInAsync_returns_baseurl()
        {
            _signInManagerMock.Setup(m => m.SignOutAsync())
                .Returns(Task.CompletedTask);

            _sut.Input = new LoginModel.InputModel
            {
                Email = "user",
                Password = "pwd",
                RememberMe = true
            };
            
            _signInManagerMock.Setup(m => m.PasswordSignInAsync("user", "pwd", true, false))
                .ReturnsAsync(SignInResult.Success);
            
            var result = (LocalRedirectResult) await _sut.OnPostAsync("some url");

            // Act
            result.Url.Should().Be("some url");
            
            _sut.ReturnUrl.Should().Be(null);
            _signInManagerMock.Verify(m => m.SignOutAsync(), Times.Once);
            _signInManagerMock.Verify(m=>m.PasswordSignInAsync("user", "pwd", true, false), Times.Once);
        }

        [Test]
        public async Task Post_Login_With_PasswordSignInAsync_Redirects_If_2factor_Is_Needed()
        {
            _signInManagerMock.Setup(m => m.SignOutAsync())
                .Returns(Task.CompletedTask);
            const string pageName = "./LoginWith2fa";

            _sut.Input = new LoginModel.InputModel
            {
                Email = "user",
                Password = "pwd",
                RememberMe = true
            };

            _signInManagerMock.Setup(m => m.PasswordSignInAsync("user", "pwd", true, false))
                .ReturnsAsync(SignInResult.TwoFactorRequired);
            
            // Act
            var result = (RedirectToPageResult) await _sut.OnPostAsync("./Lockout");

            result.PageName.Should().Be(pageName);
            result.RouteValues["ReturnUrl"].Should().Be("./Lockout");
            result.RouteValues["RememberMe"].Should().Be(true);
            _signInManagerMock.Verify(m => m.SignOutAsync(), Times.Once);
            _signInManagerMock.Verify(m=>m.PasswordSignInAsync("user", "pwd", true, false), Times.Once);
        }
        [Test]
        public async Task Post_login_with_PasswordSignInAsync_Redirects_If_LockedOut()
        {
           
            _signInManagerMock.Setup(m => m.SignOutAsync())
                .Returns(Task.CompletedTask);
            const string pageName = "./LoginWith2fa";
            _sut.Input = new LoginModel.InputModel
            {
                Email = "user",
                Password = "pwd",
                RememberMe = true
            };
            
            _signInManagerMock.Setup(m => m.PasswordSignInAsync("user", "pwd", true, false))
                .ReturnsAsync(SignInResult.TwoFactorRequired);
            // Act
            var result = (RedirectToPageResult) await _sut.OnPostAsync("./LoginWith2fa");
            //Assert
            result.PageName.Should().Be(pageName);
            result.RouteValues["ReturnUrl"].Should().Be("./LoginWith2fa");
            _signInManagerMock.Verify(m => m.SignOutAsync(), Times.Once);
            _signInManagerMock.Verify(m=>m.PasswordSignInAsync("user", "pwd", true, false), Times.Once);
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