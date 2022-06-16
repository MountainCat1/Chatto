namespace Chatto.Infrastructure;

public class DatabaseSeeder
{
    private readonly DatabaseContext _databaseContext;

    public DatabaseSeeder(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }


    public void Seed(bool recreate)
    {
        if (recreate)
        {
            _databaseContext.Database.EnsureDeleted();
        }
        _databaseContext.Database.EnsureCreated();
    }
}