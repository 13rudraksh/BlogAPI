using BlogAPI.Core.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogAPI.Persistence.Services
{
    public interface IBlogDbContext
    {
        DbSet<User> Users { get; }
        DbSet<Blog> Blogs { get; }

        // Include other DbSets and methods you want to mock
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
