using Real_chat.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Real_chat.Data
{
    public class ChatContext : DbContext
    {

        public ChatContext(DbContextOptions<ChatContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserConnection> Connections { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Room> Rooms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChatMessage>()
                .HasKey(cm => cm.Id);

            modelBuilder.Entity<ChatMessage>()
                .Property(cm => cm.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<UserConnection>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<UserConnection>()
                .HasIndex(c => new { c.UserId, c.ChatRoom })
                .IsUnique();
        }
    }
}
