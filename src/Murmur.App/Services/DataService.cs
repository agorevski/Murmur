using SQLite;
using Murmur.App.Models;

namespace Murmur.App.Services;

public interface IDataService
{
    Task InitializeAsync();
    Task<List<Mix>> GetMixesAsync();
    Task<List<Mix>> GetFavoriteMixesAsync();
    Task<Mix?> GetMixAsync(int id);
    Task<int> SaveMixAsync(Mix mix);
    Task<int> DeleteMixAsync(Mix mix);
    Task<UserPreferences> GetUserPreferencesAsync();
    Task SaveUserPreferencesAsync(UserPreferences preferences);
}

public class DataService : IDataService
{
    private SQLiteAsyncConnection? _database;
    private readonly string _dbPath;

    public DataService()
    {
        _dbPath = Path.Combine(FileSystem.AppDataDirectory, "murmur.db3");
    }

    private async Task<SQLiteAsyncConnection> GetDatabaseAsync()
    {
        if (_database != null)
            return _database;

        _database = new SQLiteAsyncConnection(_dbPath);
        await _database.CreateTableAsync<Mix>();
        await _database.CreateTableAsync<UserPreferences>();
        return _database;
    }

    public async Task InitializeAsync()
    {
        var db = await GetDatabaseAsync();
        
        // Initialize default user preferences if not exists
        var prefs = await db.Table<UserPreferences>().FirstOrDefaultAsync();
        if (prefs == null)
        {
            await db.InsertAsync(new UserPreferences
            {
                Id = 1,
                IsPremium = false,
                AdsEnabled = true,
                DefaultTimerMinutes = 30,
                DefaultVolume = 0.7f
            });
        }
    }

    public async Task<List<Mix>> GetMixesAsync()
    {
        var db = await GetDatabaseAsync();
        return await db.Table<Mix>().OrderByDescending(m => m.LastUsed).ToListAsync();
    }

    public async Task<List<Mix>> GetFavoriteMixesAsync()
    {
        var db = await GetDatabaseAsync();
        return await db.Table<Mix>().Where(m => m.IsFavorite).OrderByDescending(m => m.LastUsed).ToListAsync();
    }

    public async Task<Mix?> GetMixAsync(int id)
    {
        var db = await GetDatabaseAsync();
        return await db.Table<Mix>().Where(m => m.Id == id).FirstOrDefaultAsync();
    }

    public async Task<int> SaveMixAsync(Mix mix)
    {
        var db = await GetDatabaseAsync();
        
        if (mix.Id != 0)
        {
            return await db.UpdateAsync(mix);
        }
        else
        {
            mix.CreatedAt = DateTime.Now;
            mix.LastUsed = DateTime.Now;
            return await db.InsertAsync(mix);
        }
    }

    public async Task<int> DeleteMixAsync(Mix mix)
    {
        var db = await GetDatabaseAsync();
        return await db.DeleteAsync(mix);
    }

    public async Task<UserPreferences> GetUserPreferencesAsync()
    {
        var db = await GetDatabaseAsync();
        var prefs = await db.Table<UserPreferences>().FirstOrDefaultAsync();
        return prefs ?? new UserPreferences { Id = 1 };
    }

    public async Task SaveUserPreferencesAsync(UserPreferences preferences)
    {
        var db = await GetDatabaseAsync();
        await db.UpdateAsync(preferences);
    }
}
