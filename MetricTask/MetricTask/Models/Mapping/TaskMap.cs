using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MetricTask.Models.Mapping
{
    public class TaskMap : EntityTypeConfiguration<Task>
    {
        public TaskMap()
        {
            // Primary Key
            this.HasKey(t => t.Task_ID);

            // Properties
            this.Property(t => t.Task_Name)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("Task");
            this.Property(t => t.Task_ID).HasColumnName("Task_ID");
            this.Property(t => t.Task_Name).HasColumnName("Task_Name");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.TCI).HasColumnName("TCI");
            this.Property(t => t.EI).HasColumnName("EI");
            this.Property(t => t.EI_Tolerance).HasColumnName("EI_Tolerance");
        }
    }
}
