using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using heitech.efXt;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace tests
{
    public class QueryTests : IDisposable
    {
        private static string connectionString => "Filename=:memory:";
        private readonly SqliteConnection connection;
        private readonly IServiceProvider provider;
        private IServiceScope scope;
        private TestDbContext evaluatorDbContext;
        public QueryTests()
        {
            this.connection = new SqliteConnection(connectionString);
            connection.Open();

            var collection = new ServiceCollection();
            collection.RegisterEfAbstraction<TestDbContext>(options => options.UseSqlite(connection));

            provider = collection.BuildServiceProvider();
            scope = provider.CreateScope();
            evaluatorDbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();
            evaluatorDbContext.Database.EnsureCreated();
        }

        [Fact]
        public void Resolve_QueryBuilder_for_all_Types_with_IhasId_works()
        {
            // Arrange
            var queryFactory = scope.ServiceProvider.GetRequiredService<IQueryBuilderFactory>();

            // Act
            var entityQueryBuilder = queryFactory.Resolve<Entity, int>();
            var dependentQueryBuilder = queryFactory.Resolve<Dependent, int>();
            
            // Assert
            Assert.NotNull(entityQueryBuilder);
            Assert.NotNull(dependentQueryBuilder);
        }

        [Theory]
        [MemberData(nameof(MutationCalls))]
        public async Task MutateDatabase(Func<IUnitOfWork, Task> mutation, Func<TestDbContext, Task> assertion)
            => await Mutate(mutation, assertion);

        private async Task Mutate(Func<IUnitOfWork, Task> mutation, Func<TestDbContext, Task> assertion)
        {
            // Arrange
            var commands = scope.ServiceProvider.GetRequiredService<ICommands>();
            var unitOfWork = commands.GetUnitOfWork();

            // Act
            await mutation(unitOfWork);

            // Assert
            await assertion(this.evaluatorDbContext);
        }

        [Fact]
        public async Task Full_Integration_with_insert_and_query()
        {
            // Arrange
            var commands = scope.ServiceProvider.GetRequiredService<ICommands>();
            var unitOfWork = commands.GetUnitOfWork();
            var e = await Insert(unitOfWork);
            var queryBuilderFactory = scope.ServiceProvider.GetRequiredService<IQueryBuilderFactory>();
            var queryBuilder = queryBuilderFactory.Resolve<Entity, int>();

            // Act
            var result = await queryBuilder.SelectWithId(e.Id)
                                           .Include(x => x.Dependent)
                                           .Create()
                                           .PrjectIntoAsync
                                           (
                                               x => new 
                                               {
                                                    EntityId = x.Id, 
                                                    DependentId = x.Dependent.Id, 
                                                    ContentCombined = $"{x.Content} - {x.Dependent.Content}"
                                               }
                                            );

            // Assert
            Assert.Equal(e.Id, result.EntityId);
            Assert.Equal(e.Dependent.Id, result.DependentId);
            Assert.StartsWith(e.Content, result.ContentCombined);
            Assert.EndsWith($"{e.Dependent.Content}", result.ContentCombined);
        }

        public static IEnumerable<object[]> MutationCalls => 
            new []
            {
                // Insert data
                new object[]
                {
                    new Func<IUnitOfWork, Task>(Insert),
                    new Func<TestDbContext, Task>(async dbContext => Assert.Single((await dbContext.Set<Entity>().ToListAsync())))
                },
                // Insert and update
                new object[]
                {
                    new Func<IUnitOfWork, Task>(async unit => 
                        {
                            var e = await Insert(unit); 
                            e.Content = "updated";
                            unit.Update<Entity, int>(e);
                            await unit.SaveAsync();
                        }),
                    new Func<TestDbContext, Task>(async dbContext => 
                        {
                            var updated = await dbContext.Set<Entity>().SingleAsync();
                            Assert.Equal("updated", updated.Content);
                        })
                },
                // Insert and delete
                new object[]
                {
                    new Func<IUnitOfWork, Task>(async unit => 
                        {
                            var e = await Insert(unit);
                            unit.Delete<Entity, int>(e);
                            await unit.SaveAsync();
                        }),
                    new Func<TestDbContext, Task>(async dbContext => 
                        {
                            var result = await dbContext.Set<Entity>().ToListAsync();
                            Assert.Empty(result);
                        })
                }

                // todo
                // multiple update, multiple insert, transaction
            };

        private static async Task<Entity> Insert(IUnitOfWork unitOfWork)
        {
            var e = new Entity { Content = "42", Dependent = new Dependent { Content = 42}};
            unitOfWork.Add<Entity, int>(e);
            await unitOfWork.SaveAsync();
            return e;
        }

        [Fact]
        public async Task Rollback_one_update_does_not_change_the_db()
        {
            await Mutate
            (
                async u => {
                    var d = new Dependent { Id = 1, Content = 42 };
                    u.Add<Dependent, int>(d);
                    u.RollbackOne<Dependent, int>(d);
                    await u.SaveAsync();
                },
                async db => 
                {
                    var dependents = await db.Set<Dependent>().ToListAsync();
                    Assert.Empty(dependents);
                }
            );
        }

        [Fact]
        public async Task Rollback_all_does_not_change_db_on_saving()
        {
            await Mutate
            (
                async u => {
                    Func<int, Dependent> factory = i => new Dependent { Id = i, Content = i };
                    var d = new Dependent { Id = 11, Content = 42 };
                    u.Add<Dependent, int>(d, Enumerable.Range(0, 10).Select(i => factory(i)).ToArray());
                    u.RollBack();
                    await u.SaveAsync();
                },
                async db => 
                {
                    var dependents = await db.Set<Dependent>().ToListAsync();
                    Assert.Empty(dependents);
                }
            );
        }

        public void Dispose()
        {
            scope.Dispose();
        }
    }
}
