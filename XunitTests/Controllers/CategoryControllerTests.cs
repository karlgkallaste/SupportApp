using System.Linq;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SupportApp.Controllers;
using SupportApp.Models.Categories;
using SupportApp.ViewModels.Categories;
using SupportApp.ViewModels.Tickets;

namespace XunitTests.Controllers
{
    public class CategoryControllerTests
    {
        private CategoryController _controller;
        private Fixture _fixture;
        private Mock<ICategoryFinder> _categoryFinderMock;
        private Mock<ICategoryModifier> _categoryModifierMock;


        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _categoryFinderMock = new Mock<ICategoryFinder>();
            _categoryModifierMock = new Mock<ICategoryModifier>();
            _controller = new CategoryController(_categoryModifierMock.Object, _categoryFinderMock.Object);
        }

       [Test]
        public void Index_returns_categories_and_ids()
        {
            // Arrange
            var category1 = _fixture.Create<Category>();
            var category2 = _fixture.Create<Category>();
            var categories = new[]
            {
                category1,
                category2,
            }.ToList();
            
            _categoryFinderMock.Setup(r => r.FindAll())
                .Returns(categories);
            
            // Act
            var result = (ViewResult)_controller.Index();
            
            // Assert
            var expectedViewModels = new[]
            {
                new CategoryListViewModel
                {
                    Id = category1.Id,
                    Name = category1.Name
                },
                new CategoryListViewModel
                {
                    Id = category1.Id,
                    Name = category2.Name
                }
            };
            result.Model.Should().BeEquivalentTo(expectedViewModels);
        }
        [Test]
        public void Create_new_category_gets_added()
        {
            // Arrange
            var model = _fixture.Create<CreateCategoryModel>();

            Category createdCategory = null;
            _categoryModifierMock.Setup(r => r.Add(It.IsAny<Category>()))
                .Callback<Category>(t => createdCategory = t);
            
            // Act
            var result = (RedirectToActionResult)_controller.Create(model);
            
            // Assert
            createdCategory.Name.Should().Be(model.Name);

            result.ActionName.Should().Be(nameof(CategoryController.Index));
        }

        [Test]
        public void DeleteConfirmed_ticket_gets_deleted_by_id_and_user_gets_redirected()
        {
            // Arrange
            var categoryId = 1;

            // Act
            var result = (RedirectToActionResult)_controller.DeleteConfirmed(categoryId);
            
            // Assert
            _categoryModifierMock.Verify(r => r.RemoveCategory(categoryId), Times.Once);
            result.ActionName.Should().Be(nameof(TicketsController.Index));
        }
        [Test]
        public void DeleteConfirmed_redirects_to_index_if_ticket_doesnt_exist()
        {
            // Arrange

            _categoryFinderMock.Setup(r => r.Find(2))
                .Returns((Category)null);
            // Act
            var result = (RedirectToActionResult)_controller.DeleteConfirmed(2);
            
            // Assert
            result.ActionName.Should().Be(nameof(TicketsController.Index));
        }

    }
}