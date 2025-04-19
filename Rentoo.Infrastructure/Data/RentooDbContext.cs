using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rentoo.Domain.Models;

namespace Rentoo.Infrastructure.Data
{
    public class RentooDbContext : DbContext
    {
        public RentooDbContext(DbContextOptions<RentooDbContext> options): base(options) { }
        public DbSet<Car> Cars { get; set; }
        public DbSet<CarImage> CarImages { get; set; }
        public DbSet<CarDocument> CarDocuments { get; set; }
        public DbSet<RateCode> RateCodes { get; set; }
        public DbSet<RequestReview> RequestReviews { get; set; }
        public DbSet<RateCodeDay> rateCodeDays { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserDocument> UserDocuments { get; set; }
        public DbSet<Request> Requests { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configure CarImage index
            modelBuilder.Entity<CarImage>()
                .HasIndex(p => new { p.ID, p.CarId });

            // Configure Car to CarDocument relationship
            modelBuilder.Entity<Car>()
                .HasOne(c => c.CarDocument)
                .WithOne(cd => cd.Car)
                .HasForeignKey<CarDocument>(cd => cd.CarId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure CarDocument to User relationship
            modelBuilder.Entity<CarDocument>()
                .HasOne(cd => cd.User)
                .WithMany()
                .HasForeignKey(cd => cd.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure User to Car relationship
            modelBuilder.Entity<User>()
                .HasMany(u => u.Cars)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure User to Request relationship
            modelBuilder.Entity<User>()
                .HasMany(u => u.Requests)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Car to Request relationship
            modelBuilder.Entity<Car>()
                .HasMany(c => c.Requests)
                .WithOne(r => r.Car)
                .HasForeignKey(r => r.CarId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure indexes for User
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
            modelBuilder.Entity<User>()
                .HasIndex(u => u.PhoneNumber)
                .IsUnique();

            // Configure indexes for Car
            modelBuilder.Entity<Car>()
                .HasIndex(c => c.Model);
            modelBuilder.Entity<Car>()
                .HasIndex(c => c.IsAvailable);

            // Configure indexes for Request
            modelBuilder.Entity<Request>()
                .HasIndex(r => r.Status);
            modelBuilder.Entity<Request>()
                .HasIndex(r => new { r.UserID, r.CarId });

            // Configure indexes for CarDocument
            modelBuilder.Entity<CarDocument>()
                .HasIndex(cd => cd.LicenseNumber)
                .IsUnique();
            modelBuilder.Entity<CarDocument>()
                .HasIndex(cd => cd.status);

            // Configure indexes for RateCode
            modelBuilder.Entity<RateCode>()
                .HasIndex(rc => rc.ID);

            // Configure indexes for RateCodeDay
            modelBuilder.Entity<RateCodeDay>()
                .HasIndex(rcd => new { rcd.RateCodeId, rcd.Day });

            // Configure indexes for RequestReview
            modelBuilder.Entity<RequestReview>()
                .HasIndex(rr => rr.RequestId)
                .IsUnique();
        }


    }
    
    }

