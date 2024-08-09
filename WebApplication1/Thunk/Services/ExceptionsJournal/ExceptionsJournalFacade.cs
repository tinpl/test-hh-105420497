using Microsoft.EntityFrameworkCore;

namespace Thunk.Services.ExceptionsJournal
{
  public class JournaledException
  {
    public long Id { get; set; }

    public long EventId;
    public DateTime TimeStamp;

    public string Message;
    public string QueryParams;
    public byte[] Body;
    public string StackTrace;
  }

  public class ExceptionsJournalFacade: DbContext
  {
    public DbSet<JournaledException> JournaledExceptions { get; set; }
    
    public ExceptionsJournalFacade()
    {
    }

    public ExceptionsJournalFacade(DbContextOptions<ExceptionsJournalFacade> options)
      : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<JournaledException>(entity =>
      {
        entity.ToTable("exceptions");
        entity.HasKey(e => e.Id);
        entity.Property(x => x.Message)
          .IsRequired();
        entity.Property(x => x.Body)
          .IsRequired();
        entity.Property(x => x.EventId)
          .IsRequired();
        entity.Property(x => x.QueryParams)
          .IsRequired();
        entity.Property(x => x.StackTrace)
          .IsRequired();
        entity.Property(x => x.TimeStamp)
          .IsRequired();

        entity.HasIndex(x => x.Message)
          //.HasMethod("gin")
          .HasDatabaseName("IX_Messages");
        entity.HasIndex(x => x.TimeStamp);
      });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      base.OnConfiguring(optionsBuilder);
      optionsBuilder.UseNpgsql(@"Host=localhost;Port=5332;Username=postgre;Password=postgre;Database=exceptions_journal")
        .UseSnakeCaseNamingConvention();
    }
  }
}
