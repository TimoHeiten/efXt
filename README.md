# efXt

Aim of this library:
* Use Ef Core with common functionality so you dont have to implement it over and over again.
  * IRepository<TEntity> interface
  * Specification pattern for repeatable 
  * UnitOfWork and Rollback for better control of the ChangeTracking 
* Make it easy to register the dbContext and setup code via the Microsoft.Extensions.DependencyInjection
* There is also an abstract class for the Repositories (GenericRepository<TEntity, TId>) which implements the most basic functions already
* All implementations of the IRepository<,> interface are also automatically added to the Dependency Injection (any assembly in the dependent ones of the executing assembly)
 
## IUnitOfWork
 ```csharp
/// <summary>
/// Represents all Commands to interact with the database and have a 
/// Transaction scope in the Application before committing the changes to the database
/// </summary>
public interface IUnitOfWork : IAsyncDisposable
{
    void Add<T>(T one)
        where T : class;
    void  AddMany<T>(IEnumerable<T> many)
        where T : class;

    void Update<T>(T one)
        where T : class;
    void UpdateMany<T>(IEnumerable<T> many)
        where T : class;

    void Delete<T>(T one)
        where T : class;
    void DeleteMany<T>(IEnumerable<T> many)
        where T : class;

    ///<summary>
    /// Resets all changes on this entity, so it will not be commited to the database on Save
    ///</summary>
    void RollbackOne<T>(T entity, Func<T, bool> predicate)
        where T : class;

    ///<summary>
    /// Resets all changes on all changed entities, so it will not be commited to the database on Save
    ///</summary>
    void RollBackAll();

    ///<summary>
    /// Commit all the changes made to any entities since the start of the UnitOfWork
    ///</summary>
    Task SaveAsync();
}
 ```

## IRepository<TEntity, TId>
 ```csharp
///<summary>
/// Abstraction for DataAccess with generic and Specification methods
///</summary>
public interface IRepository<TEntity, TId>
        where TEntity : class
{
    /// <summary>
    /// for special cases operate directly on the IQueryable
    /// </summary>
    IQueryable<TEntity> AsQueryable();

    Task<TEntity> GetByIdAsync(TId id);
    Task<TEntity> GetByIdAsync(ISpecification<TEntity> specification);

    Task<IReadOnlyList<TEntity>> GetAllAsync();
    Task<IReadOnlyList<TEntity>> GetAllBySpecificationAsync(ISpecification<TEntity> specification);
}
 ```

## ISpecification<TEntity>
 ```csharp
 ///<summary>
/// Provides Filtering capabilities for a Query on the Repository calls.
///</summary>
    public interface ISpecification<TEntity>
        where TEntity : class
{
    Expression<Func<TEntity, bool>> Build();
}
 ```

 ## GenericRepositry<TEntity, TId>
 ```csharp
public abstract class GenericRepository<TEntity, TId> : IRepository<TEntity, TId>
    where TEntity : class
{
    protected DbContext Context { get; }

    public GenericRepository(DbContext dbContext)
        => Context = dbContext;

    public abstract Task<TEntity> GetByIdAsync(TId id);
    
    public IQueryable<TEntity> AsQueryable() => Context.Set<TEntity>().AsQueryable();

    public Task<TEntity> GetByIdAsync(ISpecification<TEntity> specification) 
        => Context.Set<TEntity>().FirstOrDefaultAsync(specification.Build());

    public async Task<IReadOnlyList<TEntity>> GetAllAsync()
        => await Context.Set<TEntity>().ToListAsync();

    public async Task<IReadOnlyList<TEntity>> GetAllBySpecificationAsync(ISpecification<TEntity> specification)
        => await Context.Set<TEntity>().Where(specification.Build()).ToListAsync();
}
 ```

 
 ## Examples
See some examples below. We assume Asp.Net Core for this Scenario. Else you´d need to use the static Factory class and manage the instances yourself.
First Register it in the Startup of Asp.Net Core
```csharp
// example with SqlServer, but any Action on DbContextOptionsBuilder are allowed.
 services.RegisterEfAbstraction<MyDbContext>(options => options.UseSqlServer("connectionstring"));
```

Assume the following entities as the POCOS for EF Core
```csharp
public class Entity
{
    public int Id { get; set; }
    public string Note { get; set; }
}
```

Example for all interfaces in some fictious usecases for our example Entity
 ```csharp
public class EntityUseCases
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Entity, int> _repository;

    public EntityUseCases(IRepository<Entity, int> repository, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public Task AddEntity(int id, string note)
    {
        _unitOfWork.Add(new Entity { Id = id, Note = note });
        return _unitOfWork.SaveAsync();
    }

    public async Task UpdateNote(int id, string note)
    {
        Entity entity = await _repository.GetByIdAsync(id);
        // change tracking on db context already marks the Entity as changed
        // no need to call _unitOfOwrk.Update, only if an entity is not attached yet (e.g. when using AsNoTracking)
        entity.Note = note;
        await _unitOfWork.SaveAsync();
    }

    // it is also possible to provide multiple entities for each call to Add, Update or Delete with the Many Suffix
    public async Task DeleteAllEntities()
    {
        IReadOnlyList<Entity> entities = await _repository.GetAllAsync();
        _unitOfWork.DeleteMany(entities);
        await _unitOfWork.SaveAsync();
    }

    // read use case with special filter (Specifications also work for )
    public Task<IReadOnlyList<Entity>> GetEntitiesWithSpecificNote()
    {
        var specification = new NoteShorterThanThreeCharsSpecification();
        return _repository.GetAllBySpecificationAsync(specification);
    }

    private class NoteShorterThanThreeCharsSpecification : ISpecification<Entity>
    {
        public Expression<Func<Entity, bool>> Build()
            => entity => entity.Note.Length <= 3;
    }
}
 ```
 
 ## Example for a specific Query you still want to be able to test without the dbContext
 ```csharp
public interface IFirstTenEntitiesQuery
{
    Task<IReadOnlyList<Entity>> ExecuteAsync();
}

public class Query : IFirstTenEntitiesQuery
{
    private readonly DbContext _context;

    public Query(DbContext repository)
        => _context = repository;

    public async Task<IReadOnlyList<Entity>> ExecuteAsync()
        => await _context.Set<Entity>()
                            .Take(10)
                            .ToListAsync();
}
 ```

 Example for Specified Query (testable)
  * Specification pattern for repeatable queries
  * UnitOfWork and Rollback for better control of the ChangeTracking  
* Make it easy to register the dbContext and let the abstractions be injected so you can test your code without repetitive mocks for Repositories and such

Examples tbd.
