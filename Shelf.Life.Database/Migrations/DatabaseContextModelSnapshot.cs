﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Shelf.Life.Database.Contexts;

#nullable disable

namespace Shelf.Life.Database.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.3");

            modelBuilder.Entity("Shelf.Life.Database.Models.FoodDto", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastUsed")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Foods");
                });

            modelBuilder.Entity("Shelf.Life.Database.Models.StorageItemDto", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("ExpiryDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("FoodId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("FoodId");

                    b.ToTable("StorageItems");
                });

            modelBuilder.Entity("Shelf.Life.Database.Models.StorageItemDto", b =>
                {
                    b.HasOne("Shelf.Life.Database.Models.FoodDto", "Food")
                        .WithMany()
                        .HasForeignKey("FoodId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Food");
                });
#pragma warning restore 612, 618
        }
    }
}
