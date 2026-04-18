using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using SwiftDrop.Models;

namespace SwiftDrop.Data;

public partial class SwiftDropDbContext : DbContext
{
    public SwiftDropDbContext()
    {
    }

    public SwiftDropDbContext(DbContextOptions<SwiftDropDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Menuitem> Menuitems { get; set; }

    public virtual DbSet<Openinghour> Openinghours { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Orderitem> Orderitems { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Restaurant> Restaurants { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Suborder> Suborders { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;database=SwiftDropDB;user=root", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.4.32-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("addresses");

            entity.HasIndex(e => e.UserId, "UserId");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Latitude).HasPrecision(10, 8);
            entity.Property(e => e.Longitude).HasPrecision(11, 8);
            entity.Property(e => e.Street).HasMaxLength(255);
            entity.Property(e => e.UserId).HasColumnType("int(11)");
            entity.Property(e => e.ZipCode).HasMaxLength(10);

            entity.HasOne(d => d.User).WithMany(p => p.Addresses)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("addresses_ibfk_1");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("categories");

            entity.HasIndex(e => e.RestaurantId, "RestaurantId");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.DisplayOrder)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.RestaurantId).HasColumnType("int(11)");

            entity.HasOne(d => d.Restaurant).WithMany(p => p.Categories)
                .HasForeignKey(d => d.RestaurantId)
                .HasConstraintName("categories_ibfk_1");
        });

        modelBuilder.Entity<Menuitem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("menuitems");

            entity.HasIndex(e => e.CategoryId, "CategoryId");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Allergens).HasMaxLength(255);
            entity.Property(e => e.CategoryId).HasColumnType("int(11)");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.IsAvailable).HasDefaultValueSql("'1'");
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.Price).HasPrecision(10, 2);
            entity.Property(e => e.WeightOrVolume).HasMaxLength(50);

            entity.HasOne(d => d.Category).WithMany(p => p.Menuitems)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("menuitems_ibfk_1");
        });

        modelBuilder.Entity<Openinghour>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("openinghours");

            entity.HasIndex(e => e.RestaurantId, "RestaurantId");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.ClosingTime).HasColumnType("time");
            entity.Property(e => e.DayOfWeek).HasColumnType("enum('Monday','Tuesday','Wednesday','Thursday','Friday','Saturday','Sunday')");
            entity.Property(e => e.OpenTime).HasColumnType("time");
            entity.Property(e => e.RestaurantId).HasColumnType("int(11)");

            entity.HasOne(d => d.Restaurant).WithMany(p => p.Openinghours)
                .HasForeignKey(d => d.RestaurantId)
                .HasConstraintName("openinghours_ibfk_1");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("orders");

            entity.HasIndex(e => e.AddressId, "AddressId");

            entity.HasIndex(e => e.UserId, "UserId");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.AddressId).HasColumnType("int(11)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.DeliveredAt).HasColumnType("datetime");
            entity.Property(e => e.DeliveryFee).HasPrecision(10, 2);
            entity.Property(e => e.ItemPrice).HasPrecision(10, 2);
            entity.Property(e => e.Status).HasColumnType("enum('Pending','Paid','CourierAssigned','PickupsInProgress','Delivering','Delivered','Canceled')");
            entity.Property(e => e.TotalPrice).HasPrecision(10, 2);
            entity.Property(e => e.UserId).HasColumnType("int(11)");

            entity.HasOne(d => d.Address).WithMany(p => p.Orders)
                .HasForeignKey(d => d.AddressId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("orders_ibfk_2");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("orders_ibfk_1");
        });

        modelBuilder.Entity<Orderitem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("orderitems");

            entity.HasIndex(e => e.MenuItemId, "MenuItemId");

            entity.HasIndex(e => e.SubOrderId, "SubOrderId");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.ItemNotes).HasColumnType("text");
            entity.Property(e => e.MenuItemId).HasColumnType("int(11)");
            entity.Property(e => e.Quantity).HasColumnType("int(11)");
            entity.Property(e => e.SubOrderId).HasColumnType("int(11)");
            entity.Property(e => e.UnitPrice).HasPrecision(10, 2);

            entity.HasOne(d => d.MenuItem).WithMany(p => p.Orderitems)
                .HasForeignKey(d => d.MenuItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("orderitems_ibfk_2");

            entity.HasOne(d => d.SubOrder).WithMany(p => p.Orderitems)
                .HasForeignKey(d => d.SubOrderId)
                .HasConstraintName("orderitems_ibfk_1");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("payments");

            entity.HasIndex(e => e.OrderId, "OrderId");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Amount).HasPrecision(10, 2);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.OrderId).HasColumnType("int(11)");
            entity.Property(e => e.PaymentMethod).HasColumnType("enum('CardOnline','ApplePay','GooglePay','CashOnDelivery')");
            entity.Property(e => e.PaymentStatus).HasColumnType("enum('Unpaid','Paid','Refunded')");

            entity.HasOne(d => d.Order).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("payments_ibfk_1");
        });

        modelBuilder.Entity<Restaurant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("restaurants");

            entity.HasIndex(e => e.AddressId, "AddressId");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.AddressId).HasColumnType("int(11)");
            entity.Property(e => e.AverageRating)
                .HasPrecision(3, 2)
                .HasDefaultValueSql("'0.00'");
            entity.Property(e => e.ContactEmail).HasMaxLength(255);
            entity.Property(e => e.ContactPhone).HasMaxLength(20);
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.EstimatedPrepTimeMinutes).HasColumnType("int(11)");
            entity.Property(e => e.IsAcceptingOrders).HasDefaultValueSql("'1'");
            entity.Property(e => e.IsActive).HasDefaultValueSql("'1'");
            entity.Property(e => e.LogoUrl).HasMaxLength(500);
            entity.Property(e => e.MinimumOrderAmount).HasPrecision(10, 2);
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.ReviewCount)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)");

            entity.HasOne(d => d.Address).WithMany(p => p.Restaurants)
                .HasForeignKey(d => d.AddressId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("restaurants_ibfk_1");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("reviews");

            entity.HasIndex(e => e.RestaurantId, "RestaurantId");

            entity.HasIndex(e => e.UserId, "UserId");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Comment).HasColumnType("text");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Rating).HasColumnType("int(11)");
            entity.Property(e => e.RestaurantId).HasColumnType("int(11)");
            entity.Property(e => e.UserId).HasColumnType("int(11)");

            entity.HasOne(d => d.Restaurant).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.RestaurantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("reviews_ibfk_2");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("reviews_ibfk_1");
        });

        modelBuilder.Entity<Suborder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("suborders");

            entity.HasIndex(e => e.OrderId, "OrderId");

            entity.HasIndex(e => e.RestaurantId, "RestaurantId");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.EstimatedReadyTime).HasColumnType("datetime");
            entity.Property(e => e.OrderId).HasColumnType("int(11)");
            entity.Property(e => e.RestaurantId).HasColumnType("int(11)");
            entity.Property(e => e.Status).HasColumnType("enum('Pending','Preparing','ReadyForPickUp','PickedUp')");

            entity.HasOne(d => d.Order).WithMany(p => p.Suborders)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("suborders_ibfk_1");

            entity.HasOne(d => d.Restaurant).WithMany(p => p.Suborders)
                .HasForeignKey(d => d.RestaurantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("suborders_ibfk_2");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "Email").IsUnique();

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValueSql("'1'");
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.RegisteredAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.Role).HasColumnType("enum('Customer','Courier','Admin','RestaurantManager')");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
