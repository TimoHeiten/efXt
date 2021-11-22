using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace tests
{
    public class TestDbContext : DbContext
    {
        public TestDbContext([NotNullAttribute] DbContextOptions options) 
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            var e = mb.Entity<Entity>();
            e.HasKey(x => x.Id);
        }
    }

    public class Entity 
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public Dependent Dependent { get; set; }

    }

    public class Dependent 
    {
        public int Id { get; set; }
        public int Content { get; set; }
        public ICollection<Entity> Entities { get; set; }
    }
}