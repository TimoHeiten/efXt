using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using heitech.efXt;
using heitech.efXt.Read;
using Xunit;

namespace tests;

public sealed class RegisterComponentTests : IDisposable
{
    private readonly TestContext _testContext;

    public RegisterComponentTests()
        => _testContext = new TestContext();

    [Theory]
    [MemberData(nameof(ExpectedServices))]
    public void Register_adds_all_Repositories(Type expectedService)
    {
        // Arrange
        // Act
        var repository = _testContext.GetService(expectedService);

        // Assert
        repository.Should().NotBeNull();
        repository.Should().BeAssignableTo(expectedService);
    }

    public static IEnumerable<object[]> ExpectedServices
    {
        get
        {
            yield return new object[] { typeof(IRepository<Entity, int>) };
            yield return new object[] { typeof(IRepository<EntityV2, int>) };
            yield return new object[] { typeof(IUnitOfWork) };
        }
    }

    #region Test Services
    // ReSharper disable once ClassNeverInstantiated.Local
    private sealed class Entity { public int Id { get; set; } }
    private sealed class TestRepository : IRepository<Entity, int>
    {
        public IQueryable<Entity> AsQueryable()
        {
            throw new NotImplementedException();
        }

        public Task<Entity> GetByIdAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<Entity> GetByAsync(ISpecification<Entity> specification)
        {
            throw new System.NotImplementedException();
        }

        public Task<IReadOnlyList<Entity>> GetAllAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<IReadOnlyList<Entity>> GetAllBySpecificationAsync(ISpecification<Entity> specification)
        {
            throw new System.NotImplementedException();
        }
    }
    
    private sealed class EntityV2 { public int Id { get; set; } }
    private sealed class TestRepositoryV2 : GenericRepository<TestDbContext, EntityV2, int>
    {
        public TestRepositoryV2(TestDbContext dbContext) : base(dbContext)
        {
        }

        public override Task<EntityV2> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
    #endregion


    public void Dispose()
        => _testContext.Dispose();
}