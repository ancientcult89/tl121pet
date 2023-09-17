﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using tl121pet.DAL.Data;

#nullable disable

namespace tl121pet.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20230907131628_RemoveGoalCompleteDescription")]
    partial class RemoveGoalCompleteDescription
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("tl121pet.Entities.Models.Grade", b =>
                {
                    b.Property<long>("GradeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("GradeId"));

                    b.Property<string>("GradeName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("GradeId");

                    b.ToTable("Grades");
                });

            modelBuilder.Entity("tl121pet.Entities.Models.Meeting", b =>
                {
                    b.Property<Guid>("MeetingId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("FollowUpIsSended")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("MeetingDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("MeetingPlanDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("MeetingTypeId")
                        .HasColumnType("integer");

                    b.Property<long>("PersonId")
                        .HasColumnType("bigint");

                    b.HasKey("MeetingId");

                    b.HasIndex("PersonId");

                    b.ToTable("Meetings");
                });

            modelBuilder.Entity("tl121pet.Entities.Models.MeetingGoal", b =>
                {
                    b.Property<Guid>("MeetingGoalId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("IsCompleted")
                        .HasColumnType("boolean");

                    b.Property<string>("MeetingGoalDescription")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("MeetingId")
                        .HasColumnType("uuid");

                    b.HasKey("MeetingGoalId");

                    b.HasIndex("MeetingId");

                    b.ToTable("MeetingGoals");
                });

            modelBuilder.Entity("tl121pet.Entities.Models.MeetingNote", b =>
                {
                    b.Property<Guid>("MeetingNoteId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("FeedbackRequired")
                        .HasColumnType("boolean");

                    b.Property<Guid>("MeetingId")
                        .HasColumnType("uuid");

                    b.Property<string>("MeetingNoteContent")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("MeetingNoteId");

                    b.HasIndex("MeetingId");

                    b.ToTable("MeetingNotes");
                });

            modelBuilder.Entity("tl121pet.Entities.Models.MeetingType", b =>
                {
                    b.Property<int>("MeetingTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("MeetingTypeId"));

                    b.Property<string>("MeetingTypeName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("MeetingTypeId");

                    b.ToTable("MeetingTypes");
                });

            modelBuilder.Entity("tl121pet.Entities.Models.Person", b =>
                {
                    b.Property<long>("PersonId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("PersonId"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("GradeId")
                        .HasColumnType("bigint");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ShortName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SurName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("PersonId");

                    b.HasIndex("GradeId");

                    b.ToTable("People");
                });

            modelBuilder.Entity("tl121pet.Entities.Models.ProjectMember", b =>
                {
                    b.Property<long>("ProjectMemberId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("ProjectMemberId"));

                    b.Property<long>("PersonId")
                        .HasColumnType("bigint");

                    b.Property<long>("ProjectTeamId")
                        .HasColumnType("bigint");

                    b.HasKey("ProjectMemberId");

                    b.HasIndex("PersonId");

                    b.HasIndex("ProjectTeamId");

                    b.ToTable("ProjectMembers");
                });

            modelBuilder.Entity("tl121pet.Entities.Models.ProjectTeam", b =>
                {
                    b.Property<long>("ProjectTeamId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("ProjectTeamId"));

                    b.Property<string>("ProjectTeamName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("ProjectTeamId");

                    b.ToTable("ProjectTeams");
                });

            modelBuilder.Entity("tl121pet.Entities.Models.Role", b =>
                {
                    b.Property<int>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("RoleId"));

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("RoleId");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("tl121pet.Entities.Models.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<string>("PasswordResetToken")
                        .HasColumnType("text");

                    b.Property<byte[]>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<DateTime?>("ResetTokenExpired")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("RoleId")
                        .HasColumnType("integer");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("VerificationToken")
                        .HasColumnType("text");

                    b.Property<DateTime?>("VerifiedAt")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("tl121pet.Entities.Models.UserProject", b =>
                {
                    b.Property<long>("UserProjectId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("UserProjectId"));

                    b.Property<long>("ProjectTeamId")
                        .HasColumnType("bigint");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("UserProjectId");

                    b.HasIndex("ProjectTeamId");

                    b.HasIndex("UserId");

                    b.ToTable("UserProjects");
                });

            modelBuilder.Entity("tl121pet.Entities.Models.Meeting", b =>
                {
                    b.HasOne("tl121pet.Entities.Models.Person", "Person")
                        .WithMany()
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Person");
                });

            modelBuilder.Entity("tl121pet.Entities.Models.MeetingGoal", b =>
                {
                    b.HasOne("tl121pet.Entities.Models.Meeting", "Meeting")
                        .WithMany("MeetingGoals")
                        .HasForeignKey("MeetingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Meeting");
                });

            modelBuilder.Entity("tl121pet.Entities.Models.MeetingNote", b =>
                {
                    b.HasOne("tl121pet.Entities.Models.Meeting", "Meeting")
                        .WithMany("MeetingNotes")
                        .HasForeignKey("MeetingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Meeting");
                });

            modelBuilder.Entity("tl121pet.Entities.Models.Person", b =>
                {
                    b.HasOne("tl121pet.Entities.Models.Grade", "Grade")
                        .WithMany()
                        .HasForeignKey("GradeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Grade");
                });

            modelBuilder.Entity("tl121pet.Entities.Models.ProjectMember", b =>
                {
                    b.HasOne("tl121pet.Entities.Models.Person", "Person")
                        .WithMany()
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("tl121pet.Entities.Models.ProjectTeam", "ProjectTeam")
                        .WithMany()
                        .HasForeignKey("ProjectTeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Person");

                    b.Navigation("ProjectTeam");
                });

            modelBuilder.Entity("tl121pet.Entities.Models.User", b =>
                {
                    b.HasOne("tl121pet.Entities.Models.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("tl121pet.Entities.Models.UserProject", b =>
                {
                    b.HasOne("tl121pet.Entities.Models.ProjectTeam", "ProjectTeam")
                        .WithMany()
                        .HasForeignKey("ProjectTeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("tl121pet.Entities.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ProjectTeam");

                    b.Navigation("User");
                });

            modelBuilder.Entity("tl121pet.Entities.Models.Meeting", b =>
                {
                    b.Navigation("MeetingGoals");

                    b.Navigation("MeetingNotes");
                });
#pragma warning restore 612, 618
        }
    }
}
