using System.Linq;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SupportApp.Data;
using SupportApp.Models.Categories;


namespace XunitTests.Models.Categories
{
    public class CategoryFinderTests
    {
        [Test]
        public void Find_Returns_A_Category_With_The_Same_Id()
        {
            var fixture = new Fixture();
            var contextMock = new Mock<ISupportAppContext>(); 
            var sut = new CategoryFinder(contextMock.Object);
            
            var expectedCategory = fixture.Create<Category>();
            
            contextMock.Setup(c => c.FindById<Category>(22))
                .Returns(expectedCategory);
            
            //act
            var result = sut.Find(22);
            
            //assert
            result.Should().Be(expectedCategory);
        }
        [Test]
        public void FindAll_Returns_All_Categories()
        {
            var fixture = new Fixture();
            var contextMock = new Mock<ISupportAppContext>(); 
            var sut = new CategoryFinder(contextMock.Object);
            
            var expectedTicket1 = new Category("test1");
            var expectedTicket2 = new Category("test2");
            var expectedTicket3 = new Category("test3");

            var categories = new[]
            {
                expectedTicket1,
                expectedTicket2,
                expectedTicket3,
            };
            contextMock.Setup(c => c.Queryable<Category>())
                .Returns(categories.AsQueryable());
            
            //act
            var result = sut.FindAll();
            
            //assert
            result.Should().BeEquivalentTo(expectedTicket1, expectedTicket2, expectedTicket3);

        }

        [Test]
        public void Find_Returns_Category_With_Given_Id()
        {

            var fixture = new Fixture();
            var contextMock = new Mock<ISupportAppContext>(); 
            var sut = new CategoryFinder(contextMock.Object);
            
            var expectedTicket = fixture.Create<Category>();
            
            contextMock.Setup(c => c.FindById<Category>(22))
                .Returns(expectedTicket);
            
            //act
            var result = sut.Find(22);
            
            //assert
            result.Should().Be(expectedTicket);
        }
    }
}