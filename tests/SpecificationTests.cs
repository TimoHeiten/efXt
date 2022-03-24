using Xunit;
using heitech.efXt;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;

namespace tests
{
    public class SpecificationTests
    {
        [Fact]
        public async Task Specification_works_with_List()
        {
            // Given
            var source = Enumerable.Range(0, 14).Select(x => new TextContainer()).AsQueryable();
            var spec = new FindNameEqMimiSpecification();

            // When
            var listResult = await spec.RunAsync(source);
            var singleResult = await spec.RunSingleAsync(source);

            // Then
            listResult.Should().HaveCount(7)
                      .And.OnlyContain(x => x.Name == "Mimi");
            singleResult.Should().NotBeNull();
            singleResult.Name.Should().Be("Mimi");
        }

        private class TextContainer
        {
            static int i = 0;
            public string Name { get; }
            public TextContainer()
            {
                if (i % 2 == 0)
                    Name = "Mimi";
                else
                    Name = "AbcAffeSchnee";

                i++;
            }
        }

        private class FindNameEqMimiSpecification : IReadSpecification<TextContainer>
        {
            public Task<IReadOnlyList<TextContainer>> RunAsync(IQueryable<TextContainer> query)
                => Task.FromResult<IReadOnlyList<TextContainer>>(query.Where(x => x.Name == "Mimi").ToList());

            public Task<TextContainer> RunSingleAsync(IQueryable<TextContainer> query)
                => Task.FromResult(query.FirstOrDefault(x => x.Name == "Mimi"));
        }
    }
}