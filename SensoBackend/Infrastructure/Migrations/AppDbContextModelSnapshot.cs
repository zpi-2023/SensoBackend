﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SensoBackend.Infrastructure.Data;

#nullable disable

namespace SensoBackend.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("SensoBackend.Domain.Entities.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("Active")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("LastLoginAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("LastPasswordChangeAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<int>("RoleId")
                        .HasColumnType("integer");

                    b.Property<bool>("Verified")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("Accounts", (string)null);
                });

            modelBuilder.Entity("SensoBackend.Domain.Entities.DashboardItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AccountId")
                        .HasColumnType("integer");

                    b.Property<int>("GadgetId")
                        .HasColumnType("integer");

                    b.Property<int>("Position")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("GadgetId");

                    b.ToTable("DashboardItems", (string)null);
                });

            modelBuilder.Entity("SensoBackend.Domain.Entities.Gadget", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Gadgets", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "openMenu"
                        },
                        new
                        {
                            Id = 2,
                            Name = "switchProfile"
                        },
                        new
                        {
                            Id = 3,
                            Name = "logOut"
                        },
                        new
                        {
                            Id = 4,
                            Name = "activateSos"
                        },
                        new
                        {
                            Id = 5,
                            Name = "pairCaretaker"
                        },
                        new
                        {
                            Id = 6,
                            Name = "editDashboard"
                        },
                        new
                        {
                            Id = 7,
                            Name = "toggleLanguage"
                        },
                        new
                        {
                            Id = 8,
                            Name = "trackMedication"
                        },
                        new
                        {
                            Id = 9,
                            Name = "playGames"
                        },
                        new
                        {
                            Id = 10,
                            Name = "manageNotes"
                        });
                });

            modelBuilder.Entity("SensoBackend.Domain.Entities.Profile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AccountId")
                        .HasColumnType("integer");

                    b.Property<string>("Alias")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("SeniorId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("SeniorId");

                    b.ToTable("Profiles", (string)null);
                });

            modelBuilder.Entity("SensoBackend.Domain.Entities.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Roles", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Admin"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Member"
                        });
                });

            modelBuilder.Entity("SensoBackend.Domain.Entities.Account", b =>
                {
                    b.HasOne("SensoBackend.Domain.Entities.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("SensoBackend.Domain.Entities.DashboardItem", b =>
                {
                    b.HasOne("SensoBackend.Domain.Entities.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SensoBackend.Domain.Entities.Gadget", "Gadget")
                        .WithMany()
                        .HasForeignKey("GadgetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("Gadget");
                });

            modelBuilder.Entity("SensoBackend.Domain.Entities.Profile", b =>
                {
                    b.HasOne("SensoBackend.Domain.Entities.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SensoBackend.Domain.Entities.Account", "Senior")
                        .WithMany()
                        .HasForeignKey("SeniorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("Senior");
                });
#pragma warning restore 612, 618
        }
    }
}
