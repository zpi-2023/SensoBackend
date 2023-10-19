﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SensoBackend.Infrastructure.Data;

#nullable disable

namespace SensoBackend.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20231019181221_AddRolesAndProfiles")]
    partial class AddRolesAndProfiles
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Active = true,
                            CreatedAt = new DateTimeOffset(new DateTime(2023, 10, 19, 18, 12, 21, 72, DateTimeKind.Unspecified).AddTicks(6517), new TimeSpan(0, 0, 0, 0, 0)),
                            DisplayName = "admin_senso",
                            Email = "admin@senso.pl",
                            LastLoginAt = new DateTimeOffset(new DateTime(2023, 10, 19, 18, 12, 21, 72, DateTimeKind.Unspecified).AddTicks(6517), new TimeSpan(0, 0, 0, 0, 0)),
                            LastPasswordChangeAt = new DateTimeOffset(new DateTime(2023, 10, 19, 18, 12, 21, 72, DateTimeKind.Unspecified).AddTicks(6517), new TimeSpan(0, 0, 0, 0, 0)),
                            Password = "$2a$11$2U.MLkSys/NuC5JXy.Hsie.XSMlhamOammkHZBuWAqIR7cPbyCSVO",
                            PhoneNumber = "123456789",
                            RoleId = 1,
                            Verified = true
                        },
                        new
                        {
                            Id = 2,
                            Active = true,
                            CreatedAt = new DateTimeOffset(new DateTime(2023, 10, 19, 18, 12, 21, 201, DateTimeKind.Unspecified).AddTicks(6130), new TimeSpan(0, 0, 0, 0, 0)),
                            DisplayName = "senior_senso",
                            Email = "senior@senso.pl",
                            LastLoginAt = new DateTimeOffset(new DateTime(2023, 10, 19, 18, 12, 21, 201, DateTimeKind.Unspecified).AddTicks(6130), new TimeSpan(0, 0, 0, 0, 0)),
                            LastPasswordChangeAt = new DateTimeOffset(new DateTime(2023, 10, 19, 18, 12, 21, 201, DateTimeKind.Unspecified).AddTicks(6130), new TimeSpan(0, 0, 0, 0, 0)),
                            Password = "$2a$11$ElheHSU35MNH438lzLMgge1d5LOiO2ByC6f6Mh74PTQXCeQHFrkOe",
                            PhoneNumber = "123456789",
                            RoleId = 2,
                            Verified = true
                        },
                        new
                        {
                            Id = 3,
                            Active = true,
                            CreatedAt = new DateTimeOffset(new DateTime(2023, 10, 19, 18, 12, 21, 331, DateTimeKind.Unspecified).AddTicks(9750), new TimeSpan(0, 0, 0, 0, 0)),
                            DisplayName = "caretaker_senso",
                            Email = "caretaker@senso.pl",
                            LastLoginAt = new DateTimeOffset(new DateTime(2023, 10, 19, 18, 12, 21, 331, DateTimeKind.Unspecified).AddTicks(9750), new TimeSpan(0, 0, 0, 0, 0)),
                            LastPasswordChangeAt = new DateTimeOffset(new DateTime(2023, 10, 19, 18, 12, 21, 331, DateTimeKind.Unspecified).AddTicks(9750), new TimeSpan(0, 0, 0, 0, 0)),
                            Password = "$2a$11$qBumOFo2yqLQ5Sbap0SmpOvlZcTI7pHBno03B.U4XLlUkeR0iyFaS",
                            PhoneNumber = "123456789",
                            RoleId = 2,
                            Verified = true
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
                        .HasColumnType("text");

                    b.Property<int>("SeniorId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("SeniorId");

                    b.ToTable("Profiles", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            AccountId = 2,
                            SeniorId = 2
                        },
                        new
                        {
                            Id = 2,
                            AccountId = 3,
                            Alias = "Senior",
                            SeniorId = 2
                        });
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
