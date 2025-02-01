using AnalysisEngine.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace AnalysisEngine.Infrastructure.Data
{
    public class AnalysisEngineDbContext : DbContext
    {
        public AnalysisEngineDbContext(DbContextOptions<AnalysisEngineDbContext> options) : base(options) { }
        public DbSet<AnalysisLog> AnalysisLog { get; set; }
    }
}