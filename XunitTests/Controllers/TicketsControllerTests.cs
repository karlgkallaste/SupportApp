using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SupportApp.Areas.Identity.Data;
using SupportApp.Controllers;
using SupportApp.Models.Categories;
using SupportApp.Models.Comments;
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
        private Mock<ICategoryFinder> _categoryFinderMock;

        private ClaimsPrincipal _user;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _ticketsFinderMock = new Mock<ITicketsFinder>();
            _ticketsModifierMock = new Mock<ITicketsModifier>();
            _categoryFinderMock = new Mock<ICategoryFinder>();
            _controller = new TicketsController(_ticketsFinderMock.Object, _ticketsModifierMock.Object,
                _categoryFinderMock.Object);
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
        public void Index_returns_incomplete_tickets_that_are_ordered_by_created_at_for_admin([Values]bool isAdmin)
        {
            var userId = Guid.NewGuid();
            // Arrange
            _user.AddIdentity(new ClaimsIdentity(new []
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                isAdmin 
                    ? new Claim(ClaimTypes.Role, "Admin")
                    : new Claim(ClaimTypes.Role, "User")
            }));
            
            var ticket1 = _fixture.Create<Ticket>();
            ticket1.SetProperty(t => t.Category, _fixture.Create<Category>());
            var ticket2 = _fixture.Create<Ticket>();
            ticket2.SetProperty(t => t.Category, _fixture.Create<Category>());
            var tickets = new[]
            {
                ticket1,
                ticket2,
            }.ToList();

            var searchString = "asdasd search";

            if (isAdmin)
            {
                _ticketsFinderMock.Setup(r => r.FindAllWithStatus(false, searchString))
                    .Returns(tickets);
            }
            else
            {
                _ticketsFinderMock.Setup(r => r.FindAllByAuthor(userId,false, searchString))
                    .Returns(tickets);
            }
            

            // Act
            var result = (ViewResult) _controller.Index(searchString);

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

        [Test]
        public void Completed_returns_Complete_tickets_that_are_ordered_by_created_at([Values] bool isAdmin)
        {
            var userId = Guid.NewGuid();
            // Arrange
            _user.AddIdentity(new ClaimsIdentity(new []
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                isAdmin 
                    ? new Claim(ClaimTypes.Role, "Admin")
                    : new Claim(ClaimTypes.Role, "User")
            }));
            
            var ticket1 = _fixture.Create<Ticket>();
            ticket1.SetProperty(t => t.Category, _fixture.Create<Category>());
            var ticket2 = _fixture.Create<Ticket>();
            ticket2.SetProperty(t => t.Category, _fixture.Create<Category>());
            var tickets = new[]
            {
                ticket1,
                ticket2,
            }.ToList();

            var searchString = "asdasd search";

            if (isAdmin)
            {
                _ticketsFinderMock.Setup(r => r.FindAllWithStatus(true, searchString))
                    .Returns(tickets);
            }
            else
            {
                _ticketsFinderMock.Setup(r => r.FindAllByAuthor(userId,true, searchString))
                    .Returns(tickets);
            }
            

            // Act
            var result = (ViewResult) _controller.Completed(searchString);

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

        [Test]
        public void Details_returns_notfound_when_ticket_is_not_found_by_id()
        {
            // Arrange
            var id = 2;

            _ticketsFinderMock.Setup(r => r.Find(id))
                .Returns((Ticket) null);
            // Act
            var result = _controller.Details(id);


            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public void Details_returns_view_with_ticket_by_id()
        {
            var userId = Guid.NewGuid();
            _user.AddIdentity(new ClaimsIdentity(new []
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()), new Claim(ClaimTypes.Role, "User")
            }));
            // Arrange
            var ticket = _fixture.Create<Ticket>();
            ticket.SetProperty(t => t.Category, _fixture.Create<Category>());
            ticket.SetProperty(t => t.Author, userId);

            var comment1 = _fixture.Create<Comment>();
            ticket.AddComment(comment1);
            var comment2 = _fixture.Create<Comment>();
            ticket.AddComment(comment2);

            _ticketsFinderMock.Setup(r => r.Find(ticket.Id))
                .Returns(ticket);

            // Act
            var result = (ViewResult) _controller.Details(ticket.Id);

            // Assert
            var viewModel = (TicketDetailsViewModel) result.Model;
            viewModel.Id.Should().Be(ticket.Id);
            viewModel.Title.Should().Be(ticket.Title);
            viewModel.IsCompleted.Should().Be(ticket.IsCompleted);
            viewModel.CompletedAt.Should().Be(ticket.CompletedAt);
            viewModel.Description.Should().Be(ticket.Description);
            viewModel.Category.Should().Be(ticket.Category.Name);
            viewModel.Comments[0].Content.Should().Be(comment1.Content);
            viewModel.Comments[1].Content.Should().Be(comment2.Content);
        }
        [Test]
        public void Details_returns_Forbidden_If_User_Is_Not_Author_or_Admin()
        {
            var userId = Guid.NewGuid();
            _user.AddIdentity(new ClaimsIdentity(new []
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()), new Claim(ClaimTypes.Role, "User")
            }));
            var ticket = _fixture.Create<Ticket>();
            // Arrange

            _ticketsFinderMock.Setup(r => r.Find(ticket.Id))
                .Returns(ticket);
            // Act
            var result = _controller.Details(ticket.Id);


            // Assert
            result.Should().BeOfType<ForbidResult>();
        }
        [Test]
        public void Details_returns_view_If_User_Is_Not_Author_But_Is_Admin()
        {
            var userId = Guid.NewGuid();
            _user.AddIdentity(new ClaimsIdentity(new []
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()), new Claim(ClaimTypes.Role, "Admin")
            }));
            // Arrange
            var ticket = _fixture.Create<Ticket>();
            ticket.SetProperty(t => t.Category, _fixture.Create<Category>());
            var comment1 = _fixture.Create<Comment>();
            ticket.AddComment(comment1);
            var comment2 = _fixture.Create<Comment>();
            ticket.AddComment(comment2);

            _ticketsFinderMock.Setup(r => r.Find(ticket.Id))
                .Returns(ticket);

            // Act
            var result = (ViewResult) _controller.Details(ticket.Id);

            // Assert
            var viewModel = (TicketDetailsViewModel) result.Model;
            viewModel.Id.Should().Be(ticket.Id);
            viewModel.Title.Should().Be(ticket.Title);
            viewModel.IsCompleted.Should().Be(ticket.IsCompleted);
            viewModel.CompletedAt.Should().Be(ticket.CompletedAt);
            viewModel.Description.Should().Be(ticket.Description);
            viewModel.Category.Should().Be(ticket.Category.Name);
            viewModel.Comments[0].Content.Should().Be(comment1.Content);
            viewModel.Comments[1].Content.Should().Be(comment2.Content);
        }

        [Test]
        public void Create_adds_new_ticket()
        {
            var userId = Guid.NewGuid();
            _user.AddIdentity(new ClaimsIdentity(new []
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()), new Claim(ClaimTypes.Role, "User")
            }));
            // Arrange
            var model = _fixture.Create<CreateTicketModel>();

            Ticket createdTicket = null;
            _ticketsModifierMock.Setup(r => r.Add(It.IsAny<Ticket>()))
                .Callback<Ticket>(t => createdTicket = t);

            // Act
            var result = (RedirectToActionResult) _controller.Create(model);

            // Assert
            createdTicket.Title.Should().Be(model.Title);
            createdTicket.Description.Should().Be(model.Description);
            createdTicket.Author.Should().Be(userId);
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
            var result = (RedirectToActionResult) _controller.DeleteConfirmed(ticketId);

            // Assert
            _ticketsModifierMock.Verify(r => r.RemoveTicket(ticketId), Times.Once);
            result.ActionName.Should().Be(nameof(TicketsController.Index));
        }

        [Test]
        public void DeleteConfirmed_redirects_to_index_if_ticket_doesnt_exist()
        {
            // Arrange

            _ticketsFinderMock.Setup(r => r.Find(2))
                .Returns((Ticket) null);
            // Act
            var result = (RedirectToActionResult) _controller.DeleteConfirmed(2);

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
            var result = (RedirectToActionResult) _controller.MarkDone(ticket.Id);
            //assert
            result.ActionName.Should().Be(nameof(TicketsController.Index));
            ticket.CompletedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(200));
            ticket.IsCompleted.Should().BeTrue();
            _ticketsModifierMock.Verify(m => m.UpdateTicket(ticket), Times.Once);
        }

        
        [Test]
        public void Edit_returns_notfound_when_ticket_is_not_found()
        {
            // Arrange
            var id = 2;

            _ticketsFinderMock.Setup(r => r.Find(id))
                .Returns((Ticket) null);
            // Act
            var result = _controller.Edit(id);


            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public void Edit_Changes_and_Saves_New_Ticket_Values()
        {
            var userId = Guid.NewGuid();
            _user.AddIdentity(new ClaimsIdentity(new []
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()), new Claim(ClaimTypes.Role, "User")
            }));
            
            var model = _fixture.Create<TicketEditViewModel>();
            model.Title = "xd";
            model.Description = "NewDesc";

            var ticket = _fixture.Create<Ticket>();
            ticket.SetProperty(t => t.Author, userId);
            ticket.SetProperty(t => t.Title, "yo");

            _ticketsFinderMock.Setup(r => r.Find(model.Id))
                .Returns(ticket);

            // act
            var result = (RedirectToActionResult) _controller.Edit(model);

            // assert
            ticket.Title.Should().BeEquivalentTo("xd");
            ticket.Description.Should().BeEquivalentTo("NewDesc");
            _ticketsModifierMock.Verify(m => m.UpdateTicket(ticket), Times.Once);
        }
        [Test]
        public void Edit_returns_Forbidden_If_User_Is_Not_Author_or_Admin()
        {
            var userId = Guid.NewGuid();
            _user.AddIdentity(new ClaimsIdentity(new []
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()), new Claim(ClaimTypes.Role, "User")
            }));
            var ticket = _fixture.Create<Ticket>();
            // Arrange

            _ticketsFinderMock.Setup(r => r.Find(ticket.Id))
                .Returns(ticket);
            // Act
            var result = _controller.Edit(ticket.Id);


            // Assert
            result.Should().BeOfType<ForbidResult>();
        }
        [Test]
        public void Delete_returns_Forbidden_If_User_Is_Not_Author_Or_Admin()
        {
            var userId = Guid.NewGuid();
            _user.AddIdentity(new ClaimsIdentity(new []
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()), new Claim(ClaimTypes.Role, "User")
            }));
            var ticket = _fixture.Create<Ticket>();
            // Arrange

            _ticketsFinderMock.Setup(r => r.Find(ticket.Id))
                .Returns(ticket);
            // Act
            var result = _controller.Delete(ticket.Id);


            // Assert
            result.Should().BeOfType<ForbidResult>();
        }
        [Test]
        public void Edit_Marks_Ticket_As_Done()
        {
            var userId = Guid.NewGuid();
            _user.AddIdentity(new ClaimsIdentity(new []
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()), new Claim(ClaimTypes.Role, "User")
            }));
            
            var model = _fixture.Create<TicketEditViewModel>();
            model.IsCompleted = true;

            var ticket = _fixture.Create<Ticket>();
            ticket.SetProperty(t => t.Author, userId);
            ticket.MarkUndone();

            _ticketsFinderMock.Setup(r => r.Find(model.Id))
                .Returns(ticket);

            // act
            var result = (RedirectToActionResult) _controller.Edit(model);

            // assert
            ticket.IsCompleted.Should().BeTrue();
            _ticketsModifierMock.Verify(m => m.UpdateTicket(ticket), Times.Once);
        }

        [Test]
        public void Edit_Marks_Ticket_As_Undone()
        {
            var userId = Guid.NewGuid();
            _user.AddIdentity(new ClaimsIdentity(new []
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()), new Claim(ClaimTypes.Role, "User")
            }));
            var model = _fixture.Create<TicketEditViewModel>();
            model.IsCompleted = false;

            var ticket = _fixture.Create<Ticket>();
            ticket.SetProperty(t => t.Author, userId);
            ticket.MarkUndone();

            _ticketsFinderMock.Setup(r => r.Find(model.Id))
                .Returns(ticket);

            // act
            var result = (RedirectToActionResult) _controller.Edit(model);

            // assert
            ticket.IsCompleted.Should().BeFalse();
            _ticketsModifierMock.Verify(m => m.UpdateTicket(ticket), Times.Once);
        }

        [Test]
        public void OverdueTickets_is_authorized_with_Admin_role()
        {
            _controller.AssertActionIsAuthorized("Admin");
        }
        [Test]
        public void Completed_returns_complete_tickets_that_are_ordered_by_created_at_for_admin([Values]bool isAdmin)
        {
            var userId = Guid.NewGuid();
            // Arrange
            _user.AddIdentity(new ClaimsIdentity(new []
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                isAdmin 
                    ? new Claim(ClaimTypes.Role, "Admin")
                    : new Claim(ClaimTypes.Role, "User")
            }));
            
            var ticket1 = _fixture.Create<Ticket>();
            ticket1.SetProperty(t => t.Category, _fixture.Create<Category>());
            var ticket2 = _fixture.Create<Ticket>();
            ticket2.SetProperty(t => t.Category, _fixture.Create<Category>());
            var tickets = new[]
            {
                ticket1,
                ticket2,
            }.ToList();

            var searchString = "asdasd search";

            if (isAdmin)
            {
                _ticketsFinderMock.Setup(r => r.FindAllWithStatus(true, searchString))
                    .Returns(tickets);
            }
            else
            {
                _ticketsFinderMock.Setup(r => r.FindAllByAuthor(userId,true, searchString))
                    .Returns(tickets);
            }
            

            // Act
            var result = (ViewResult) _controller.Completed(searchString);

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
        
        [Test]
        public void OverDueTickets_Shows_Tickets_That_Have_Past_Deadline_Only_To_Admin_Role()
        {
            var userId = Guid.NewGuid();
            // Arrange
            _user.AddIdentity(new ClaimsIdentity(new []
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, "Admin")
                
            }));
            
            var ticket1 = _fixture.Create<Ticket>();
            ticket1.SetProperty(t => t.Category, _fixture.Create<Category>());
            var ticket2 = _fixture.Create<Ticket>();
            ticket2.SetProperty(t => t.Category, _fixture.Create<Category>());
            var tickets = new[]
            {
                ticket1,
                ticket2,
            }.ToList();
            
                _ticketsFinderMock.Setup(r => r.FindAllByOverDueTickets(false))
                    .Returns(tickets);

            

            // Act
            var result = (ViewResult) _controller.OverdueTickets();


            // Assert
            var expectedViewModels = new[]
            {
                new OverDueTicketListViewModel
                {
                    Id = ticket1.Id,
                    Title = ticket1.Title,
                    Category = ticket1.Category.Name,
                    Deadline = ticket1.Deadline,
                    Hourspast = (int) DateTime.Now.Subtract(ticket1.Deadline).TotalHours

                },
                new OverDueTicketListViewModel
                {
                    Id = ticket2.Id,
                    Title = ticket2.Title,
                    Category = ticket2.Category.Name,
                    Deadline = ticket2.Deadline,
                    Hourspast = (int) DateTime.Now.Subtract(ticket2.Deadline).TotalHours
                }
            };
            result.Model.Should().BeEquivalentTo(expectedViewModels);
        }
    }
}