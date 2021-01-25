using System.Linq;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SupportApp.Data;
using SupportApp.Models.Categories;

namespace XunitTests.Models.Categorys
{
    public class CategorysModifierTests
    {
        [Test]
        public void Add_Inserts_New_Category_To_DataBase()
        {
            var fixture = new Fixture();
            var contextMock = new Mock<ISupportAppContext>();
            var categorySetMock = new Mock<DbSet<Category>>();
            contextMock.Setup(c => c.Set<Category>())
                .Returns(categorySetMock.Object);

            var sut = new CategoryModifier(contextMock.Object);


            var newCategory = fixture.Create<Category>();

            //act
            var result = sut.Add(newCategory);

            //assert
            result.Should().Be(newCategory);
            categorySetMock.Verify(c => c.Add(newCategory), Times.Once);
            contextMock.Verify(c => c.SaveChanges());
        }
        [Test]
        public void RemoveCategory_Removes_Category_From_Database_By_Id()
        {
            var fixture = new Fixture();
            var contextMock = new Mock<ISupportAppContext>();
            
            var categorySetMock = new Mock<DbSet<Category>>();
            contextMock.Setup(c => c.Set<Category>())
                .Returns(categorySetMock.Object);
            
            var sut = new CategoryModifier(contextMock.Object);
            var category = fixture.Create<Category>();
            
            contextMock.Setup(c => c.FindById<Category>(category.Id))
                .Returns(category);

            //act
            sut.RemoveCategory(category.Id);
            
            categorySetMock.Verify(c=>c.Remove(category));
            contextMock.Verify(c=>c.SaveChanges());
            
            
        }
        [Test]
        public void Remove_Does_Nothing_If_Category_With_Given_Id_Is_Not_Found()
        {
            var contextMock = new Mock<ISupportAppContext>();
            
            var categorySetMock = new Mock<DbSet<Category>>();
            contextMock.Setup(c => c.Set<Category>())
                .Returns(categorySetMock.Object);
            
            var sut = new CategoryModifier(contextMock.Object);

            contextMock.Setup(c => c.FindById<Category>(22))
                .Returns((Category)null);

            //act
            sut.RemoveCategory(22);
            
            categorySetMock.Verify(c=>c.Remove(It.IsAny<Category>()), Times.Never);
            contextMock.Verify(c=>c.SaveChanges(), Times.Never);
            
            
        }
        [Test]
        public void Remove_Removes_Category_From_Database_By_Id()
        {
            var fixture = new Fixture();
            var contextMock = new Mock<ISupportAppContext>();
            
            var categorySetMock = new Mock<DbSet<Category>>();
            contextMock.Setup(c => c.Set<Category>())
                .Returns(categorySetMock.Object);
            
            var sut = new CategoryModifier(contextMock.Object);
            var category = fixture.Create<Category>();
            
            contextMock.Setup(c => c.FindById<Category>(category.Id))
                .Returns(category);

            //act
            sut.RemoveCategory(category.Id);
            
            categorySetMock.Verify(c=>c.Remove(category));
            contextMock.Verify(c=>c.SaveChanges());
            
            
        }
    }
}