using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Rentoo.Domain.Entities;

namespace Rentoo.Infrastructure.Data
{
    public class RentooDbContext : IdentityDbContext<User>
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
       

    }
    
}

