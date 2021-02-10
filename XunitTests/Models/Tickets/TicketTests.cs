using System;
using System.Security.Claims;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SupportApp.Controllers;
using SupportApp.Infrastructure;
using SupportApp.Models.Categories;
using SupportApp.Models.Comments;
using SupportApp.Models.Images;
using SupportApp.Models.Tickets;
using SupportApp.ViewModels.Tickets;

namespace XunitTests.Models.Tickets
{
    public class TicketTests
    {
        private TicketsController _controller;
        private Fixture _fixture;
        private Mock<ITicketsFinder> _ticketsFinderMock;
        private Mock<ITicketsModifier> _ticketsModifierMock;
        private Mock<ICategoryFinder> _categoryFinderMock;
        private ClaimsPrincipal _user;
        private Mock<IMemoryStreamProvider> _memorySteamProviderMock;
        
        
        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _ticketsFinderMock = new Mock<ITicketsFinder>();
            _ticketsModifierMock = new Mock<ITicketsModifier>();
            _categoryFinderMock = new Mock<ICategoryFinder>();
            _memorySteamProviderMock = new Mock<IMemoryStreamProvider>();
            _controller = new TicketsController(_ticketsFinderMock.Object, _ticketsModifierMock.Object,
                _categoryFinderMock.Object, _memorySteamProviderMock.Object);
            _user = new ClaimsPrincipal();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = _user
                }
            };
        }
        
        [Test]
        public void GetComments_Returns_Tickets_Comments()
        {
            var ticket = _fixture.Create<Ticket>();
            var expectedcomment1 = _fixture.Create<Comment>();
            ticket.AddComment(expectedcomment1);
            var expectedcomment2 = _fixture.Create<Comment>();
            ticket.AddComment(expectedcomment2);
            
            // Act & Assert
            ticket.GetComments().Should().BeEquivalentTo(expectedcomment1, expectedcomment2);
        }

        [Test]
        public void GetImages_Returns_Tickets_Images_By_Id()
        {
            var ticket = _fixture.Create<Ticket>();
            var expectedImage1 = _fixture.Create<Image>();
            ticket.AddImage(expectedImage1);
            var expectedImage2 = _fixture.Create<Image>();
            ticket.AddImage(expectedImage2);
            
            // Act & Assert
            ticket.GetImages().Should().BeEquivalentTo(expectedImage1, expectedImage2);
        }

        [Test]
        public void MarkDone_Changes_Tickets_Status_To_Complete_And_CompletedAt_To_DateTime()
        {
            var ticket = _fixture.Create<Ticket>();
            ticket.SetProperty(t => t.IsCompleted, false);
            ticket.SetProperty(t => t.CompletedAt, null);
            
            // Act
            ticket.MarkDone();
            
            // Assert
            ticket.IsCompleted.Should().Be(true);
            ticket.CompletedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(200));
        }

        [Test]
        public void MarkUndone_Changes_Tickets_Status_To_Incomplete()
        {
            var ticket = _fixture.Create<Ticket>();
            ticket.SetProperty(t => t.IsCompleted, true);
            ticket.MarkUndone();
            ticket.IsCompleted.Should().Be(false);   
        }
    }
}