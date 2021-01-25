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
        public void FindAllByStatus_Returns_All_Tickets_With_Given_Status([Values]bool completed)
        {
            var fixture = new Fixture();
            var contextMock = new Mock<ISupportAppContext>(); 
            var sut = new TicketsFinder(contextMock.Object);
            
            var expectedTicket1 = new Ticket("test1","title1","author1", DateTime.Now.AddSeconds(2), 1);
            expectedTicket1.MarkDone();
            var expectedTicket2 = new Ticket("test2","title2","author2", DateTime.Now.AddSeconds(3), 1);
            expectedTicket2.MarkDone();
            var expectedTicket3 = new Ticket("test3","title3","author3", DateTime.Now.AddSeconds(4),1);
            expectedTicket3.MarkDone();
            var notExpectedTicket = new Ticket("test4","title4","author4", DateTime.Now.AddSeconds(43), 1);
            notExpectedTicket.MarkUndone();
            
            var tickets = new[]
            {
                expectedTicket2,
                expectedTicket3,
                notExpectedTicket,
                expectedTicket1,
            };
            contextMock.Setup(c => c.Queryable<Ticket>())
                .Returns(tickets.AsQueryable());
            
            //act
            var result = sut.FindAllWithStatus(true);
            
            //assert
            result.Should().BeEquivalentTo(new []{expectedTicket1, expectedTicket2, expectedTicket3}, 
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