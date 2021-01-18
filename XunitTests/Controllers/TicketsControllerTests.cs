
using System;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SupportApp.Controllers;
using SupportApp.Data;
using SupportApp.Models;
using SupportApp.Models.Tickets;
using SupportApp.ViewModels.Tickets;

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
            var ticket1 = _fixture.Create<Ticket>();
            var ticket2 = _fixture.Create<Ticket>();
            var tickets = new[]
            {
                ticket1,
                ticket2,
            };
            
            _ticketsFinderMock.Setup(r => r.FindAllWithStatus(false))
                .Returns(tickets);
            
            // Act
            var result = (ViewResult)_controller.Index();
            
            // Assert
            var expectedViewModels = new[]
            {
                new TicketListViewModel
                {
                    Id = ticket1.Id,
                    Title = ticket1.Title
                },
                new TicketListViewModel
                {
                    Id = ticket2.Id,
                    Title = ticket2.Title
                }
            };
            result.Model.Should().BeEquivalentTo(expectedViewModels);
        }
        [Test]
        public void Completed_returns_completed_tickets_with_the_status_1()
        {
            // Arrange
            var ticket1 = _fixture.Create<Ticket>();
            var ticket2 = _fixture.Create<Ticket>();
            var tickets = new[]
            {
                ticket1,
                ticket2,
            };
            
            _ticketsFinderMock.Setup(r => r.FindAllWithStatus(true))
                .Returns(tickets);
            
            // Act
            var result = (ViewResult)_controller.Completed();
            
            // Assert
            var expectedViewModels = new[]
            {
                new TicketListViewModel
                {
                    Id = ticket1.Id,
                    Title = ticket1.Title
                },
                new TicketListViewModel
                {
                    Id = ticket2.Id,
                    Title = ticket2.Title
                }
            };
            result.Model.Should().BeEquivalentTo(expectedViewModels);
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
            var model = _fixture.Create<CreateTicketModel>();

            Ticket createdTicket = null;
            _ticketsModifierMock.Setup(r => r.Add(It.IsAny<Ticket>()))
                .Callback<Ticket>(t => createdTicket = t);
            
            // Act
            var result = (RedirectToActionResult)_controller.Create(model);
            
            // Assert
            createdTicket.Title.Should().Be(model.Title);
            createdTicket.Description.Should().Be(model.Description);
            createdTicket.Author.Should().Be(model.Author);
            createdTicket.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(200));
            createdTicket.Deadline.Should().Be(createdTicket.CreatedAt.AddDays(2));
            createdTicket.IsCompleted.Should().BeFalse();
            createdTicket.CompletedAt.Should().BeNull();
            
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
        [Test]
        public void MarkDone_Changes_Tickets_CompletedAt_And_IsComplete()
        {
            var ticket = _fixture.Create<Ticket>();
            ticket.MarkUndone();
            
            _ticketsFinderMock.Setup(r => r.Find(ticket.Id))
                .Returns(ticket);
            //act
            var result = (RedirectToActionResult)_controller.MarkDone(ticket.Id);
            //assert
            result.ActionName.Should().Be(nameof(TicketsController.Index));
            ticket.CompletedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(200));
            ticket.IsCompleted.Should().BeTrue();
            _ticketsModifierMock.Verify(m=>m.UpdateTicket(ticket),Times.Once);
            
        }
        
        [Test]
        public void Edit_returns_notfound_when_ticket_is_not_found()
        {
            // Arrange
            var id = 2;
            
            _ticketsFinderMock.Setup(r => r.Find(id))
                .Returns((Ticket)null);
            // Act
            var result = _controller.Edit(id);
            
            
            
            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }
        [Test]
        public void Edit_Ticket_Info_When_Id_Exists()
        {
            // Arrange
            var ticket1 = _fixture.Create<Ticket>();

            _ticketsFinderMock.Setup(r => r.Find(ticket1.Id))
                .Returns(ticket1);
            
            // Act
            var result = (RedirectToActionResult)_controller.Edit(ticket1.Id); 
            
            // Assert
            result.ActionName.Should().Be(nameof(TicketsController.Edit));      
        }

        [Test]
        public void Edit_Marks_Ticket_As_Done()
        {
            var model = _fixture.Create<TicketEditViewModel>();
            model.IsCompleted = true;
            
            var ticket = _fixture.Create<Ticket>();
            ticket.MarkUndone();
            
            _ticketsFinderMock.Setup(r => r.Find(model.Id))
                .Returns(ticket);
            
            // act
            var result = (RedirectToActionResult)_controller.Edit(model);
            
            // assert
            ticket.IsCompleted.Should().BeTrue();
            _ticketsModifierMock.Verify(m=>m.UpdateTicket(ticket),Times.Once);
            
            

        }
        [Test]
        public void Edit_Marks_Ticket_As_Undone()
        {
            var model = _fixture.Create<TicketEditViewModel>();
            model.IsCompleted = false;
            
            var ticket = _fixture.Create<Ticket>();
            ticket.MarkUndone();
            
            _ticketsFinderMock.Setup(r => r.Find(model.Id))
                .Returns(ticket);
            
            // act
            var result = (RedirectToActionResult)_controller.Edit(model);
            
            // assert
            ticket.IsCompleted.Should().BeFalse();
            _ticketsModifierMock.Verify(m=>m.UpdateTicket(ticket),Times.Once);
            
            

        }
    }
}