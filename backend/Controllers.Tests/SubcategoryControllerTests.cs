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
    public class SubcategoryControllerTests
    {
        private readonly Mock<ICategoryService> _mockCategoryService;
        private readonly SubcategoryController _controller;

        public SubcategoryControllerTests()
        {
            _mockCategoryService = new Mock<ICategoryService>();

            _controller = new SubcategoryController(_mockCategoryService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        }

        [Fact]
        public async Task GetSubcategoriesByCategoryId_ExistingCategory_ReturnsOkWithSubcategories()
        {
            var categoryId = Guid.NewGuid();
            var subcategories = new List<SubcategoryDTO>
            {
                new SubcategoryDTO { Id = Guid.NewGuid(), Name = "Subcategory1", CategoryId = categoryId },
                new SubcategoryDTO { Id = Guid.NewGuid(), Name = "Subcategory2", CategoryId = categoryId }
            };

            _mockCategoryService
                .Setup(s => s.GetSubcategoriesByCategoryIdAsync(categoryId))
                .ReturnsAsync(Subcategory.FromDTO(subcategories));

            var result = await _controller.GetSubcategoriesByCategoryId(categoryId);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsAssignableFrom<IEnumerable<SubcategoryDTO>>(okResult.Value);
            Assert.Equal(subcategories.Count, returned.Count());
            Assert.All(returned, dto => Assert.Equal(categoryId, dto.CategoryId));
        }

        [Fact]
        public async Task GetSubcategoriesByCategoryId_NoSubcategories_ReturnsOkWithEmptyList()
        {
            var categoryId = Guid.NewGuid();

            _mockCategoryService
                .Setup(s => s.GetSubcategoriesByCategoryIdAsync(categoryId))
                .ReturnsAsync(Subcategory.FromDTO(new List<SubcategoryDTO>())); 
            var result = await _controller.GetSubcategoriesByCategoryId(categoryId);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsAssignableFrom<IEnumerable<SubcategoryDTO>>(okResult.Value);
            Assert.Empty(returned);
        }

        [Fact]
        public async Task GetSubcategoriesByCategoryId_ServiceReturnsNull_ReturnsOkWithEmptyList()
        {
            var categoryId = Guid.NewGuid();

            _mockCategoryService
                .Setup(s => s.GetSubcategoriesByCategoryIdAsync(categoryId))
                .ReturnsAsync((List<Subcategory>)null); 
            var result = await _controller.GetSubcategoriesByCategoryId(categoryId);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsAssignableFrom<IEnumerable<SubcategoryDTO>>(okResult.Value);
            Assert.Empty(returned);
        }
    }
}
