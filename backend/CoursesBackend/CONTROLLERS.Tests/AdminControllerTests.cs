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
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CONTROLLERS.Tests
{
    public class AdminControllerTests
    {
        private readonly Mock<ICategoryService> _categoryService;
        private readonly Mock<ISubcategoryService> _subcategoryService;
        private readonly Mock<ICourseService> _courseService;
        private readonly Mock<IReviewService> _reviewService;
        private readonly AdminController _controller;

        public AdminControllerTests()
        {
            _categoryService = new Mock<ICategoryService>();
            _subcategoryService = new Mock<ISubcategoryService>();
            _courseService = new Mock<ICourseService>();
            _reviewService = new Mock<IReviewService>();

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "GooglePerspective:ApiKey", "fake-api-key" }
                })
                .Build();

            _controller = new AdminController(
                _categoryService.Object,
                _subcategoryService.Object,
                _courseService.Object,
                _reviewService.Object,
                config
            )
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        }

        [Fact]
        public async Task AddCategory_ValidInput_ReturnsCreated()
        {
            var dto = new CategoryNameDto { Name = "Programming" };
            var category = new Category { Id = Guid.NewGuid(), Name = "Programming" };

            _categoryService.Setup(s => s.AddCategoryAsync(It.IsAny<Category>())).ReturnsAsync(category);

            var result = await _controller.AddCategory(dto);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            var response = Assert.IsType<CategoryDTO>(created.Value);
            Assert.Equal("Programming", response.Name);
        }

        [Fact]
        public async Task AddCategory_MissingName_ReturnsBadRequest()
        {
            var result = await _controller.AddCategory(new CategoryNameDto { Name = " " });

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Nazwa kategorii jest wymagana.", badRequest.Value);
        }

        [Fact]
        public async Task AddSubcategory_ValidInput_ReturnsCreated()
        {
            var dto = new SubcategoryNameDto
            {
                Name = "C#",
                CategoryId = Guid.NewGuid()
            };

            var sub = new Subcategory
            {
                Id = Guid.NewGuid(),
                Name = "C#",
                CategoryId = dto.CategoryId
            };

            _subcategoryService.Setup(s => s.AddSubcategoryAsync(It.IsAny<Subcategory>())).ReturnsAsync(sub);

            var result = await _controller.AddSubcategory(dto);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            var response = Assert.IsType<SubcategoryDTO>(created.Value);
            Assert.Equal("C#", response.Name);
        }

        [Fact]
        public async Task AddSubcategory_MissingName_ReturnsBadRequest()
        {
            var result = await _controller.AddSubcategory(new SubcategoryNameDto { Name = "", CategoryId = Guid.NewGuid() });

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Subcategory name is required", badRequest.Value!.ToString());
        }

        [Fact]
        public async Task DeleteCourse_ExistingCourse_ReturnsOk()
        {
            var courseId = Guid.NewGuid();
            var course = new Course { Id = courseId, Name = "Test Course" };

            _courseService.Setup(s => s.DeleteCourseAsync(courseId)).ReturnsAsync(course);

            var result = await _controller.DeleteCourse(courseId);

            var ok = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<DeleteResponseDTO>(ok.Value);

            Assert.Equal("Course deleted", response.message);
            Assert.Equal("Test Course", response.name);
        }

        [Fact]
        public async Task DeleteCourse_NotFound_ReturnsNotFound()
        {
            var id = Guid.NewGuid();
            _courseService.Setup(s => s.DeleteCourseAsync(id)).ReturnsAsync((Course?)null);

            var result = await _controller.DeleteCourse(id);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Course not found", notFound.Value);
        }

        [Fact]
        public async Task ToggleCourseVisibility_ChangesState_ReturnsOk()
        {
            var id = Guid.NewGuid();
            var course = new Course { Id = id, IsHidden = false };
            var originalIsHidden = course.IsHidden;

            _courseService.Setup(s => s.GetCourseByIdAsync(id)).ReturnsAsync(course);
            _courseService.Setup(s => s.UpdateCourseAsync(course)).ReturnsAsync(course);

            var result = await _controller.ToggleCourseVisibility(id);

            var ok = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<CourseResponseDTO>(ok.Value);

            Assert.Equal(!originalIsHidden, dto.IsHidden);
        }

        [Fact]
        public async Task ToggleCourseVisibility_CourseNotFound_ReturnsNotFound()
        {
            _courseService.Setup(s => s.GetCourseByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Course?)null);

            var result = await _controller.ToggleCourseVisibility(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteReview_ReviewFoundAndDeleted_ReturnsNoContent()
        {
            var id = Guid.NewGuid();

            _reviewService.Setup(s => s.GetReviewByIdAsync(id)).ReturnsAsync(new Review());
            _reviewService.Setup(s => s.DeleteReviewAsync(id)).ReturnsAsync(new Review());

            var result = await _controller.DeleteReview(id);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteReview_NotFound_ReturnsNotFound()
        {
            var id = Guid.NewGuid();

            _reviewService.Setup(s => s.GetReviewByIdAsync(id)).ReturnsAsync((Review?)null);

            var result = await _controller.DeleteReview(id);

            var notFound = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteReview_DeleteFails_ReturnsBadRequest()
        {
            var id = Guid.NewGuid();

            _reviewService.Setup(s => s.GetReviewByIdAsync(id)).ReturnsAsync(new Review());
            _reviewService.Setup(s => s.DeleteReviewAsync(id)).ReturnsAsync((Review?)null);

            var result = await _controller.DeleteReview(id);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed to delete review", bad.Value);
        }

        [Fact]
        public async Task AnalyzeToxicity_EmptyComment_ReturnsBadRequest()
        {
            var result = await _controller.AnalyzeToxicity(new AnalyzeReviewDTO { Comment = "" });

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("The comment field is required.", bad.Value);
        }
    }
}
