﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using tl121pet.DAL.Data;

#nullable disable

namespace tl121pet.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
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

                    b.Property<long>("SalaryId")
                        .HasColumnType("bigint");

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

                    b.HasIndex("MeetingTypeId");

                    b.HasIndex("PersonId");

                    b.ToTable("Meeting");
                });

            modelBuilder.Entity("tl121pet.Entities.Models.MeetingGoal", b =>
                {
                    b.Property<Guid>("MeetingGoalId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("MeetingGoalDescription")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("MeetingId")
                        .HasColumnType("uuid");

                    b.HasKey("MeetingGoalId");

                    b.HasIndex("MeetingId");

                    b.ToTable("MeetingGoal");
                });

            modelBuilder.Entity("tl121pet.Entities.Models.MeetingNote", b =>
                {
                    b.Property<Guid>("MeetingNoteId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

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

                    b.ToTable("MeetingType");
                });

            modelBuilder.Entity("tl121pet.Entities.Models.Person", b =>
                {
                    b.Property<long>("PersonId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("PersonId"));

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("GradeId")
                        .HasColumnType("bigint");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SurName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("PersonId");

                    b.HasIndex("GradeId");

                    b.ToTable("People");
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

            modelBuilder.Entity("tl121pet.Entities.Models.Skill", b =>
                {
                    b.Property<long>("SkillId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("SkillId"));

                    b.Property<long>("SkillGroupId")
                        .HasColumnType("bigint");

                    b.Property<int>("SkillTypeId")
                        .HasColumnType("integer");

                    b.Property<string>("SkillsDescription")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("SkillId");

                    b.HasIndex("SkillGroupId");

                    b.HasIndex("SkillTypeId");

                    b.ToTable("Skills");
                });

            modelBuilder.Entity("tl121pet.Entities.Models.SkillGroup", b =>
                {
                    b.Property<long>("SkillGroupId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("SkillGroupId"));

                    b.Property<string>("SkillGroupName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("SkillGroupId");

                    b.ToTable("SkillGroups");
                });

            modelBuilder.Entity("tl121pet.Entities.Models.SkillType", b =>
                {
                    b.Property<int>("SkillTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("SkillTypeId"));

                    b.Property<string>("SkillTypeName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("SkillTypeId");

                    b.ToTable("SkillTypes");
                });

            modelBuilder.Entity("tl121pet.Entities.Models.Meeting", b =>
                {
                    b.HasOne("tl121pet.Entities.Models.MeetingType", "MeetingType")
                        .WithMany()
                        .HasForeignKey("MeetingTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("tl121pet.Entities.Models.Person", "Person")
                        .WithMany()
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MeetingType");

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

            modelBuilder.Entity("tl121pet.Entities.Models.Skill", b =>
                {
                    b.HasOne("tl121pet.Entities.Models.SkillGroup", "SkillGroup")
                        .WithMany()
                        .HasForeignKey("SkillGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("tl121pet.Entities.Models.SkillType", "SkillType")
                        .WithMany()
                        .HasForeignKey("SkillTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SkillGroup");

                    b.Navigation("SkillType");
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
