using BlogAPI.Core.Model;
using BlogAPI.Persistence.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace BlogAPI.Controllers;

[Route("api/v1.0/blogsite")]
public class BlogSite : Controller
{
    private readonly IBlogService _blogService;
    private readonly ILogger _logger;

    public BlogSite(IBlogService blogService, ILogger<BlogSite> logger)
    {
        _blogService = blogService;
        _logger = logger;
    }

    [HttpPost("user/register")]
    public async Task<IActionResult> RegisterUser([FromBody] User user)
    {
        _logger.LogInformation("User Registration Stated for {0} with {1}", user.UserName, user.Email);
        try
        {
            if (ModelState.IsValid)
            {
                bool doesUserExist = await _blogService.DoesUserExist(user.UserName, user.Email);
                if (doesUserExist)
                {
                    string userExists = "A user with the same username and email already exists.";
                    _logger.LogInformation(userExists);
                    return BadRequest(userExists);
                }

                var newUser = await _blogService.RegisterUser(user);
                _logger.LogInformation("Registration Successfull for user: {0}, ID: {1}", newUser.UserName, newUser._id);
                return Ok();
            }
            else
            {
                _logger.LogInformation("Invalid entry: {0}", ModelState.ToString());
                return BadRequest(ModelState);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Operation failed, Error: {ex}", ex);
            return StatusCode(500);
        }
    }

    [HttpGet("blogs/info/{category}")]
    public async Task<IActionResult> GetBlogsByCategory(string category)
    {
        _logger.LogInformation("Get Blogs by Category request started for category: {category}", category);
        try
        {
            if (string.IsNullOrEmpty(category))
            {
                string msg = "Category is missing.";
                _logger.LogInformation(msg);
                return BadRequest(msg);
            }

            List<Blog> blogs = await _blogService.GetBlogsByCategory(category);
            _logger.LogInformation("{0} Blogs found successfully for category: {1}.", blogs.Count, category);
            return Ok(blogs);
        }
        catch (Exception ex)
        {
            _logger.LogError("Operation failed, Error: {ex}", ex);
            return StatusCode(500);
        }
    }

    [HttpGet("user/getall")]
    public async Task<IActionResult> GetAllUserBlogs([FromHeader] string userId)
    {
        _logger.LogInformation("Get All Blog request started for userId: {userId}", userId);
        try
        {
            if (string.IsNullOrEmpty(userId))
            {
                string msg = "User ID is missing from the headers.";
                _logger.LogInformation(msg);
                return BadRequest(msg);
            }

            List<Blog> blogs = await _blogService.GetAllUserBlogs(new ObjectId(userId));
            _logger.LogInformation("{0} Blogs found successfully.", blogs.Count);
            return Ok(blogs);
        }
        catch (Exception ex)
        {
            _logger.LogError("Operation failed, Error: {ex}", ex);
            return StatusCode(500);
        }
    }

    [HttpDelete("user/delete/{blogname}")]
    public async Task<IActionResult> DeleteBlog([FromHeader] string userId, string blogname)
    {
        _logger.LogInformation("Delete Blog request started for blogname: {blogname}", blogname);
        try
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(blogname))
            {
                string msg = "User ID or blog name is missing.";
                _logger.LogInformation(msg);
                return BadRequest(msg);
            }

            bool success = await _blogService.DeleteBlog(new ObjectId(userId), blogname);
            if (success)
            {
                return Ok();
            }
            else
            {
                return NotFound("The blog does not exist or the user ID is incorrect.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Operation failed, Error: {ex}", ex);
            return StatusCode(500);
        }
    }

    [HttpPost("user/blogs/add/{blogname}")]
    public async Task<IActionResult> AddBlog([FromHeader] string userId, string blogname, [FromBody] Blog blog)
    {
        _logger.LogInformation("Add Blog request started for userId: {userId}", userId);
        try
        {
            if (string.IsNullOrEmpty(userId))
                ModelState.AddModelError("userId", "User ID is missing from the headers.");

            if (string.IsNullOrEmpty(blogname) || blogname.Length < 20)
                ModelState.AddModelError("blogname", "Blog name is missing or too short.");
            else
            {
                blog.BlogName = blogname;
                ModelState.Remove("BlogName");
            }

            if (ModelState.IsValid)
            {
                blog.UserId = new ObjectId(userId);
                var newBlog = await _blogService.AddBlog(blog);
                _logger.LogInformation("Blog added successfully for userId: {0}", newBlog.UserId);
                return Ok(newBlog);
            }
            else
            {
                _logger.LogInformation("Invalid entry: {0}", ModelState.ToString());
                return BadRequest(ModelState);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Operation failed, Error: {ex}", ex);
            return StatusCode(500);
        }
    }

    [HttpGet("blogs/get/{category}/{durationFromRange}/{durationToRange}")]
    public async Task<IActionResult> GetBlogsByDuration(string category, DateTime durationFromRange, DateTime durationToRange)
    {
        _logger.LogInformation("Get Blogs by Duration request started for category: {category} between {durationFromRange} - {durationToRange}"
            , category, durationFromRange, durationToRange);
        try
        {
            if (string.IsNullOrEmpty(category))
            {
                string msg = "Category is missing.";
                _logger.LogInformation(msg);
                return BadRequest(msg);
            }

            if (durationFromRange > durationToRange)
            {
                string msg = "Invalid duration range. The start date cannot be later than the end date.";
                _logger.LogInformation(msg);
                return BadRequest(msg);
            }

            List<Blog> blogs = await _blogService.GetBlogsByDuration(category, durationFromRange, durationToRange);
            if (blogs.Count > 0)
            {
                _logger.LogInformation("{0} Blogs found successfully for category: {1} within the given duration.", blogs.Count, category);
                return Ok(blogs);
            }
            else
            {
                string msg = "No blogs found for the given category and duration.";
                _logger.LogInformation(msg);
                return NotFound(msg);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Operation failed, Error: {ex}", ex);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}
