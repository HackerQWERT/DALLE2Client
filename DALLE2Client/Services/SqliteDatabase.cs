namespace DALLE2Client.Services;

public class SqliteDatabase
{
    private  SQLiteAsyncConnection sqliteDatabase;

    public SqliteDatabase(string dbPath)
    {
        sqliteDatabase = new SQLiteAsyncConnection(dbPath);
        _ = CreateImageModelsAsync();
    }

    public async Task CreateImageModelsAsync()
    {
        await sqliteDatabase.CreateTableAsync<ImageModel>();
    }
    public async Task<List<ImageModel>> GetImageModelsAsync()
    {
        await CreateImageModelsAsync();
        return await sqliteDatabase.Table<ImageModel>().ToListAsync();
    }
    public async Task<int> InsertImageModelAsync(ImageModel imageModel)
    {
        await CreateImageModelsAsync();
        return await sqliteDatabase.InsertAsync(imageModel);
    }
    public async Task<int> InsertImageModelsAsync(ObservableCollection<ImageModel> imageModelsList)
    {
        await CreateImageModelsAsync();
        return await sqliteDatabase.InsertAllAsync(imageModelsList);
    }
    public async Task<int> DeleteImageModelAsync(ImageModel imageModel)
    {
        await CreateImageModelsAsync();
        return await sqliteDatabase.DeleteAsync<ImageModel>(imageModel);
    }
    public async Task DeleteImageModelsAsync(ObservableCollection<ImageModel> imageModelsList)
    {
        await CreateImageModelsAsync();             
        foreach(var imageModel in imageModelsList)
        {
            await sqliteDatabase.DeleteAsync<ImageModel>(imageModel);
        }
    }
}
