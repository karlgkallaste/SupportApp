
using System;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SupportApp.Controllers;
using SupportApp.Data;
using SupportApp.Models;

namespace XunitTests.Controllers
{
    public class TicketsControllerTests
    {
        private TicketsController _controller;
        private Mock<ISupportAppRepository<Ticket>> _ticketsRepoMock;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _ticketsRepoMock = new Mock<ISupportAppRepository<Ticket>>(MockBehavior.Strict);
            _controller = new TicketsController(_ticketsRepoMock.Object);
        }
        
        [Test]
        public void Index_returns_incomplete_tickets_that_are_ordered_by_created_at()
        {
            // Arrange
            var ticket1 = _fixture.Create<Ticket>();
            ticket1.CreatedAt = DateTime.Now.AddSeconds(1);
            
            var ticket2 = _fixture.Create<Ticket>();
            ticket2.CreatedAt = DateTime.Now.AddSeconds(2);
            
            var ticket3 = _fixture.Create<Ticket>();
            ticket3.CreatedAt = DateTime.Now.AddSeconds(3);
            
            var tickets = new[]
            {
                ticket3,
                ticket1,
                ticket2,
            };
            
            _ticketsRepoMock.Setup(r => r.Find(t => t.Status == 0))
                .Returns(tickets);
            
            // Act
            var result = (ViewResult)_controller.Index();
            
            // Assert
            result.Model.Should().BeEquivalentTo(new[] {ticket1, ticket2, ticket3}, conf => conf.WithStrictOrdering());
        }
        [Test]
        public void Completed_returns_completed_tickets_with_the_status_1()
        {
            // Arrange
            var tickets = new[]
            {
                _fixture.Create<Ticket>(),
                _fixture.Create<Ticket>(),
                _fixture.Create<Ticket>(),
            };
            
            _ticketsRepoMock.Setup(r => r.Find(t => t.Status == 1))
                .Returns(tickets);
            
            // Act
            var result = (ViewResult)_controller.Completed();
            
            // Assert
            result.Model.Should().BeEquivalentTo(tickets);
        }
        [Test]
        public void Details_returns_notfound_when_id_is_null()
        {
            // Arrange

            
            // Act
            var result = _controller.Details(null);
            
            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }
        [Test]
        public void Details_returns_notfound_when_ticket_is_not_found()
        {
            // Arrange
            var id = 2;
            
            _ticketsRepoMock.Setup(r => r.GetById(id))
                .Returns((Ticket)null);
            // Act
            var result = _controller.Details(id);
            
            
            
            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }
        [Test]
        public void Details_returns_view_with_ticket_by_id()
        {
            // Arrange
            var ticketId = 12345;
            var ticket = _fixture.Create<Ticket>();
            
            _ticketsRepoMock.Setup(r => r.GetById(ticketId))
                .Returns(ticket);
            
            // Act
            var result = (ViewResult)_controller.Details(ticketId);
            
            // Assert
            result.Model.Should().BeEquivalentTo(ticket);
        }
        [Test]
        public void Create_new_ticket_gets_added()
        {
            // Arrange
            var ticket = _fixture.Create<Ticket>();
            
            // Act
            var result = (RedirectToActionResult)_controller.Create(ticket);
            
            // Assert
            _ticketsRepoMock.Verify(r => r.Add(ticket), Times.Once);
            result.ActionName.Should().Be(nameof(TicketsController.Index));
        }
        [Test]
        public void DeleteConfirmed_ticket_gets_deleted_by_id_and_user_gets_redirected()
        {
            // Arrange
            var ticket = _fixture.Create<Ticket>();
            ticket.Id = 1;
            
            _ticketsRepoMock.Setup(r => r.GetById(ticket.Id))
                .Returns(ticket);
            // Act
            var result = (RedirectToActionResult)_controller.DeleteConfirmed(ticket.Id);
            
            // Assert
            _ticketsRepoMock.Verify(r => r.Remove(ticket), Times.Once);
            result.ActionName.Should().Be(nameof(TicketsController.Index));
        }
        [Test]
        public void DeleteConfirmed_redirects_to_index_if_ticket_doesnt_exist()
        {
            // Arrange

            _ticketsRepoMock.Setup(r => r.GetById(2))
                .Returns((Ticket)null);
            // Act
            var result = (RedirectToActionResult)_controller.DeleteConfirmed(2);
            
            // Assert
            result.ActionName.Should().Be(nameof(TicketsController.Index));
        }
    }
}