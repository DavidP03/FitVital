﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebAPI.DAL;

#nullable disable

namespace FitVital.Migrations
{
    [DbContext(typeof(DataBaseContext))]
    [Migration("20240605064235_UpdateFitVitalDB")]
    partial class UpdateFitVitalDB
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.22")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("FitVital.DAL.Entities.Agenda", b =>
                {
                    b.Property<Guid>("CitaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("EntrenadorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("FechaHora")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UsuarioId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("CitaId");

                    b.HasIndex("CitaId")
                        .IsUnique();

                    b.HasIndex("EntrenadorId");

                    b.HasIndex("UsuarioId");

                    b.ToTable("Agendas");
                });

            modelBuilder.Entity("FitVital.DAL.Entities.Ejercicio", b =>
                {
                    b.Property<Guid>("EjercicioId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("DuracionMinutos")
                        .HasColumnType("int");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("EjercicioId");

                    b.HasIndex("EjercicioId")
                        .IsUnique();

                    b.ToTable("Ejercicios");
                });

            modelBuilder.Entity("FitVital.DAL.Entities.Entrenador", b =>
                {
                    b.Property<Guid>("EntrenadorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Activo")
                        .HasColumnType("bit");

                    b.Property<string>("Especialidad")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("EntrenadorId");

                    b.HasIndex("EntrenadorId")
                        .IsUnique();

                    b.ToTable("Entrenadores");
                });

            modelBuilder.Entity("FitVital.DAL.Entities.Usuario", b =>
                {
                    b.Property<Guid>("UsuarioId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Correo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Edad")
                        .HasColumnType("int");

                    b.Property<string>("Genero")
                        .IsRequired()
                        .HasMaxLength(9)
                        .HasColumnType("nvarchar(9)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Telefono")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UsuarioId");

                    b.HasIndex("UsuarioId")
                        .IsUnique();

                    b.ToTable("Usuarios");
                });

            modelBuilder.Entity("FitVital.DAL.Entities.Agenda", b =>
                {
                    b.HasOne("FitVital.DAL.Entities.Entrenador", "Entrenador")
                        .WithMany("Agenda")
                        .HasForeignKey("EntrenadorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FitVital.DAL.Entities.Usuario", "Usuario")
                        .WithMany("Agenda")
                        .HasForeignKey("UsuarioId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Entrenador");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("FitVital.DAL.Entities.Entrenador", b =>
                {
                    b.Navigation("Agenda");
                });

            modelBuilder.Entity("FitVital.DAL.Entities.Usuario", b =>
                {
                    b.Navigation("Agenda");
                });
#pragma warning restore 612, 618
        }
    }
}
