using DataAccessAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessAPI
{
    public class CountryContext : DbContext
    {
        public DbSet<Country> Countries { get; set; }

        public CountryContext(DbContextOptions<CountryContext> options) : base(options) { }
    }
}
