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
using Microsoft.AspNetCore.Http.HttpResults;

namespace Controllers.Tests
{
    public class AdminControllerTests
    {
        private readonly Mock<ICategoryService> _mockCategoryService;
        private readonly Mock<ISubcategoryService> _mockSubcategoryService;
        private readonly Mock<ICourseService> _mockCourseService;
        private readonly Mock<IReviewService> _mockReviewService;
        private readonly AdminController _controller;

        public AdminControllerTests()
        {
            _mockCategoryService = new Mock<ICategoryService>();
            _mockSubcategoryService = new Mock<ISubcategoryService>();
            _mockCourseService = new Mock<ICourseService>();
            _mockReviewService = new Mock<IReviewService>();

            _controller = new AdminController(
                _mockCategoryService.Object,
                _mockSubcategoryService.Object,
                _mockCourseService.Object,
                _mockReviewService.Object
            )
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        }

        [Fact]
        public void Dashboard_ReturnsOk()
        {
            var result = _controller.Dashboard();

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task AddCategory_ValidDto_ReturnsCreated()
        {
            var dto = new CategoryNameDto { Name = "New Category" };
            var categoryEntity = Category.FromDTO(dto);
            categoryEntity.Id = Guid.NewGuid();

            _mockCategoryService
                .Setup(s => s.AddCategoryAsync(It.Is<Category>(c => c.Name == dto.Name)))
                .ReturnsAsync(categoryEntity);

            var result = await _controller.AddCategory(dto);

            var createdAtResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnedDto = Assert.IsType<CategoryDTO>(createdAtResult.Value);

            Assert.Equal(categoryEntity.Id, returnedDto.Id);
            Assert.Equal(categoryEntity.Name, returnedDto.Name);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task AddCategory_InvalidName_ReturnsBadRequest(string invalidName)
        {
            var dto = new CategoryNameDto { Name = invalidName };

            var result = await _controller.AddCategory(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Nazwa kategorii jest wymagana.", badRequest.Value);
        }

        [Fact]
        public async Task AddSubcategory_ValidDto_ReturnsCreated()
        {
            var dto = new SubcategoryNameDto { Name = "Subcat", CategoryId = Guid.NewGuid() };
            var subcategoryEntity = Subcategory.FromDTO(dto);
            subcategoryEntity.Id = Guid.NewGuid();

            _mockSubcategoryService.Setup(s => s.AddSubcategoryAsync(It.Is<Subcategory>(sc => sc.Name == dto.Name && sc.CategoryId == dto.CategoryId)))
                .ReturnsAsync(subcategoryEntity);

            var result = await _controller.AddSubcategory(dto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnedDto = Assert.IsType<SubcategoryDTO>(createdResult.Value);
            Assert.Equal(subcategoryEntity.Id, returnedDto.Id);
            Assert.Equal(subcategoryEntity.Name, returnedDto.Name);
            Assert.Equal(subcategoryEntity.CategoryId, returnedDto.CategoryId);
        }

        [Fact]
        public async Task AddSubcategory_MissingName_ReturnsBadRequest()
        {
            var dto = new SubcategoryNameDto { Name = null, CategoryId = Guid.NewGuid() };

            var result = await _controller.AddSubcategory(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Error! Subcategory name is required!", badRequest.Value);
        }

        [Fact]
        public async Task AddSubcategory_MissingCategoryId_ReturnsBadRequest()
        {
            var dto = new SubcategoryNameDto { Name = "Test", CategoryId = Guid.Empty };

            var result = await _controller.AddSubcategory(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Error! Category ID is required!", badRequest.Value);
        }

        [Fact]
        public async Task DeleteCourse_ExistingId_ReturnsOk()
        {
            var courseId = Guid.NewGuid();
            var course = new Course { Id = courseId, Name = "Course1" };

            _mockCourseService.Setup(s => s.DeleteCourseAsync(courseId)).ReturnsAsync(course);
            var result = await _controller.DeleteCourse(courseId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<DeleteResponseDTO>(okResult.Value);
            Assert.Equal("Course deleted", response.message);
            Assert.Equal(course.Name, response.name);
        }

        [Fact]
        public async Task DeleteCourse_NonExistingId_ReturnsNotFound()
        {
            var courseId = Guid.NewGuid();

            _mockCourseService.Setup(s => s.DeleteCourseAsync(courseId)).ReturnsAsync((Course)null);

            var result = await _controller.DeleteCourse(courseId);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteReviews_ValidIds_ReturnsOk()
        {
            var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var dto = new DeleteReviewsDto { ReviewIds = ids };

            _mockReviewService.Setup(s => s.DeleteReviewsAsync(ids)).Returns(Task.CompletedTask);

            var result = await _controller.DeleteReviews(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<DeleteResponseDTO>(okResult.Value);

            Assert.Equal("Review deleted", response.message);
        }

        [Fact]
        public async Task DeleteReviews_EmptyIds_ReturnsBadRequest()
        {
            var dto = new DeleteReviewsDto { ReviewIds = new List<Guid>() };

            var result = await _controller.DeleteReviews(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Review ID not found", badRequest.Value);
        }

        [Fact]
        public async Task DeleteCategory_ExistingId_ReturnsOk()
        {
            var id = Guid.NewGuid();
            var category = new Category { Id = id, Name = "Cat1" };

            _mockCategoryService.Setup(s => s.DeleteCategoryAsync(id)).ReturnsAsync(category);

            var result = await _controller.DeleteCategory(id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<DeleteResponseDTO>(okResult.Value);
            Assert.Equal("Category deleted", response.message);
            Assert.Equal(category.Name, response.name);
        }

        [Fact]
        public async Task DeleteCategory_NonExistingId_ReturnsNotFound()
        {
            var id = Guid.NewGuid();

            _mockCategoryService.Setup(s => s.DeleteCategoryAsync(id)).ReturnsAsync((Category)null);

            var result = await _controller.DeleteCategory(id);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteSubcategory_ExistingId_ReturnsOk()
        {
            var id = Guid.NewGuid();
            var subcategory = new Subcategory { Id = id, Name = "Subcat1", CategoryId = Guid.NewGuid() };

            _mockSubcategoryService.Setup(s => s.DeleteSubcategoryAsync(id)).ReturnsAsync(subcategory);

            var result = await _controller.DeleteSubcategory(id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<DeleteResponseDTO>(okResult.Value);

            Assert.Equal("Subcategory deleted", response.message);
            Assert.Equal(subcategory.Name, response.name);
        }

        [Fact]
        public async Task DeleteSubcategory_NonExistingId_ReturnsNotFound()
        {
            var id = Guid.NewGuid();

            _mockSubcategoryService.Setup(s => s.DeleteSubcategoryAsync(id)).ReturnsAsync((Subcategory)null);

            var result = await _controller.DeleteSubcategory(id);

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
