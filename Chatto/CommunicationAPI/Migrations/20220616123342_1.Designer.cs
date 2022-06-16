﻿// <auto-generated />
using System;
using Chatto.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CommunicationAPI.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20220616123342_1")]
    partial class _1
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Chatto.Infrastructure.Chat", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("UserGuid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Guid");

                    b.HasIndex("UserGuid");

                    b.ToTable("Chats");
                });

            modelBuilder.Entity("Chatto.Infrastructure.Message", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AuthorGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ChatGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Guid");

                    b.HasIndex("AuthorGuid");

                    b.HasIndex("ChatGuid");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("Chatto.Infrastructure.User", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("AccountId")
                        .HasColumnType("int");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Guid");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Chatto.Infrastructure.Chat", b =>
                {
                    b.HasOne("Chatto.Infrastructure.User", null)
                        .WithMany("Chats")
                        .HasForeignKey("UserGuid");
                });

            modelBuilder.Entity("Chatto.Infrastructure.Message", b =>
                {
                    b.HasOne("Chatto.Infrastructure.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Chatto.Infrastructure.Chat", "Chat")
                        .WithMany("Messages")
                        .HasForeignKey("ChatGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Chat");
                });

            modelBuilder.Entity("Chatto.Infrastructure.Chat", b =>
                {
                    b.Navigation("Messages");
                });

            modelBuilder.Entity("Chatto.Infrastructure.User", b =>
                {
                    b.Navigation("Chats");
                });
#pragma warning restore 612, 618
        }
    }
}