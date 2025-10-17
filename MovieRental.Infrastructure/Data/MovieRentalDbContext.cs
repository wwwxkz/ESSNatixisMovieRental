using Microsoft.EntityFrameworkCore;
using MovieRental.Domain.Entities;

namespace MovieRental.Infrastructure.Data
{
    public class MovieRentalDbContext : DbContext
    {
        public DbSet<Movie> Movies { get; set; } = null!;
        public DbSet<Rental> Rentals { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;

        private readonly string? _dbPath;

        public MovieRentalDbContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            _dbPath = Path.Combine(path, "movierental.db");
        }

        public MovieRentalDbContext(DbContextOptions<MovieRentalDbContext> options) 
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                options.UseSqlite($"Data Source={_dbPath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(255);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.HasIndex(e => e.Email).IsUnique();
            });

            modelBuilder.Entity<Movie>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.RentalPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.SalePrice).HasColumnType("decimal(18,2)");
                entity.HasIndex(e => e.Title);
            });

            modelBuilder.Entity<Rental>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.HasOne(r => r.Movie)
                    .WithMany()
                    .HasForeignKey(r => r.MovieId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(r => r.Customer)
                    .WithMany(c => c.Rentals)
                    .HasForeignKey(r => r.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.Property(r => r.PaymentMethod).IsRequired().HasMaxLength(50);
                entity.Property(r => r.RentalDate).IsRequired();
            });
        }
    }
}
