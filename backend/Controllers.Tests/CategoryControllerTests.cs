using COURSES.API.Controllers;
using IBL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.DTO;
using Model;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controllers.Tests
{
    public class CategoryControllerTests
    {
        private readonly Mock<ICategoryService> _mockCategoryService;
        private readonly CategoryController _controller;

        public CategoryControllerTests()
        {
            _mockCategoryService = new Mock<ICategoryService>();
            _controller = new CategoryController(_mockCategoryService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        }

        [Fact]
        public async Task GetAllCategories_ReturnsOkWithCategories()
        {
            var categories = new List<Category>
                {
                    new Category { Id = Guid.NewGuid(), Name = "Category 1" },
                    new Category { Id = Guid.NewGuid(), Name = "Category 2" }
                };

            _mockCategoryService.Setup(s => s.GetAllCategoriesAsync())
                .ReturnsAsync(categories);

            var result = await _controller.GetAllCategories();
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCategories = Assert.IsAssignableFrom<IEnumerable<CategoryDTO>>(okResult.Value);

            Assert.Equal(categories.Count, returnedCategories.Count());
            Assert.Collection(returnedCategories,
                item => Assert.Equal("Category 1", item.Name),
                item => Assert.Equal("Category 2", item.Name));
        }

        [Fact]
        public async Task GetAllCategories_WhenEmpty_ReturnsOkWithEmptyList()
        {
            _mockCategoryService.Setup(s => s.GetAllCategoriesAsync())
            .Returns(Task.FromResult(new List<Category>()));

            var result = await _controller.GetAllCategories();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCategories = Assert.IsAssignableFrom<IEnumerable<CategoryDTO>>(okResult.Value);
            Assert.Empty(returnedCategories);
        }
    }
}

