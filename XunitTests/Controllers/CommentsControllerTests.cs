using System.Linq;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SupportApp.Controllers;
using SupportApp.Models.Categories;
using SupportApp.Models.Comments;
using SupportApp.Models.Tickets;
using SupportApp.ViewModels.Categories;
using SupportApp.ViewModels.Comments;

namespace XunitTests.Controllers
{
    public class CommentsControllerTests
    {
        private CommentController _controller;
        private Fixture _fixture;
        private Mock<ICommentModifier> _commentModifierMock;
        private Mock<ICommentFinder> _commentFinderMock;
        private Mock<ITicketsFinder> _ticketFinderMock;
        private Mock<ITicketsModifier> _ticketModifierMock;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _commentFinderMock = new Mock<ICommentFinder>();
            _commentModifierMock = new Mock<ICommentModifier>();
            _ticketFinderMock = new Mock<ITicketsFinder>();
            _ticketModifierMock = new Mock<ITicketsModifier>();
            _controller = new CommentController(_commentModifierMock.Object, _commentFinderMock.Object, _ticketFinderMock.Object, _ticketModifierMock.Object);
        }
        
        [Test]
        public void Create_adds_new_comment_to_given_ticket()
        {
            // Arrange
            var model = _fixture.Create<CreateCommentModel>();
            
            var ticket = _fixture.Create<Ticket>();
            _ticketFinderMock.Setup(f => f.Find(model.TicketId))
                .Returns(ticket);
            
            // Act
            var result = (RedirectToActionResult)_controller.Create(model);

            ticket.GetComments().Single().Should().BeEquivalentTo(model.ToDomainObject());
            _ticketModifierMock.Verify(m => m.UpdateTicket(ticket), Times.Once);
            
            result.ActionName.Should().Be(nameof(TicketsController.Details));
            result.ControllerName.Should().Be("Tickets");
            result.RouteValues["Id"].Should().Be(model.TicketId);
        }

        [Test]
        public void DeleteConfirmed_Removes_Comment_By_Id_And_Redirects_To_Tickets_Index()
        {
            // Arrange
            var model = _fixture.Create<CreateCommentModel>();
            model.TicketId = 1;

            // Act
            var result = (RedirectToActionResult)_controller.DeleteConfirmed(model.TicketId);
            
            // Assert
            _commentModifierMock.Verify(r => r.RemoveComment(model.TicketId), Times.Once);
            result.ActionName.Should().Be(nameof(TicketsController.Index));
        }
    }
}