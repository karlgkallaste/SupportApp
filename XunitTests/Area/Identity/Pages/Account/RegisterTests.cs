using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity.UI.V3.Pages.Account.Manage.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SupportApp;
using SupportApp.Areas.Identity;
using SupportApp.Areas.Identity.Data;
using SupportApp.Areas.Identity.Pages.Account;
using EmailModel = Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Manage.Internal.EmailModel;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace XunitTests.Identity.Pages.Account
{
    public class RegisterTests
    {
        private Mock<ISupportAppSignInManager> _signInManagerMock;
        private Mock<ISupportAppRoleManager> _roleManagerMock;
        private Mock<ISupportAppUserManager> _supportAppUserManagerMock;
        private Mock<IEmailSender> _emailSenderMock;
        private RegisterModel _sut;
        private ClaimsPrincipal _user;
        private Mock<ISupportAppUrlHelper> _urlHelperMock;

        [SetUp]
        public void Setup()
        {
            _signInManagerMock = new Mock<ISupportAppSignInManager>(MockBehavior.Strict);
            _roleManagerMock = new Mock<ISupportAppRoleManager>(MockBehavior.Strict);
            _supportAppUserManagerMock = new Mock<ISupportAppUserManager>(MockBehavior.Strict);
            _emailSenderMock = new Mock<IEmailSender>(MockBehavior.Strict);
            _urlHelperMock = new Mock<ISupportAppUrlHelper>();
            
            _user = new ClaimsPrincipal();
            _sut = new RegisterModel(_supportAppUserManagerMock.Object, _signInManagerMock.Object, Mock.Of<ILogger<RegisterModel>>(),_emailSenderMock.Object,_roleManagerMock.Object, _urlHelperMock.Object)
            {
                PageContext = new PageContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = _user,
                        Request = { Scheme = "scheme123"}
                    },
                }
            };
        }

        [Test]
        public async Task Get_Redirects()
        {
            //Arrange
            
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
            await _sut.OnGetAsync(returnUrl);
            
            // Assert
            _sut.ExternalLogins.Should().BeEquivalentTo(externalLoginSchemes);
            _sut.ReturnUrl.Should().Be(returnUrl);
        }

        [Test]
        public async Task Post_Creates_NewUser_If_Model_State_Is_Valid()
        {
            var returnUrl = "some return url";
            SupportAppUser createdUser = null;
            _sut.Input = new RegisterModel.InputModel
            {
                Email = "test@test.ee",
                Password = "pwd",
            };
            var externalLoginSchemes = new[]
            {
                new AuthenticationScheme("auth1", "authscheme1", typeof(TestAuthHandlerType)),
                new AuthenticationScheme("auth2", "authscheme2", typeof(TestAuthHandlerType)),
            };
            _signInManagerMock.Setup(m => m.GetExternalAuthenticationSchemesAsync())
                .ReturnsAsync(externalLoginSchemes);
            
            _supportAppUserManagerMock.Setup(m=>m.CreateAsync(It.IsAny<SupportAppUser>(), "pwd"))
                .Callback<SupportAppUser, string>((u,p)=>createdUser=u)
                .ReturnsAsync(IdentityResult.Success);

            _supportAppUserManagerMock.Setup(m => m.AddToRoleAsync(It.IsAny<SupportAppUser>(), "User"))
                .ReturnsAsync(IdentityResult.Success);
            
            var code = "some random code for test";
            _supportAppUserManagerMock.Setup(m => m.GenerateEmailConfirmationTokenAsync(It.IsAny<SupportAppUser>()))
                .ReturnsAsync(code);

            string expectedCallbackUrl = "some url";
            _urlHelperMock.Setup(m => m.Page("/Account/ConfirmEmail", null, It.IsAny<object>(), _sut.Request.Scheme))
                .Returns(expectedCallbackUrl);

            _emailSenderMock.Setup(m => m.SendEmailAsync("test@test.ee", "Confirm your email",
        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(expectedCallbackUrl)}'>clicking here</a>."))
                .Returns(Task.CompletedTask);

            _supportAppUserManagerMock.Setup(m => m.Options)
                .Returns(new IdentityOptions
                {
                    SignIn = new SignInOptions
                    {
                        RequireConfirmedAccount = true
                    }
                });
            
            //Act
            var result = (RedirectToPageResult) await _sut.OnPostAsync(returnUrl);
            
            //_supportAppUserManagerMock.VerifyAll();
            result.PageName.Should().Be("RegisterConfirmation");
            _supportAppUserManagerMock.Verify(m=>m.AddToRoleAsync(createdUser, "User"), Times.Once);

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