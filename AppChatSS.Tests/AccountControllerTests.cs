using AppChatSS.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace AppChatSS.Tests
{
    public class AccountControllerTests
    {
        [Fact]
        public void LoginViewResult()
        {
            // Arrange
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(m => m.User.IsInRole("Administrator")).Returns(true);
            httpContext.Setup(m => m.User.Identity.Name).Returns("1");
            var context = new ControllerContext(new ActionContext(httpContext.Object, new RouteData(), new ControllerActionDescriptor()));

            AccountController controller = new AccountController();
            controller.ControllerContext = context;

            // Act
            ViewResult result = controller.Login() as ViewResult;
            // Assert
            Assert.NotNull(result);
            Assert.Equal("Login",result?.ViewName);
        }

        [Fact]
        public void RegisterViewResult()
        {
            // Arrange
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(m => m.User.IsInRole("Administrator")).Returns(true);
            httpContext.Setup(m => m.User.Identity.Name).Returns("1");
            var context = new ControllerContext(new ActionContext(httpContext.Object, new RouteData(), new ControllerActionDescriptor()));

            AccountController controller = new AccountController();
            controller.ControllerContext = context;

            // Act
            ViewResult result = controller.Register() as ViewResult;
            // Assert
            Assert.NotNull(result);
            Assert.Equal("Register", result?.ViewName);
        }

        [Fact]
        public void LoginInputApplication()
        {
            // Arrange
            // ------
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(m => m.User.IsInRole("Administrator")).Returns(true);
            httpContext.Setup(m => m.User.Identity.Name).Returns("1");
            var context = new ControllerContext(new ActionContext(httpContext.Object, new RouteData(), new ControllerActionDescriptor()));

            AccountController controller = new AccountController();
            controller.ControllerContext = context;

            // Act
            ViewResult result = controller.Login() as ViewResult;
            // Assert
            Assert.NotNull(result);
            Assert.Equal("Register", result?.ViewName);
        }

        [Fact]
        public void LogoutViewResult()
        {
            // Arrange
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(m => m.User.IsInRole("Administrator")).Returns(true);
            httpContext.Setup(m => m.User.Identity.Name).Returns("1");
            var context = new ControllerContext(new ActionContext(httpContext.Object, new RouteData(), new ControllerActionDescriptor()));

            AccountController controller = new AccountController();
            controller.ControllerContext = context;

            // Act
            var result = controller.Logout();
            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirectToActionResult.ActionName);
        }
    }
}
