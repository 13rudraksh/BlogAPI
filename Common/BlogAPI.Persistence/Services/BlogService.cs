using BlogAPI.Core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BlogAPI.Persistence.Services;

public class BlogService : IBlogService
{
    private readonly IBlogDbContext _context;
    private readonly ILogger _logger;

    public BlogService(IBlogDbContext context, ILogger<BlogService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<User> RegisterUser(User newUser)
    {
        await _context.Users.AddAsync(newUser);
        await _context.SaveChangesAsync();

        return newUser;
    }

    public async Task<bool> DoesUserExist(string username, string email)
    {
        return await _context.Users.AnyAsync(u => u.UserName == username || u.Email == email);
    }

    public async Task<User?> GetUser(ObjectId id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<Blog> AddBlog(Blog blog)
    {
        blog.Timestamp = DateTime.UtcNow;

        await _context.Blogs.AddAsync(blog);
        await _context.SaveChangesAsync();

        return blog;
    }

    public async Task<bool> DoesBlogExist(ObjectId userId, string blogname)
    {
        return await _context.Blogs.AnyAsync(b => b.UserId == userId && b.BlogName == blogname);
    }

    public async Task<bool> DeleteBlog(ObjectId userId, string blogname)
    {
        var blogToDelete = await _context.Blogs.FirstOrDefaultAsync(b => b.UserId == userId && b.BlogName == blogname);

        if (blogToDelete != null)
        {
            _context.Blogs.Remove(blogToDelete);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Blog deleted successfully.");
            return true;
        }
        else
        {
            _logger.LogWarning("Blog not found.");
            return false;
        }
    }

    public async Task<List<Blog>> GetAllUserBlogs(ObjectId userId)
    {
        return await _context.Blogs.Where(b => b.UserId == userId).ToListAsync();
    }

    public async Task<List<Blog>> GetBlogsByCategory(string category)
    {
        return await _context.Blogs.Where(b => b.Category == category).ToListAsync();
    }

    public async Task<List<Blog>> GetBlogsByDuration(string category, DateTime durationFromRange, DateTime durationToRange)
    {
        return await _context.Blogs.Where(b => b.Category == category && b.Timestamp >= durationFromRange && b.Timestamp <= durationToRange).ToListAsync();
    }

}
