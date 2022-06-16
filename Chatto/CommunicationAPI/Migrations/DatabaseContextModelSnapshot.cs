﻿// <auto-generated />
using System;
using Chatto.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CommunicationAPI.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
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

                    b.HasKey("Guid");

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

            modelBuilder.Entity("ChatUser", b =>
                {
                    b.Property<Guid>("ChatsGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UsersGuid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("ChatsGuid", "UsersGuid");

                    b.HasIndex("UsersGuid");

                    b.ToTable("ChatUser");
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

            modelBuilder.Entity("ChatUser", b =>
                {
                    b.HasOne("Chatto.Infrastructure.Chat", null)
                        .WithMany()
                        .HasForeignKey("ChatsGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Chatto.Infrastructure.User", null)
                        .WithMany()
                        .HasForeignKey("UsersGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Chatto.Infrastructure.Chat", b =>
                {
                    b.Navigation("Messages");
                });
#pragma warning restore 612, 618
        }
    }
}
