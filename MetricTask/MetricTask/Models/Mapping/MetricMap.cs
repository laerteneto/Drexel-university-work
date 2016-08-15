using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MetricTask.Models.Mapping
{
    public class MetricMap : EntityTypeConfiguration<Metric>
    {
        public MetricMap()
        {
            // Primary Key
            this.HasKey(t => t.Metric_Name);

            // Properties
            this.Property(t => t.Metric_Name)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Metric_Description)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("Metric");
            this.Property(t => t.Metric_Name).HasColumnName("Metric_Name");
            this.Property(t => t.Metric_Description).HasColumnName("Metric_Description");
        }
    }
}
