using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AIITSM.Infrastructure.Data;

public partial class ITServiceDeskContext : DbContext
{
    public ITServiceDeskContext()
    {
    }

    public ITServiceDeskContext(DbContextOptions<ITServiceDeskContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Aianalysis> Aianalyses { get; set; }

    public virtual DbSet<AianalysisRelatedIncident> AianalysisRelatedIncidents { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Escalation> Escalations { get; set; }

    public virtual DbSet<Incident> Incidents { get; set; }

    public virtual DbSet<IncidentAssignment> IncidentAssignments { get; set; }

    public virtual DbSet<IncidentComment> IncidentComments { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=ITServiceDesk;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Aianalysis>(entity =>
        {
            entity.HasKey(e => e.AianalysisId).HasName("PK__AIAnalys__D290B95AE966629F");

            entity.ToTable("AIAnalysis");

            entity.Property(e => e.AianalysisId).HasColumnName("AIAnalysisId");
            entity.Property(e => e.ConfidenceScore).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pending", "DF_AIAnalysis_Status");
            entity.Property(e => e.SuggestedCategory)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.SuggestedPriority)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SuggestedResolution).IsUnicode(false);

            entity.HasOne(d => d.Incident).WithMany(p => p.Aianalyses)
                .HasForeignKey(d => d.IncidentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AIAnalysi__Incid__656C112C");
        });

        modelBuilder.Entity<AianalysisRelatedIncident>(entity =>
        {
            entity.HasKey(e => e.AianalysisRelatedIncidentId).HasName("PK__AIAnalys__9E4A3F6E8B2D703F");

            entity.ToTable("AIAnalysisRelatedIncident");

            entity.Property(e => e.AianalysisRelatedIncidentId).HasColumnName("AIAnalysisRelatedIncidentId");
            entity.Property(e => e.AianalysisId).HasColumnName("AIAnalysisId");
            entity.Property(e => e.RelationshipType)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.SimilarityScore).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.Aianalysis).WithMany(p => p.AianalysisRelatedIncidents)
                .HasForeignKey(d => d.AianalysisId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AIAnalysi__AIAna__0E6E26BF");

            entity.HasOne(d => d.RelatedIncident).WithMany(p => p.AianalysisRelatedIncidents)
                .HasForeignKey(d => d.RelatedIncidentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AIAnalysi__Relat__0F624AF8");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A0BBAC5736B");

            entity.HasIndex(e => e.CategoryName, "UQ__Categori__8517B2E0077FB8C9").IsUnique();

            entity.Property(e => e.CategoryName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Escalation>(entity =>
        {
            entity.HasKey(e => e.EscalationId).HasName("PK__Escalati__6C7956D0A03B9137");

            entity.Property(e => e.EscalatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Reason)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.ResolvedAt).HasColumnType("datetime");

            entity.HasOne(d => d.EscalatedByNavigation).WithMany(p => p.EscalationEscalatedByNavigations)
                .HasForeignKey(d => d.EscalatedBy)
                .HasConstraintName("FK__Escalatio__Escal__70DDC3D8");

            entity.HasOne(d => d.EscalatedToNavigation).WithMany(p => p.EscalationEscalatedToNavigations)
                .HasForeignKey(d => d.EscalatedTo)
                .HasConstraintName("FK__Escalatio__Escal__71D1E811");

            entity.HasOne(d => d.Incident).WithMany(p => p.Escalations)
                .HasForeignKey(d => d.IncidentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Escalatio__Incid__6FE99F9F");
        });

        modelBuilder.Entity<Incident>(entity =>
        {
            entity.HasKey(e => e.IncidentId).HasName("PK__Incident__3D8053B28A3C50A8");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.Priority)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ResolvedAt).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Open");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.Category).WithMany(p => p.Incidents)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Incidents__Categ__571DF1D5");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Incidents)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Incidents__Creat__5812160E");
        });

        modelBuilder.Entity<IncidentAssignment>(entity =>
        {
            entity.HasKey(e => e.AssignmentId).HasName("PK__Incident__32499E77D5F666AC");

            entity.Property(e => e.AssignedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.AssignedToNavigation).WithMany(p => p.IncidentAssignments)
                .HasForeignKey(d => d.AssignedTo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__IncidentA__Assig__5CD6CB2B");

            entity.HasOne(d => d.Incident).WithMany(p => p.IncidentAssignments)
                .HasForeignKey(d => d.IncidentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__IncidentA__Incid__5BE2A6F2");
        });

        modelBuilder.Entity<IncidentComment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Incident__C3B4DFCA5B4DD186");

            entity.Property(e => e.CommentText).IsUnicode(false);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Incident).WithMany(p => p.IncidentComments)
                .HasForeignKey(d => d.IncidentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__IncidentC__Incid__60A75C0F");

            entity.HasOne(d => d.User).WithMany(p => p.IncidentComments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__IncidentC__UserI__619B8048");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E12F08AF824");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Message)
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.HasOne(d => d.Incident).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.IncidentId)
                .HasConstraintName("FK__Notificat__Incid__6C190EBB");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__UserI__6B24EA82");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1A69205298");

            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C458FDF3A");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534E5456BCD").IsUnique();

            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__RoleId__4F7CD00D");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
