using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AnalisisPrice;

public partial class AnalysisPriceBotDbContext : DbContext
{
    public AnalysisPriceBotDbContext()
    {
    }

    public AnalysisPriceBotDbContext(DbContextOptions<AnalysisPriceBotDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Analisi> Analises { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("DB data connection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Analisi>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("analisis_pkey");

            entity.ToTable("analisis");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AnalisisName)
                .HasMaxLength(300)
                .HasColumnName("analisis_name");
            entity.Property(e => e.AnalisisPrice).HasColumnName("analisis_price");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
