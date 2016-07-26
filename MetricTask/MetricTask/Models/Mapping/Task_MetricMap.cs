using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MetricTask.Models.Mapping
{
    public class Task_MetricMap : EntityTypeConfiguration<Task_Metric>
    {
        public Task_MetricMap()
        {
            // Primary Key
            this.HasKey(t => new { t.Task_ID, t.Metric_Name });

            // Properties
            this.Property(t => t.Metric_Name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Task_Metric");
            this.Property(t => t.Task_ID).HasColumnName("Task_ID");
            this.Property(t => t.Metric_Name).HasColumnName("Metric_Name");
            this.Property(t => t.Metric_Value).HasColumnName("Metric_Value");

            // Relationships
            this.HasRequired(t => t.Metric)
                .WithMany(t => t.Task_Metric)
                .HasForeignKey(d => d.Metric_Name);
            this.HasRequired(t => t.Task)
                .WithMany(t => t.Task_Metric)
                .HasForeignKey(d => d.Task_ID);

        }
    }
}
