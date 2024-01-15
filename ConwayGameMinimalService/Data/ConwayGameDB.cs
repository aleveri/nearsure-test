using ConwayGameMinimalService.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConwayGameMinimalService.Data
{
    public class ConwayGameDB : DbContext
    {
        public ConwayGameDB(DbContextOptions<ConwayGameDB> options) : base(options) { }

        public DbSet<GameOfLife> GamesOfLife => Set<GameOfLife>();
    }
}
