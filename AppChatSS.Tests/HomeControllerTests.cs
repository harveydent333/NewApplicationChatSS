using AppChatSS.Controllers;
using AppChatSS.Models.Users;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using Xunit;
using System.Web;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Controllers;
using AppChatSS.Models.Members;
using AppChatSS.Models.Messages;
using System.Linq;
using AppChatSS.Models.Rooms;

namespace AppChatSS.Tests
{
    public class HomeControllerTests
    {
        [Fact]
        public void IndexViewResultNotNull()
        {
            // Arrange
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(m=>m.User.IsInRole("Administrator")).Returns(true);
            httpContext.Setup(m => m.User.Identity.Name).Returns("1");
            var context = new ControllerContext(new ActionContext(httpContext.Object, new RouteData(), new ControllerActionDescriptor()));

            HomeController controller = new HomeController();
            controller.ControllerContext = context;

            // Act
            ViewResult result = controller.Index() as ViewResult;
            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ChatViewResultNotNull()
        {
            
            // Arrange
            var mockMember = new Mock<IMessageRepository>();
            mockMember.Setup(mm => mm.Messages.ToList()).Returns(new List<Message>() { new Message { Id = "1", ContentMessage = "Hell Tests", DatePublication=DateTime.Now, RoomId = "1", UserId = 1} });
            var mockRoom = new Mock<RoomRepository>();
            mockRoom.Setup(r => r.FindRoomById("1")).Returns(new Room { Id = "1", RoomName ="MainRoom", LastMessageId = "1", OwnerId = 1, TypeId = 1});

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(m => m.User.IsInRole("Administrator")).Returns(true);
            httpContext.Setup(m => m.User.Identity.Name).Returns("1");
            var context = new ControllerContext(new ActionContext(httpContext.Object, new RouteData(), new ControllerActionDescriptor()));

            HomeController controller = new HomeController(mockRoom.Object);
            controller.ControllerContext = context;
            // Act
            ViewResult result = controller.Chat("1") as ViewResult;
            // Assert
            Assert.NotNull(result);
            
        }
    }
}

