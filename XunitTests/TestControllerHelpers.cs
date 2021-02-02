using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace XunitTests
{
    public static class TestControllerHelpers
    {
        public static void AssertControllerIsAuthorized(this Controller controller, string roles = "")
        {
            var controllerType = controller.GetType();
            var authorize = controllerType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().SingleOrDefault();
            authorize.Should().NotBeNull();
            if (!string.IsNullOrWhiteSpace(roles))
            {
                authorize.Roles.Should().Be(roles);
            }
        }

        public static void AssertActionIsAuthorized(this Controller controller, string action, string roles = "")
        {
            var controllerType = controller.GetType();
            var methodInfos = controllerType.GetMethods().Where(m => m.Name == action).ToArray();
            foreach (var method in methodInfos)
            {
                var authorize = method.GetCustomAttributes(true).OfType<AuthorizeAttribute>().SingleOrDefault();
                authorize.Should().NotBeNull();
                if (!string.IsNullOrWhiteSpace(roles))
                {
                    authorize.Roles.Should().Be(roles);
                }
            }
        }
    }
}