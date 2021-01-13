using System.Linq;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SupportApp.Data;
using SupportApp.Models.Tickets;

namespace XunitTests.Models.Tickets
{
    public class TicketsModifierTests
    {
        [Test]
        public void Add_Inserts_New_Ticket_To_DataBase()
        {
            var fixture = new Fixture();
            var contextMock = new Mock<ISupportAppContext>();
            var ticketSetMock = new Mock<DbSet<Ticket>>();
            contextMock.Setup(c => c.Set<Ticket>())
                .Returns(ticketSetMock.Object);
            
            var sut = new TicketsModifier(contextMock.Object);
            
            
            var newTicket = fixture.Create<Ticket>();
            
            //act
            var result = sut.Add(newTicket);
            
            //assert
            result.Should().Be(newTicket);
            ticketSetMock.Verify(c=>c.Add(newTicket), Times.Once);
            contextMock.Verify(c=>c.SaveChanges());
        }

        [Test]
        public void RemoveTicket_Removes_Ticket_From_Database_By_Id()
        {
            var fixture = new Fixture();
            var contextMock = new Mock<ISupportAppContext>();
            
            var ticketSetMock = new Mock<DbSet<Ticket>>();
            contextMock.Setup(c => c.Set<Ticket>())
                .Returns(ticketSetMock.Object);
            
            var sut = new TicketsModifier(contextMock.Object);
            var ticket = fixture.Create<Ticket>();
            
            contextMock.Setup(c => c.FindById<Ticket>(ticket.Id))
                .Returns(ticket);

            //act
            sut.RemoveTicket(ticket.Id);
            
            ticketSetMock.Verify(c=>c.Remove(ticket));
            contextMock.Verify(c=>c.SaveChanges());
            
            
        }

        [Test]
        public void RemoveTicket_Does_Nothing_When_Ticket_Not_Found()
        {
            var contextMock = new Mock<ISupportAppContext>();
            
            var ticketSetMock = new Mock<DbSet<Ticket>>();
            contextMock.Setup(c => c.Set<Ticket>())
                .Returns(ticketSetMock.Object);
            
            var sut = new TicketsModifier(contextMock.Object);

            contextMock.Setup(c => c.FindById<Ticket>(232))
                .Returns((Ticket)null);

            //act
            sut.RemoveTicket(232);
            
            ticketSetMock.Verify(c=>c.Remove(It.IsAny<Ticket>()), Times.Never);
            contextMock.Verify(c=>c.SaveChanges(), Times.Never);
        }

        [Test]
        public void EditTicket_Changes_The_Contents_And_Saves_To_Database()
        {
            var fixture = new Fixture();
            var contextMock = new Mock<ISupportAppContext>();
            var ticketSetMock = new Mock<DbSet<Ticket>>();
            contextMock.Setup(c => c.Set<Ticket>())
                .Returns(ticketSetMock.Object);
            
            var sut = new TicketsModifier(contextMock.Object);
            
            
            var newTicket = fixture.Create<Ticket>();
            
            //act
            var result = sut.EditTicket(newTicket);
            
            //assert
            result.Should().Be(newTicket);
            ticketSetMock.Verify(c=>c.Update(newTicket), Times.Once);
            contextMock.Verify(c=>c.SaveChanges());
            
        }

        [Test]
        public void RemoveAllBy_Removes_All_Tickets_By_Author()
        {
            {
                var fixture = new Fixture();
                var contextMock = new Mock<ISupportAppContext>();
            
                var ticketSetMock = new Mock<DbSet<Ticket>>();
                contextMock.Setup(c => c.Set<Ticket>())
                    .Returns(ticketSetMock.Object);
            
                var sut = new TicketsModifier(contextMock.Object);
            
                var author = "KGK";
                var ticket1 = fixture.Create<Ticket>();
                ticket1.Author = author;
                var ticket2 = fixture.Create<Ticket>();
                ticket2.Author = author;
            
                var tickets = new[]
                {
                    fixture.Create<Ticket>(),
                    ticket1,
                    fixture.Create<Ticket>(),
                    fixture.Create<Ticket>(),
                    ticket2
                };
                contextMock.Setup(c => c.Queryable<Ticket>())
                    .Returns(tickets.AsQueryable());
            
                //act
                sut.RemoveAllByAuthor(author);
            
                //assert
                ticketSetMock.Verify(c=>c.RemoveRange(
                    It.Is<Ticket[]>(a => a.AssertIsEquivalent(new []{ticket1, ticket2}))));
                contextMock.Verify(c=>c.SaveChanges());
            }
        }
    }
}