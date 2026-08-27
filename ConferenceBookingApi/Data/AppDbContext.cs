using Microsoft.EntityFrameworkCore;
using ConferenceBookingApi.Models;

namespace ConferenceBookingApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Room> Rooms => Set<Room>();
        public DbSet<Equipment> Equipment => Set<Equipment>();
        public DbSet<Booking> Bookings => Set<Booking>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Equipment>()
                .HasOne(e => e.Room)
                .WithMany(r => r.Equipment)
                .HasForeignKey(e => e.RoomId);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Room)
                .WithMany(r => r.Bookings)
                .HasForeignKey(b => b.RoomId);

            modelBuilder.Entity<Booking>()
                .HasMany(b => b.SelectedEquipment)
                .WithMany();
        }
    }
}