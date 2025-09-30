using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RightBrothersProduction.Models;

namespace RightBrothersProduction.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {}

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<NormalRequest> NormalRequests { get; set; }
        public DbSet<DetailedRequest> DetailedRequests { get; set; }
        public DbSet<RequestFile> RequestFiles { get; set; }
        public DbSet<NormalRequestVote> NormalVotes { get; set; }
        public DbSet<DetailedRequestVote> DetailedVotes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<NormalRequestVote>()
                .HasKey(v => new { v.UserId, v.RequestId });

            modelBuilder.Entity<NormalRequestVote>()
                .HasOne(v => v.User)
                .WithMany()
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<NormalRequestVote>()
                .HasOne(v => v.Request)
                .WithMany()
                .HasForeignKey(v => v.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DetailedRequestVote>()
                .HasKey(v => new { v.UserId, v.RequestId });

            modelBuilder.Entity<DetailedRequestVote>()
                .HasOne(v => v.User)
                .WithMany()
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DetailedRequestVote>()
                .HasOne(v => v.Request)
                .WithMany()
                .HasForeignKey(v => v.RequestId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }


}

