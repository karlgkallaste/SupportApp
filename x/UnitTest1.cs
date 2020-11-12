using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using SupportApp.Controllers;
using SupportApp.Data;
using SupportApp.Models;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Microsoft.EntityFrameworkCore;

namespace x
{
    public class UnitTest1
    {
        [Fact]
        public async Task Check_if_index_exists()
        {
            var controller = new HomeController();
            var result = controller.Index();
            Assert.IsType<ViewResult>(result);
        }
        [Fact]
        public async Task Check_if_privacy_page_exists()
        {
            var controller = new HomeController();
            var result = controller.Privacy();
            Assert.IsType<ViewResult>(result);
        }
        [Fact]
        public void Index_should_return_default_view()
        {
            var controller = new HomeController();
            var viewResult = (ViewResult)controller.Index();
            var viewName = viewResult.ViewName;

            Assert.True(string.IsNullOrEmpty(viewName) || viewName == "Index");
        }
        [Fact]
        public async Task Get_Ticket_By_Id_Return_OkResult()
        {
            var controller = new TicketsController(supportAppRepository);
            var Id = 2;
            var data = await controller.Edit(Id);
            Assert.IsType<OkObjectResult>(data);
        }

    }
}