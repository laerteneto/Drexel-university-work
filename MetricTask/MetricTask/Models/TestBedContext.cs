using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using MetricTask.Models.Mapping;

namespace MetricTask.Models
{
    public partial class TestBedContext : DbContext
    {
        static TestBedContext()
        {
            Database.SetInitializer<TestBedContext>(null);
        }

        public TestBedContext()
            : base("Name=TestBedContext")
        {
        }

        public DbSet<Metric> Metrics { get; set; }
        public DbSet<sysdiagram> sysdiagrams { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Task_Metric> Task_Metric { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new MetricMap());
            modelBuilder.Configurations.Add(new sysdiagramMap());
            modelBuilder.Configurations.Add(new TaskMap());
            modelBuilder.Configurations.Add(new Task_MetricMap());
        }
    }
}