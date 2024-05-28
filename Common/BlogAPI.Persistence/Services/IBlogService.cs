using BlogAPI.Core.Model;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogAPI.Persistence.Services
{
    public interface IBlogService
    {
        Task<User> RegisterUser(User newUser);
        Task<User?> GetUser(ObjectId id);
        Task<Blog> AddBlog(Blog blog);
        Task<bool> DoesUserExist(string username, string email);
        Task<bool> DoesBlogExist(ObjectId userId, string blogname);
        Task<bool> DeleteBlog(ObjectId userId, string blogname);
        Task<List<Blog>> GetAllUserBlogs(ObjectId userId);
        Task<List<Blog>> GetBlogsByCategory(string category);
        Task<List<Blog>> GetBlogsByDuration(string category, DateTime durationFromRange, DateTime durationToRange);
    }
}
