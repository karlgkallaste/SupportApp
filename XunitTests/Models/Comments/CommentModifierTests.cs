using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SupportApp.Data;
using SupportApp.Models.Categories;
using SupportApp.Models.Comments;

namespace XunitTests.Models.Comments
{
    public class CommentModifierTests
    {
        [Test]
        public void RemoveComment_Removes_Comment_From_Database_By_Id()
        {
            var fixture = new Fixture();
            var contextMock = new Mock<ISupportAppContext>();
            
            var commentSetMock = new Mock<DbSet<Comment>>();
            contextMock.Setup(c => c.Set<Comment>())
                .Returns(commentSetMock.Object);
            
            var sut = new CommentModifier(contextMock.Object);
            var comment = fixture.Create<Comment>();
            
            contextMock.Setup(c => c.FindById<Comment>(comment.Id))
                .Returns(comment);

            //act
            sut.RemoveComment(comment.Id);
            
            commentSetMock.Verify(c=>c.Remove(comment));
            contextMock.Verify(c=>c.SaveChanges());
            
            
        }
        [Test]
        public void Add_Inserts_New_Comment_To_DataBase()
        {
            var fixture = new Fixture();
            var contextMock = new Mock<ISupportAppContext>();
            var commentSetMock = new Mock<DbSet<Comment>>();
            contextMock.Setup(c => c.Set<Comment>())
                .Returns(commentSetMock.Object);

            var sut = new CommentModifier(contextMock.Object);


            var newComment = fixture.Create<Comment>();

            //act
            var result = sut.Add(newComment);

            //assert
            result.Should().Be(newComment);
            commentSetMock.Verify(c => c.Add(newComment), Times.Once);
            contextMock.Verify(c => c.SaveChanges());
        }
    }
}