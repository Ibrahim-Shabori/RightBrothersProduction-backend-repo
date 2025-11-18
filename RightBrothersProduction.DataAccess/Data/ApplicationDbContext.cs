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
        public DbSet<Request> Requests { get; set; }
        public DbSet<DetailedRequest> DetailedRequests { get; set; }
        public DbSet<RequestFile> RequestFiles { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<RegisteredRequest> RegisteredRequests { get; set; }
        public DbSet<RequestLog> RequestLogs { get; set; }
        public DbSet<Category> Categories { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // One-to-one: Request ↔ DetailedRequest
            modelBuilder.Entity<Request>()
                .HasOne(r => r.DetailedRequest)
                .WithOne(dr => dr.Request)
                .HasForeignKey<DetailedRequest>(dr => dr.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RegisteredRequest>()
                .HasKey(r => r.RequestId);

            modelBuilder.Entity<RegisteredRequest>()
                .HasOne(r => r.Request)
                .WithOne()
                .HasForeignKey<RegisteredRequest>(r => r.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-one: Request ↔ RegisteredRequest
            modelBuilder.Entity<Request>()
                .HasOne(r => r.RegisteredRequest)
                .WithOne(rr => rr.Request)
                .HasForeignKey<RegisteredRequest>(rr => rr.RequestId)
                .OnDelete(DeleteBehavior.Restrict);


            // ---- Votes ----
            modelBuilder.Entity<Vote>()
                .HasOne(v => v.Request)
                .WithMany(r => r.Votes)
                .HasForeignKey(v => v.RequestId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---- RequestFiles ----
            modelBuilder.Entity<RequestFile>()
                .HasOne(f => f.Request)
                .WithMany(r => r.Files)
                .HasForeignKey(f => f.RequestId)
                .OnDelete(DeleteBehavior.Restrict);  // ✅ no cascade

            // ---- RequestLogs ----
            modelBuilder.Entity<RequestLog>()
                .HasOne(l => l.Request)
                .WithMany(r => r.Logs)
                .HasForeignKey(l => l.RequestId)
                .OnDelete(DeleteBehavior.Restrict); // ✅ no cascade

            // Enum to string
            modelBuilder.Entity<Request>()
                .Property(r => r.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Request>()
                .Property(r => r.Type)
                .HasConversion<string>();
        }

    }


}

