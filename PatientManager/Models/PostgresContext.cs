using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PatientManager.Helpers;

namespace PatientManager.Models;

public partial class PostgresContext : DbContext
{
    public PostgresContext()
    {
    }

    public PostgresContext(DbContextOptions<PostgresContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Attachment> Attachments { get; set; }

    public virtual DbSet<Disease> Diseases { get; set; }

    public virtual DbSet<Examination> Examinations { get; set; }

    public virtual DbSet<Examinationtype> Examinationtypes { get; set; }

    public virtual DbSet<Medicalfile> Medicalfiles { get; set; }

    public virtual DbSet<Medicalhistory> Medicalhistories { get; set; }

    public virtual DbSet<Medication> Medications { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<Prescription> Prescriptions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=aws-0-eu-central-1.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.mhjlkjzxcozlfvgepomi;Password=XSBFV49s.;SSL Mode=Require;Trust Server Certificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var dateOnlyConverter = new DateOnlyConverter();

        modelBuilder
            .HasPostgresEnum("auth", "aal_level", new[] { "aal1", "aal2", "aal3" })
            .HasPostgresEnum("auth", "code_challenge_method", new[] { "s256", "plain" })
            .HasPostgresEnum("auth", "factor_status", new[] { "unverified", "verified" })
            .HasPostgresEnum("auth", "factor_type", new[] { "totp", "webauthn", "phone" })
            .HasPostgresEnum("auth", "one_time_token_type", new[] { "confirmation_token", "reauthentication_token", "recovery_token", "email_change_token_new", "email_change_token_current", "phone_change_token" })
            .HasPostgresEnum("pgsodium", "key_status", new[] { "default", "valid", "invalid", "expired" })
            .HasPostgresEnum("pgsodium", "key_type", new[] { "aead-ietf", "aead-det", "hmacsha512", "hmacsha256", "auth", "shorthash", "generichash", "kdf", "secretbox", "secretstream", "stream_xchacha20" })
            .HasPostgresEnum("realtime", "action", new[] { "INSERT", "UPDATE", "DELETE", "TRUNCATE", "ERROR" })
            .HasPostgresEnum("realtime", "equality_op", new[] { "eq", "neq", "lt", "lte", "gt", "gte", "in" })
            .HasPostgresExtension("extensions", "pg_stat_statements")
            .HasPostgresExtension("extensions", "pgcrypto")
            .HasPostgresExtension("extensions", "pgjwt")
            .HasPostgresExtension("extensions", "uuid-ossp")
            .HasPostgresExtension("graphql", "pg_graphql")
            .HasPostgresExtension("pgsodium", "pgsodium")
            .HasPostgresExtension("vault", "supabase_vault");

        modelBuilder.Entity<Attachment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("attachments_pkey");

            entity.ToTable("attachments");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ExaminationId).HasColumnName("examination_id");
            entity.Property(e => e.FileName)
                .HasMaxLength(255)
                .HasColumnName("file_name");
            entity.Property(e => e.FilePath)
                .HasMaxLength(255)
                .HasColumnName("file_path");
            entity.Property(e => e.UploadedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("uploaded_at");

            entity.HasOne(d => d.Examination).WithMany(p => p.Attachments)
                .HasForeignKey(d => d.ExaminationId)
                .HasConstraintName("attachments_examination_id_fkey");
        });

        modelBuilder.Entity<Disease>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("disease_pkey");

            entity.ToTable("disease");

            entity.HasIndex(e => e.Name, "disease_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Examination>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("examinations_pkey");

            entity.ToTable("examinations");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Date)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.TypeId).HasColumnName("type_id");

            entity.HasOne(d => d.Patient).WithMany(p => p.Examinations)
                .HasForeignKey(d => d.PatientId)
                .HasConstraintName("examinations_patient_id_fkey");

            entity.HasOne(d => d.Type).WithMany(p => p.Examinations)
                .HasForeignKey(d => d.TypeId)
                .HasConstraintName("examinations_type_id_fkey");

        });

        modelBuilder.Entity<Examinationtype>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("examinationtype_pkey");

            entity.ToTable("examinationtype");

            entity.HasIndex(e => e.Code, "examinationtype_code_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(10)
                .HasColumnName("code");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Medicalfile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("medicalfiles_pkey");

            entity.ToTable("medicalfiles");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ExaminationId).HasColumnName("examination_id");
            entity.Property(e => e.FilePath)
                .HasMaxLength(255)
                .HasColumnName("file_path");

            entity.HasOne(d => d.Examination).WithMany(p => p.Medicalfiles)
                .HasForeignKey(d => d.ExaminationId)
                .HasConstraintName("medicalfiles_examination_id_fkey");
        });

        modelBuilder.Entity<Medicalhistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("medicalhistory_pkey");

            entity.ToTable("medicalhistory");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DiseaseEnd).HasColumnName("disease_end");
            entity.Property(e => e.DiseaseId).HasColumnName("disease_id");
            entity.Property(e => e.DiseaseStart).HasColumnName("disease_start");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");

            entity.HasOne(d => d.Disease).WithMany(p => p.Medicalhistories)
                .HasForeignKey(d => d.DiseaseId)
                .HasConstraintName("medicalhistory_disease_id_fkey");

            entity.HasOne(d => d.Patient).WithMany(p => p.Medicalhistories)
                .HasForeignKey(d => d.PatientId)
                .HasConstraintName("medicalhistory_patient_id_fkey");
        });

        modelBuilder.Entity<Medication>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("medication_pkey");

            entity.ToTable("medication");

            entity.HasIndex(e => e.Name, "medication_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("patients_pkey");

            entity.ToTable("patients");

            entity.HasIndex(e => e.Oib, "patients_oib_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BirthDate).HasColumnName("birth_date");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Oib)
                .HasMaxLength(11)
                .HasColumnName("oib");
            entity.Property(e => e.Spol)
                .HasMaxLength(10)
                .HasColumnName("spol");
        });

        modelBuilder.Entity<Prescription>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("prescriptions_pkey");

            entity.ToTable("prescriptions");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.MedicationId).HasColumnName("medication_id");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.PrescriptionDate).HasColumnName("prescription_date");

            entity.HasOne(d => d.Medication).WithMany(p => p.Prescriptions)
                .HasForeignKey(d => d.MedicationId)
                .HasConstraintName("prescriptions_medication_id_fkey");

            entity.HasOne(d => d.Patient).WithMany(p => p.Prescriptions)
                .HasForeignKey(d => d.PatientId)
                .HasConstraintName("prescriptions_patient_id_fkey");
        });
        modelBuilder.HasSequence<int>("seq_schema_version", "graphql").IsCyclic();

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
