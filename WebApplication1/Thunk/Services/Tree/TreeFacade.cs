using Microsoft.EntityFrameworkCore;
using Thunk.Services.ExceptionsJournal;

namespace Thunk.Services.Tree
{
  public class UserTree
  {
    public long Id { get; set; }
    public string TreeName { get; set; }
  }

  public class TreeNode
  {
    public UserTree UserTree { get; set; }

    public long Id { get; set; }
    public string Name { get; set; }
    public TreeNode? Parent { get; set; }
    public IList<TreeNode> Children { get; set; }
  }

  public class TreeFacade: DbContext
  {
    public DbSet<UserTree> UserTrees { get; set; }
    public DbSet<TreeNode> Nodes { get; set; }

    public TreeFacade() {}

    public TreeFacade(DbContextOptions<TreeFacade> options)
      : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<UserTree>(entity =>
      {
        entity.ToTable("user_trees");

        entity.HasKey(x => x.Id);
        entity.Property(x => x.TreeName)
          .IsRequired();

        entity.HasIndex(x => x.TreeName).IsUnique();
      });

      modelBuilder.Entity<TreeNode>(entity =>
      {
        entity.ToTable("nodes");

        entity.HasKey(x => x.Id);
        entity.Property(x => x.Name)
          .IsRequired();

        entity.HasOne(x => x.Parent)
          .WithMany(x => x.Children)
          .IsRequired(false)
          .OnDelete(DeleteBehavior.Restrict);

        entity.HasMany(x => x.Children)
          .WithOne(x => x.Parent)
          .OnDelete(DeleteBehavior.Cascade);

        entity.HasIndex(x => x.Name);
      });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      base.OnConfiguring(optionsBuilder);
      optionsBuilder.UseNpgsql(@"Host=localhost;Port=5333;Username=postgre;Password=postgre;Database=tree")
        .UseSnakeCaseNamingConvention();
    }
  }
}
