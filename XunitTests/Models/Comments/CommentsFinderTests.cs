using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SupportApp.Data;
using SupportApp.Models.Categories;
using SupportApp.Models.Comments;
using SupportApp.ViewModels.Comments;

namespace XunitTests.Models.Comments
{
    public class CommentsFinderTests
    {
        [Test]
        public void Find_Returns_A_Comment_With_The_Same_Id()
        {
            var fixture = new Fixture();
            var contextMock = new Mock<ISupportAppContext>(); 
            var sut = new CommentFinder(contextMock.Object);
            
            var expectedCategory = fixture.Create<Comment>();
            
            contextMock.Setup(c => c.FindById<Comment>(22))
                .Returns(expectedCategory);
            
            //act
            var result = sut.Find(22);
            
            //assert
            result.Should().Be(expectedCategory);
        }
        
    }
}