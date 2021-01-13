
using System;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SupportApp.Controllers;
using SupportApp.Data;
using SupportApp.Models;
using SupportApp.Models.Tickets;

namespace XunitTests.Controllers
{
    public class TicketsControllerTests
    {
        private TicketsController _controller;
        private Fixture _fixture;
        private Mock<ITicketsFinder> _ticketsFinderMock;
        private Mock<ITicketsModifier> _ticketsModifierMock;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _ticketsFinderMock = new Mock<ITicketsFinder>();
            _ticketsModifierMock = new Mock<ITicketsModifier>();
            _controller = new TicketsController(_ticketsFinderMock.Object, _ticketsModifierMock.Object);
        }
        
        [Test]
        public void Index_returns_incomplete_tickets_that_are_ordered_by_created_at()
        {
            // Arrange
            var tickets = new[]
            {
                _fixture.Create<Ticket>(),
                _fixture.Create<Ticket>(),
                _fixture.Create<Ticket>(),
                _fixture.Create<Ticket>(),
            };
            
            _ticketsFinderMock.Setup(r => r.FindAllWithStatus(0))
                .Returns(tickets);
            
            // Act
            var result = (ViewResult)_controller.Index();
            
            // Assert
            result.Model.Should().BeEquivalentTo(tickets);
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
            
            _ticketsFinderMock.Setup(r => r.FindAllWithStatus(1))
                .Returns(tickets);
            
            // Act
            var result = (ViewResult)_controller.Completed();
            
            // Assert
            result.Model.Should().BeEquivalentTo(tickets);
        }

        [Test]
        public void Details_returns_notfound_when_ticket_is_not_found()
        {
            // Arrange
            var id = 2;
            
            _ticketsFinderMock.Setup(r => r.Find(id))
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
            
            _ticketsFinderMock.Setup(r => r.Find(ticketId))
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
            _ticketsModifierMock.Verify(r => r.Add(ticket), Times.Once);
            result.ActionName.Should().Be(nameof(TicketsController.Index));
        }
        [Test]
        public void DeleteConfirmed_ticket_gets_deleted_by_id_and_user_gets_redirected()
        {
            // Arrange
            var ticketId = 1;

            // Act
            var result = (RedirectToActionResult)_controller.DeleteConfirmed(ticketId);
            
            // Assert
            _ticketsModifierMock.Verify(r => r.RemoveTicket(ticketId), Times.Once);
            result.ActionName.Should().Be(nameof(TicketsController.Index));
        }
        [Test]
        public void DeleteConfirmed_redirects_to_index_if_ticket_doesnt_exist()
        {
            // Arrange

            _ticketsFinderMock.Setup(r => r.Find(2))
                .Returns((Ticket)null);
            // Act
            var result = (RedirectToActionResult)_controller.DeleteConfirmed(2);
            
            // Assert
            result.ActionName.Should().Be(nameof(TicketsController.Index));
        }
    }
}