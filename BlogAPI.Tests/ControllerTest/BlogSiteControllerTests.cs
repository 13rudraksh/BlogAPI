using Xunit;
using BlogAPI.Controllers;
using BlogAPI.Persistence.Services;
using BlogAPI.Core.Model;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MongoDB.Bson;

namespace BlogAPI.Tests.ControllerTest
{
    public class BlogSiteControllerTests
    {
        private readonly Mock<IBlogService> _mockBlogService;
        private readonly Mock<ILogger<BlogSite>> _mockLogger;
        private readonly BlogSite _controller;

        public BlogSiteControllerTests()
        {
            _mockBlogService = new Mock<IBlogService>();
            _mockLogger = new Mock<ILogger<BlogSite>>();
            _controller = new BlogSite(_mockBlogService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task RegisterUser_ReturnsOkResult_WhenUserDoesNotExist()
        {
            var user = new User { UserName = "test", Email = "test@test.com" };
            _mockBlogService.Setup(service => service.DoesUserExist(user.UserName, user.Email)).ReturnsAsync(false);
            _mockBlogService.Setup(service => service.RegisterUser(user)).ReturnsAsync(user);

            var result = await _controller.RegisterUser(user);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task GetAllUserBlogsAsync_ReturnsOkResult_WhenUserHasBlogs()
        {
            string userId = "60d5ea9f2e59d623b30b1772";
            var blogs = new List<Blog> { new Blog { UserId = new ObjectId(userId) } };
            _mockBlogService.Setup(service => service.GetAllUserBlogs(new ObjectId(userId))).ReturnsAsync(blogs);

            var result = await _controller.GetAllUserBlogs(userId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnBlogs = Assert.IsType<List<Blog>>(okResult.Value);
            Assert.Equal(blogs.Count, returnBlogs.Count);
        }

        [Fact]
        public async Task DeleteBlogAsync_ReturnsOkResult_WhenBlogExists()
        {
            string userId = "60d5ea9f2e59d623b30b1772";
            string blogname = "test";
            _mockBlogService.Setup(service => service.DeleteBlog(new ObjectId(userId), blogname)).ReturnsAsync(true);

            var result = await _controller.DeleteBlog(userId, blogname);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task AddBlog_ReturnsOkResult_WhenBlogIsValid()
        {
            string userId = "60d5ea9f2e59d623b30b1772";
            string blogname = "This is the title of my new blog post";
            string authorname = "Author's Name";
            string article = "Article";
            string category = "Cat";
            var blog = new Blog { UserId = new ObjectId(userId), BlogName = blogname, AuthorName = authorname, Article = article, Category = category };
            _mockBlogService.Setup(service => service.AddBlog(blog)).ReturnsAsync(blog);

            var result = await _controller.AddBlog(userId, blogname, blog);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnBlog = Assert.IsType<Blog>(okResult.Value);
            Assert.Equal(blog.BlogName, returnBlog.BlogName);
        }

        [Fact]
        public async Task GetBlogsByCategory_ReturnsOkResult_WhenCategoryExists()
        {
            string category = "technology";
            var blogs = new List<Blog> { new Blog { Category = category } };
            _mockBlogService.Setup(service => service.GetBlogsByCategory(category)).ReturnsAsync(blogs);

            var result = await _controller.GetBlogsByCategory(category);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnBlogs = Assert.IsType<List<Blog>>(okResult.Value);
            Assert.Equal(blogs.Count, returnBlogs.Count);
        }

        [Fact]
        public async Task GetBlogsByDuration_ReturnsOkResult_WhenDurationIsValid()
        {
            string category = "technology";
            DateTime durationFromRange = DateTime.Now.AddDays(-1);
            DateTime durationToRange = DateTime.Now.AddDays(1);
            var blogs = new List<Blog> { new Blog { Category = category, Timestamp = DateTime.Now } };
            _mockBlogService.Setup(service => service.GetBlogsByDuration(category, durationFromRange, durationToRange)).ReturnsAsync(blogs);

            var result = await _controller.GetBlogsByDuration(category, durationFromRange, durationToRange);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnBlogs = Assert.IsType<List<Blog>>(okResult.Value);
            Assert.Equal(blogs.Count, returnBlogs.Count);
        }

    }
}
