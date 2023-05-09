using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ApiERP.Models
{
    public partial class ERPContext : DbContext
    {
        public ERPContext()
        {
        }

        public ERPContext(DbContextOptions<ERPContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Componente> Componentes { get; set; } = null!;
        public virtual DbSet<EstatusComponente> EstatusComponentes { get; set; } = null!;
        public virtual DbSet<Gasolinero> Gasolineros { get; set; } = null!;
        public virtual DbSet<Menu> Menus { get; set; } = null!;
        public virtual DbSet<Monitor> Monitors { get; set; } = null!;
        public virtual DbSet<SolicitudCambio> SolicitudCambios { get; set; } = null!;
        public virtual DbSet<Usuario> Usuarios { get; set; } = null!;
        public virtual DbSet<UsuarioGasolinero> UsuarioGasolineros { get; set; } = null!;
        public virtual DbSet<UsuarioMenu> UsuarioMenus { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=servicios.redpacifico.com; Database=ERP; Integrated Security=False; Encrypt=False; User ID=sa; PWD=N0p4rK3m");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Componente>(entity =>
            {
                entity.ToTable("Componente");

                entity.Property(e => e.ComponenteId).HasColumnName("ComponenteID");

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Version)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<EstatusComponente>(entity =>
            {
                entity.ToTable("EstatusComponente");

                entity.Property(e => e.EstatusComponenteId).HasColumnName("EstatusComponenteID");

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Gasolinero>(entity =>
            {
                entity.ToTable("Gasolinero");

                entity.Property(e => e.GasolineroId)
                    .ValueGeneratedNever()
                    .HasColumnName("GasolineroID");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Menu>(entity =>
            {
                entity.ToTable("Menu");

                entity.Property(e => e.MenuId).HasColumnName("MenuID");

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PadreId).HasColumnName("PadreID");
            });

            modelBuilder.Entity<Monitor>(entity =>
            {
                entity.HasKey(e => e.MacAddress)
                    .HasName("PK_Monitor_MacAddress");

                entity.ToTable("Monitor");

                entity.Property(e => e.MacAddress)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Fecha).HasColumnType("datetime");

                entity.Property(e => e.GasolineroId).HasColumnName("GasolineroID");

                entity.Property(e => e.IpAddress)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Tipo)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Version)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Gasolinero)
                    .WithMany(p => p.Monitors)
                    .HasForeignKey(d => d.GasolineroId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_monitor_gasolinero");
            });

            modelBuilder.Entity<SolicitudCambio>(entity =>
            {
                entity.ToTable("SolicitudCambio");

                entity.Property(e => e.SolicitudCambioId)
                    .ValueGeneratedNever()
                    .HasColumnName("SolicitudCambioID");

                entity.Property(e => e.Commit)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ComponenteId).HasColumnName("ComponenteID");

                entity.Property(e => e.EstatusComponenteId).HasColumnName("EstatusComponenteID");

                entity.Property(e => e.Md5)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("MD5");

                entity.Property(e => e.Version)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Componente)
                    .WithMany(p => p.SolicitudCambios)
                    .HasForeignKey(d => d.ComponenteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SolicitudCambio_Componente");

                entity.HasOne(d => d.EstatusComponente)
                    .WithMany(p => p.SolicitudCambios)
                    .HasForeignKey(d => d.EstatusComponenteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SolicitudCambio_EstatusComponente");
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuario");

                entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");

                entity.Property(e => e.Clave)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Usuario1)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("Usuario");
            });

            modelBuilder.Entity<UsuarioGasolinero>(entity =>
            {
                entity.HasKey(e => new { e.UsuarioId, e.GasolineroId });

                entity.ToTable("UsuarioGasolinero");

                entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");

                entity.Property(e => e.GasolineroId).HasColumnName("GasolineroID");

                entity.HasOne(d => d.Usuario)
                    .WithMany(p => p.UsuarioGasolineros)
                    .HasForeignKey(d => d.UsuarioId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UsuarioCliente_UsuarioID");
            });

            modelBuilder.Entity<UsuarioMenu>(entity =>
            {
                entity.HasKey(e => new { e.UsuarioId, e.MenuId });

                entity.ToTable("UsuarioMenu");

                entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");

                entity.Property(e => e.MenuId).HasColumnName("MenuID");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
