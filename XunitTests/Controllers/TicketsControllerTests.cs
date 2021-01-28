
using System;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SupportApp.Controllers;
using SupportApp.Data;
using SupportApp.Models;
using SupportApp.Models.Categories;
using SupportApp.Models.Comments;
using SupportApp.Models.Tickets;
using SupportApp.ViewModels.Categories;
using SupportApp.ViewModels.Comments;
using SupportApp.ViewModels.Tickets;

namespace XunitTests.Controllers
{
    public class TicketsControllerTests
    {
        private TicketsController _controller;
        private Fixture _fixture;
        private Mock<ITicketsFinder> _ticketsFinderMock;
        private Mock<ITicketsModifier> _ticketsModifierMock;
        private Mock<ICategoryFinder> _categoryFinderMock;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _ticketsFinderMock = new Mock<ITicketsFinder>();
            _ticketsModifierMock = new Mock<ITicketsModifier>();
            _categoryFinderMock = new Mock<ICategoryFinder>();
            _controller = new TicketsController(_ticketsFinderMock.Object, _ticketsModifierMock.Object, _categoryFinderMock.Object);
        }
        
   /*     [Test]
        public void Index_returns_incomplete_tickets_that_are_ordered_by_created_at()
        {
            // Arrange
            var ticket1 = _fixture.Create<Ticket>();
            ticket1.SetProperty(t => t.Category, _fixture.Create<Category>());
            var ticket2 = _fixture.Create<Ticket>();
            ticket2.SetProperty(t => t.Category, _fixture.Create<Category>());
            var tickets = new[]
            {
                ticket1,
                ticket2,
            };
            
            _ticketsFinderMock.Setup(r => r.FindAllWithStatus(false))
                .Returns(tickets);
            
            // Act
            var result = (ViewResult)_controller.Index(searchString: null);
            
            // Assert
            var expectedViewModels = new[]
            {
                new TicketListViewModel
                {
                    Id = ticket1.Id,
                    Title = ticket1.Title,
                    Category = ticket1.Category.Name
                },
                new TicketListViewModel
                {
                    Id = ticket2.Id,
                    Title = ticket2.Title,
                    Category = ticket2.Category.Name
                }
            };
            result.Model.Should().BeEquivalentTo(expectedViewModels);
        }
        */
        [Test]
        public void Index_returns_Complete_tickets_that_are_ordered_by_created_at()
        {
            // Arrange
            var ticket1 = _fixture.Create<Ticket>();
            ticket1.SetProperty(t => t.Category, _fixture.Create<Category>());
            var ticket2 = _fixture.Create<Ticket>();
            ticket2.SetProperty(t => t.Category, _fixture.Create<Category>());
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
                    Title = ticket1.Title,
                    Category = ticket1.Category.Name,
                },
                new TicketListViewModel
                {
                    Id = ticket2.Id,
                    Title = ticket2.Title,
                    Category = ticket2.Category.Name
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
            ticket.SetProperty(t => t.Category, _fixture.Create<Category>());
            
            var comment1 = _fixture.Create<Comment>();
            ticket.AddComment(comment1);
            var comment2 = _fixture.Create<Comment>();
            ticket.AddComment(comment2);
            
            _ticketsFinderMock.Setup(r => r.Find(ticketId))
                .Returns(ticket);
            
            // Act
            var result = (ViewResult)_controller.Details(ticketId);
            
            // Assert
            var viewModel = (TicketDetailsViewModel)result.Model;
            viewModel.Id.Should().Be(ticket.Id);
            viewModel.Title.Should().Be(ticket.Title);
            viewModel.CreatedAt.Should().Be(ticket.CreatedAt);
            viewModel.IsCompleted.Should().Be(ticket.IsCompleted);
            viewModel.CompletedAt.Should().Be(ticket.CompletedAt);
            viewModel.Description.Should().Be(ticket.Description);
            viewModel.Category.Should().Be(ticket.Category.Name);
            viewModel.Comments[0].Content.Should().Be(comment1.Content);
            viewModel.Comments[1].Content.Should().Be(comment2.Content);
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
            createdTicket.CategoryId.Should().Be(model.Category);
            
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
