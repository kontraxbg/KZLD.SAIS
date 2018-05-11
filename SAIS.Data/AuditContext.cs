using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SAIS.Data
{
    public partial class AuditContext : DbContext
    {
        public virtual DbSet<Audit> Audits { get; set; }
        public virtual DbSet<AuditConfig> AuditConfigs { get; set; }
        public virtual DbSet<AuditDetail> AuditDetails { get; set; }
        public virtual DbSet<AuditType> AuditTypes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=TestSql;Database=KzldSaisDev;User Id=sa;Password=kontrax;MultipleActiveResultSets=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Audit>(entity =>
            {
                entity.ToTable("Audit", "audit");

                entity.HasIndex(e => e.AuditTypeCode);

                entity.HasIndex(e => e.DateTime);

                entity.Property(e => e.Action).HasMaxLength(1000);

                entity.Property(e => e.AuditTypeCode)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Controller).HasMaxLength(1000);

                entity.Property(e => e.EntityName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.EntityRecordId).HasMaxLength(100);

                entity.Property(e => e.Hash).HasMaxLength(128);

                entity.Property(e => e.IpAddress).HasMaxLength(100);

                entity.Property(e => e.RequestMethod).HasMaxLength(100);

                entity.Property(e => e.SessionId).HasMaxLength(1000);

                entity.Property(e => e.Url).HasMaxLength(1000);

                entity.Property(e => e.UserId).HasMaxLength(128);

                entity.Property(e => e.UserName).HasMaxLength(256);

                entity.HasOne(d => d.AuditTypeCodeNavigation)
                    .WithMany(p => p.Audits)
                    .HasForeignKey(d => d.AuditTypeCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Audit_AuditType");
            });

            modelBuilder.Entity<AuditConfig>(entity =>
            {
                entity.HasKey(e => new { e.EntityName, e.PropertyName });

                entity.ToTable("AuditConfig", "audit");

                entity.Property(e => e.EntityName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PropertyName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Mapping)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Translation).HasMaxLength(100);
            });

            modelBuilder.Entity<AuditDetail>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .ForSqlServerIsClustered(false);

                entity.ToTable("AuditDetail", "audit");

                entity.Property(e => e.AuditDetailType)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.EntityName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PropertyName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.RecordId)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.Audit)
                    .WithMany(p => p.AuditDetails)
                    .HasForeignKey(d => d.AuditId)
                    .HasConstraintName("FK_AuditDetail_Audit");
            });

            modelBuilder.Entity<AuditType>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.ToTable("AuditType", "audit");

                entity.Property(e => e.Code)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });
        }
    }
}
