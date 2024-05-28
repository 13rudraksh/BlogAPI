using BlogAPI.Core.Model;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace BlogAPI.Persistence.Services;

[ExcludeFromCodeCoverage]
public class BlogDbContext : DbContext, IBlogDbContext
{
    public DbSet<User> Users { get; init; }
    public DbSet<Blog> Blogs { get; init; }

    public static BlogDbContext Create(IMongoDatabase database) =>
        new(new DbContextOptionsBuilder<BlogDbContext>()
            .UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName)
            .Options);

    public BlogDbContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
        .HasKey(u => u._id);

        modelBuilder.Entity<Blog>()
        .HasKey(b => b._id);

        modelBuilder.Entity<Blog>()
        .HasIndex(b => b.UserId);

        modelBuilder.Entity<Blog>()
        .HasIndex(b => b.Category);

        modelBuilder.Entity<User>().ToCollection("users");
        modelBuilder.Entity<Blog>().ToCollection("blogs");
    }
}
