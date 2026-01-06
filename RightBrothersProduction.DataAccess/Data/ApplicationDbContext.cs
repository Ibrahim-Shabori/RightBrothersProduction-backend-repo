using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RightBrothersProduction.Models;

namespace RightBrothersProduction.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<
    ApplicationUser,
    IdentityRole,
    string,
    IdentityUserClaim<string>,
    ApplicationUserRole,
    IdentityUserLogin<string>,
    IdentityRoleClaim<string>,
    IdentityUserToken<string>>
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
            modelBuilder.Entity<ApplicationUserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId }); // Primary Key

                // Map the 'Role' property to the EXISTING 'RoleId' column
                userRole.HasOne(ur => ur.Role)
                        .WithMany() // Roles usually don't have a list of UserRoles, keep this empty
                        .HasForeignKey(ur => ur.RoleId) // <--- VITAL: Use existing column
                        .IsRequired();

                // Map the 'User' property to the EXISTING 'UserId' column
                userRole.HasOne(ur => ur.User)
                        .WithMany(u => u.UserRoles) // Link back to ApplicationUser.UserRoles
                        .HasForeignKey(ur => ur.UserId) // <--- VITAL: Use existing column
                        .IsRequired();

                // Map to the existing table
                userRole.ToTable("AspNetUserRoles");
            });


            modelBuilder.Entity<ApplicationUser>(b =>
            {
                // Connect the User to the UserRole
                b.HasMany(e => e.UserRoles)
                 .WithOne(e => e.User)
                 .HasForeignKey(ur => ur.UserId)
                 .IsRequired();
            });


            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.IsBanned)
                .HasDefaultValue(false);

            modelBuilder.Entity<ApplicationUser>()
            .HasMany(u => u.RequestLogs)
            .WithOne(l => l.CreatedBy)
            .HasForeignKey(l => l.CreatedById) // Uses the new column
            .OnDelete(DeleteBehavior.Restrict);

            // One-to-one: Request ↔ DetailedRequest
            modelBuilder.Entity<Request>()
                .HasOne(r => r.DetailedRequest)
                .WithOne(dr => dr.Request)
                .HasForeignKey<DetailedRequest>(dr => dr.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Request>()
                .Property(u => u.IsRegistered)
                .HasDefaultValue(false);

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

            modelBuilder.Entity<Vote>()
            .HasIndex(v => new { v.UserId, v.RequestId }) // Composite Index
            .IsUnique();

            modelBuilder.Entity<Vote>()
            .HasIndex(v => new { v.RequestId, v.VotedAt });

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


            modelBuilder.Entity<RequestLog>()
                .Property(l => l.IsPublic)
                .HasDefaultValue(true);

            // Enum to string
            modelBuilder.Entity<Request>()
                .Property(r => r.Status)
                .IsRequired()
                .HasConversion<string>();

            modelBuilder.Entity<Request>()
                .Property(r => r.Type)
                .HasConversion<string>();

            modelBuilder.Entity<RequestLog>()
                .Property(l => l.NewStatus)
                .HasConversion<string>();

            modelBuilder.Entity<Category>()
                .Property(c => c.IsActive)
                .HasDefaultValue(true);
        }

    }


}

