using System;
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
            expectedTicket1.CreatedAt =  DateTime.Now.AddSeconds(-2);
            var expectedTicket2 = fixture.Create<Ticket>();
            expectedTicket2.Status = status;
            expectedTicket2.CreatedAt = DateTime.Now.AddSeconds(-5);
            var expectedTicket3 = fixture.Create<Ticket>();
            expectedTicket3.Status = status;
            expectedTicket3.CreatedAt = DateTime.Now.AddSeconds(-9);
            
            
            var tickets = new[]
            {
                fixture.Create<Ticket>(),
                expectedTicket2,
                expectedTicket3,
                fixture.Create<Ticket>(),
                fixture.Create<Ticket>(),
                expectedTicket1
            };
            contextMock.Setup(c => c.Queryable<Ticket>())
                .Returns(tickets.AsQueryable());
            
            //act
            var result = sut.FindAllWithStatus(status);
            
            //assert
            result.Should().BeEquivalentTo(new []{expectedTicket3, expectedTicket2, expectedTicket1}, 
                conf => conf.WithStrictOrdering() );

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