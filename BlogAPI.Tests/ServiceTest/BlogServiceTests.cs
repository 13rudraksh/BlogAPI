using Xunit;
using BlogAPI.Core.Model;
using BlogAPI.Persistence.Services;
using MongoDB.Bson;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace BlogAPI.Tests.ServiceTest;

public class BlogServiceTests : IDisposable
{
    private readonly BlogDbContext _context;
    private readonly ILogger<BlogService> _logger;
    private readonly BlogService _blogService;

    public BlogServiceTests()
    {
        var options = new DbContextOptionsBuilder<BlogDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new BlogDbContext(options);

        _logger = new Mock<ILogger<BlogService>>().Object;
        _blogService = new BlogService(_context, _logger);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
    }

    [Fact]
    public async Task RegisterUser_ShouldReturnUser_WhenUserIsRegistered()
    {
        var newUser = new User { UserName = "TestUser", Email = "testuser@test.com", Password = "Test@1234" };

        var result = await _blogService.RegisterUser(newUser);

        Assert.Equal(newUser, result);
    }

    [Fact]
    public async Task DoesUserExist_ShouldReturnTrue_WhenUserExists()
    {
        var existingUser = new User { UserName = "ExistingUser", Email = "existinguser@test.com", Password = "Test@1234" };
        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        var result = await _blogService.DoesUserExist(existingUser.UserName, existingUser.Email);

        Assert.True(result);
    }

    [Fact]
    public async Task GetUser_ShouldReturnUser_WhenUserExists()
    {
        var existingUser = new User { UserName = "ExistingUser", Email = "existinguser@test.com", Password = "Test@1234" };
        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        var result = await _blogService.GetUser(existingUser._id);

        Assert.Equal(existingUser, result);
    }

    [Fact]
    public async Task AddBlog_ShouldReturnBlog_WhenBlogIsAdded()
    {
        var newBlog = new Blog { UserId = ObjectId.GenerateNewId(), BlogName = "Test Blog", Category = "Test", Article = "This is a test blog.", AuthorName = "Test Author" };

        var result = await _blogService.AddBlog(newBlog);

        Assert.Equal(newBlog, result);
    }

    [Fact]
    public async Task DoesBlogExist_ShouldReturnTrue_WhenBlogExists()
    {
        var existingBlog = new Blog { UserId = ObjectId.GenerateNewId(), BlogName = "Existing Blog", Category = "Test", Article = "This is an existing blog.", AuthorName = "Test Author" };
        _context.Blogs.Add(existingBlog);
        await _context.SaveChangesAsync();

        var result = await _blogService.DoesBlogExist(existingBlog.UserId, existingBlog.BlogName);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteBlog_ShouldReturnTrue_WhenBlogIsDeleted()
    {
        // Arrange
        var existingBlog = new Blog { UserId = ObjectId.GenerateNewId(), BlogName = "Existing Blog", Category = "Test", Article = "This is an existing blog.", AuthorName = "Test Author" };
        _context.Blogs.Add(existingBlog);
        await _context.SaveChangesAsync();

        // Act
        var result = await _blogService.DeleteBlog(existingBlog.UserId, existingBlog.BlogName);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task GetAllUserBlogs_ShouldReturnAllUserBlogs_WhenCalled()
    {
        // Arrange
        var userId = ObjectId.GenerateNewId();
        var blogs = new List<Blog>
    {
        new Blog { UserId = userId, BlogName = "Blog 1", Category = "Test", Article = "This is blog 1.", AuthorName = "Test Author" },
        new Blog { UserId = userId, BlogName = "Blog 2", Category = "Test", Article = "This is blog 2.", AuthorName = "Test Author" }
    };
        _context.Blogs.AddRange(blogs);
        await _context.SaveChangesAsync();

        // Act
        var result = await _blogService.GetAllUserBlogs(userId);

        // Assert
        Assert.Equal(blogs, result);
    }

    [Fact]
    public async Task GetBlogsByCategory_ShouldReturnAllBlogsInCategory_WhenCalled()
    {
        // Arrange
        var category = "Test";
        var blogs = new List<Blog>
    {
        new Blog { UserId = ObjectId.GenerateNewId(), BlogName = "Blog 1", Category = category, Article = "This is blog 1.", AuthorName = "Test Author" },
        new Blog { UserId = ObjectId.GenerateNewId(), BlogName = "Blog 2", Category = category, Article = "This is blog 2.", AuthorName = "Test Author" }
    };
        _context.Blogs.AddRange(blogs);
        await _context.SaveChangesAsync();

        // Act
        var result = await _blogService.GetBlogsByCategory(category);

        // Assert
        Assert.Equal(blogs, result);
    }

    [Fact]
    public async Task GetBlogsByDuration_ShouldReturnAllBlogsInDuration_WhenCalled()
    {
        // Arrange
        var category = "Test";
        var durationFromRange = DateTime.UtcNow.AddDays(-7);
        var durationToRange = DateTime.UtcNow;
        var blogs = new List<Blog>
    {
        new Blog { UserId = ObjectId.GenerateNewId(), BlogName = "Blog 1", Category = category, Article = "This is blog 1.", AuthorName = "Test Author", Timestamp = DateTime.UtcNow.AddDays(-5) },
        new Blog { UserId = ObjectId.GenerateNewId(), BlogName = "Blog 2", Category = category, Article = "This is blog 2.", AuthorName = "Test Author", Timestamp = DateTime.UtcNow.AddDays(-3) }
    };
        _context.Blogs.AddRange(blogs);
        await _context.SaveChangesAsync();

        // Act
        var result = await _blogService.GetBlogsByDuration(category, durationFromRange, durationToRange);

        // Assert
        Assert.Equal(blogs, result);
    }

}

