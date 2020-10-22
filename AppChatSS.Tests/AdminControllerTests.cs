using AppChatSS.Controllers;
using AppChatSS.Models.Roles;
using AppChatSS.Models.Users;
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
    public class AdminControllerTests
    {
        [Fact]
        public void IndexViewResultNotNull()
        {
            // Arrange
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(m => m.User.IsInRole("Administrator")).Returns(true);
            httpContext.Setup(m => m.User.Identity.Name).Returns("1");
            var context = new ControllerContext(new ActionContext(httpContext.Object, new RouteData(), new ControllerActionDescriptor()));

            AdminController controller = new AdminController();
            controller.ControllerContext = context;

            // Act
            ViewResult result = controller.Index() as ViewResult;
            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void SetRegularUserRole()
        {
            // Arrange
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(m => m.User.IsInRole("Administrator")).Returns(true);
            httpContext.Setup(m => m.User.Identity.Name).Returns("1");
            var context = new ControllerContext(new ActionContext(httpContext.Object, new RouteData(), new ControllerActionDescriptor()));

            AdminController controller = new AdminController();
            controller.ControllerContext = context;

            // Act
            ViewResult result = controller.SetRegularUserRole(2) as ViewResult;
            // Assert
            Assert.NotNull(result);
        }

        private List<Role> GetTestRoles()
        {
            var roles = new List<Role>
            {
                new Role { Id = 1, RoleName = "RegularUser"},
                new Role { Id = 2, RoleName = "Moderator" },
                new Role { Id = 3, RoleName = "Administrator" }
            };
            return roles;
        }

        private List<User> GetTestUsers()
        {
            var users = new List<User>
            {
                new User { Id=2, Login="login1", Email="email1@mail.ru"},
            };
            return users;
        }

    }
}
/*
var mock = new Mock<IRepository>();
mock.Setup(repo => repo.GetAll()).Returns(GetTestUsers());
var controller = new HomeController(mock.Object);

// Act
var result = controller.Index();

// Assert
var viewResult = Assert.IsType<ViewResult>(result);
var model = Assert.IsAssignableFrom<IEnumerable<User>>(viewResult.Model);
Assert.Equal(GetTestUsers().Count, model.Count());
*/