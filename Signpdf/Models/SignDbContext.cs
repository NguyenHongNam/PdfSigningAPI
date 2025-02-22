﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Signpdf.Models
{
    public partial class SignDbContext : DbContext
    {
        public SignDbContext()
        {
        }

        public SignDbContext(DbContextOptions<SignDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Contract> Contracts { get; set; } = null!;
        public virtual DbSet<Signature> Signatures { get; set; } = null!;
        public virtual DbSet<Ocr> Ocrs { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=SignDb;Username=postgres;Password=namvip1234;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contract>(entity =>
            {
                entity.ToTable("contract");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("created_at");

                entity.Property(e => e.IsSigned).HasColumnName("is_signed");

                entity.Property(e => e.Path)
                    .HasMaxLength(255)
                    .HasColumnName("path");

                entity.Property(e => e.Title)
                    .HasMaxLength(255)
                    .HasColumnName("title");
            });

            modelBuilder.Entity<Signature>(entity =>
            {
                entity.ToTable("signature");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Contractid).HasColumnName("contractid");

                entity.Property(e => e.Height).HasColumnName("height");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.Page).HasColumnName("page");

                entity.Property(e => e.Reason).HasColumnName("reason");

                entity.Property(e => e.Width).HasColumnName("width");

                entity.Property(e => e.X).HasColumnName("x");

                entity.Property(e => e.Y).HasColumnName("y");

            });

            modelBuilder.Entity<Ocr>(entity =>
            {
                entity.ToTable("ocr");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.OcrText).HasColumnName("ocrtext");

                entity.Property(e => e.Created_at)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("created_at");

                entity.Property(e => e.Height).HasColumnName("height");

                entity.Property(e => e.Page).HasColumnName("page");

                entity.Property(e => e.Width).HasColumnName("width");

                entity.Property(e => e.X).HasColumnName("x");

                entity.Property(e => e.Y).HasColumnName("y");

            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
