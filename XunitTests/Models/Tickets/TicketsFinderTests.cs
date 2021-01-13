using System.Linq;
using AutoFixture;
using Castle.Core.Internal;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SupportApp.Data;
using SupportApp.Models.Tickets;

namespace XunitTests.Models.Tickets
{
    public class TicketsFinderTests
    {
        [Test]
        public void FindAllByStatus_Returns_All_Tickets_With_Given_Status()
        {
            var fixture = new Fixture();
            var contextMock = new Mock<ISupportAppContext>(); 
            var sut = new TicketsFinder(contextMock.Object);
            
            var status = 5678;
            var expectedTicket1 = fixture.Create<Ticket>();
            expectedTicket1.Status = status;
            var expectedTicket2 = fixture.Create<Ticket>();
            expectedTicket2.Status = status;
            
            var tickets = new[]
            {
                fixture.Create<Ticket>(),
                expectedTicket1,
                fixture.Create<Ticket>(),
                fixture.Create<Ticket>(),
                expectedTicket2
            };
            contextMock.Setup(c => c.Queryable<Ticket>())
                .Returns(tickets.AsQueryable());
            
            //act
            var result = sut.FindAllWithStatus(status);
            
            //assert
            result.Should().BeEquivalentTo(expectedTicket1, expectedTicket2);

        }

        [Test]
        public void Find_Returns_A_Ticket_With_The_Same_Id()
        {
            var fixture = new Fixture();
            var contextMock = new Mock<ISupportAppContext>(); 
            var sut = new TicketsFinder(contextMock.Object);
            
            var expectedTicket = fixture.Create<Ticket>();
            
            contextMock.Setup(c => c.FindById<Ticket>(22))
                .Returns(expectedTicket);
            
            //act
            var result = sut.Find(22);
            
            //assert
            result.Should().Be(expectedTicket);
        }
    }
}