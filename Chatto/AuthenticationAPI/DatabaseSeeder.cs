
using ChattoAuth.Infrastructure;

public class DatabaseSeeder
{
    private readonly DatabaseContext _databaseContext;

    public DatabaseSeeder(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public void Seed(bool reset)
    {
        if (reset)
        {
            _databaseContext.Database.EnsureDeleted();
        }
        _databaseContext.Database.EnsureCreated();
        
        _databaseContext.SaveChanges();
    }
}