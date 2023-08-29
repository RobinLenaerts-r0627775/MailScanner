namespace MailScanner.Shared;

public class CrudService<T> where T : BaseModel
{
    /// <summary>
    /// DbContext instance
    /// </summary>
    protected readonly DbContext _dbContext;

    /// <summary>
    /// A Service that provides CRUD operations for a given entity
    /// </summary>
    /// <param name="logger">ILogger instance</param>
    /// <param name="context">DbContext instance</param>
    public CrudService(DbContext context)
    {
        _dbContext = context;
    }

    /// <summary>
    /// Get all entities
    /// </summary>
    /// <returns>
    /// A list of entities
    /// </returns> 
    public List<T> GetAll()
    {
        var entities = _dbContext.Set<T>().ToList();
        return entities;
    }

    /// <summary>
    /// Get all entities asynchronously
    /// </summary>
    /// <returns>
    /// A list of entities
    /// </returns>
    public Task<List<T>> GetAllAsync()
    {
        var entities = _dbContext.Set<T>().ToListAsync();
        return entities;
    }

    /// <summary>
    /// Get an entity by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>
    /// An entity of type T 
    /// </returns>
    public T? Get(int id)
    {
        var entity = _dbContext.Set<T>().FirstOrDefault(x => x.Id == id) ?? throw new Exception($"Entity not found");
        return entity;
    }

    /// <summary>
    /// Get an entity by id asynchronously
    /// </summary>
    /// <param name="id"></param>
    /// <returns>
    /// An entity of type T
    /// </returns>
    public Task<T?> GetAsync(int id)
    {
        var entity = _dbContext.Set<T>().FirstOrDefaultAsync(x => x.Id == id) ?? throw new Exception($"Entity not found");
        return entity;
    }

    /// <summary>
    /// Add an entity
    /// </summary>
    /// <param name="entity"></param>

    public void Add(T entity)
    {
        _dbContext.Set<T>().Add(entity);
        _dbContext.SaveChanges();
    }

    /// <summary>
    /// Add an entity asynchronously
    /// </summary>
    /// <param name="entity"></param>
    /// <returns>
    /// An integer representing the number of rows affected
    /// </returns>
    public async Task<int> AddAsync(T entity)
    {
        await _dbContext.Set<T>().AddAsync(entity);
        return await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Update an entity
    /// </summary>
    /// <param name="entity"></param>
    public void Update(T entity)
    {
        _dbContext.Set<T>().Update(entity);
        _dbContext.SaveChanges();
    }

    /// <summary>
    /// Update an entity asynchronously
    /// </summary>
    /// <param name="entity"></param>
    /// <returns>
    /// Update an entity asynchronously
    /// </returns>  
    public async Task<int> UpdateAsync(T entity)
    {
        _dbContext.Set<T>().Update(entity);
        return await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Delete an entity
    /// </summary>
    /// <param name="id"></param>
    /// <exception cref="Exception"></exception> <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    public void Delete(int id)
    {
        var entity = _dbContext.Set<T>().FirstOrDefault(x => x.Id == id) ?? throw new Exception("Entity not found");
        _dbContext.Set<T>().Remove(entity);
        _dbContext.SaveChanges();
    }

    /// <summary>
    /// Delete an entity asynchronously
    /// </summary>
    /// <param name="id"></param>
    /// <returns>
    /// An integer representing the number of rows affected
    /// </returns>
    public async Task<int> DeleteAsync(int id)
    {
        var entity = _dbContext.Set<T>().FirstOrDefault(x => x.Id == id) ?? throw new Exception("Entity not found");
        _dbContext.Set<T>().Remove(entity);
        return await _dbContext.SaveChangesAsync();
    }

}
