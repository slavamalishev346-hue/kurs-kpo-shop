using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Shop.Domain.Entities;

namespace Shop.DAL;

public partial class MagazContext : DbContext
{
    public MagazContext()
    {
    }

    public MagazContext(DbContextOptions<MagazContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Loyaltycard> Loyaltycards { get; set; }

    public virtual DbSet<Loyaltylevel> Loyaltylevels { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<Returnitem> Returnitems { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Sale> Sales { get; set; }

    public virtual DbSet<Saleitem> Saleitems { get; set; }

    public virtual DbSet<Supply> Supplies { get; set; }

    public virtual DbSet<Supplyitem> Supplyitems { get; set; }

    public virtual DbSet<User> Users { get; set; }

    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //        => optionsBuilder.UseNpgsql("Host=localhost;Database=Magaz;Username=postgres;Password=3662");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Loyaltycard>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("loyaltycard_pkey");

            entity.ToTable("loyaltycard");

            entity.HasIndex(e => e.CardNumber, "loyaltycard_cardNumber_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CardNumber)
                .HasMaxLength(50)
                .HasColumnName("cardNumber");
            entity.Property(e => e.Customer)
                .HasMaxLength(255)
                .HasColumnName("customer");
            entity.Property(e => e.Discount)
                .HasDefaultValue(0)
                .HasColumnName("discount");
            entity.Property(e => e.LevelId).HasColumnName("levelId");
            entity.Property(e => e.Points)
                .HasDefaultValue(0)
                .HasColumnName("points");

            entity.HasOne(d => d.Level).WithMany(p => p.Loyaltycards)
                .HasForeignKey(d => d.LevelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("loyaltycard_levelId_fkey");
        });

        modelBuilder.Entity<Loyaltylevel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("loyaltylevel_pkey");

            entity.ToTable("loyaltylevel");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Percent).HasColumnName("percent");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("product_pkey");

            entity.ToTable("product");

            entity.HasIndex(e => e.Article, "product_article_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Article)
                .HasMaxLength(100)
                .HasColumnName("article");
            entity.Property(e => e.Cost).HasColumnName("cost");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Price).HasColumnName("price");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("report_pkey");

            entity.ToTable("report");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AdminId).HasColumnName("adminId");
            entity.Property(e => e.Data).HasColumnName("data");
            entity.Property(e => e.EndDate).HasColumnName("endDate");
            entity.Property(e => e.StartDate).HasColumnName("startDate");
            entity.Property(e => e.Type)
                .HasMaxLength(100)
                .HasColumnName("type");

            entity.HasOne(d => d.Admin).WithMany(p => p.Reports)
                .HasForeignKey(d => d.AdminId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("report_adminId_fkey");
        });

        modelBuilder.Entity<Returnitem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("returnitem_pkey");

            entity.ToTable("returnitem");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CashierId).HasColumnName("cashierId");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.SaleItemId).HasColumnName("saleItemId");

            entity.HasOne(d => d.Cashier).WithMany(p => p.Returnitems)
                .HasForeignKey(d => d.CashierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("returnitem_cashierId_fkey");

            entity.HasOne(d => d.SaleItem).WithMany(p => p.Returnitems)
                .HasForeignKey(d => d.SaleItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("returnitem_saleItemId_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("role_pkey");

            entity.ToTable("role");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("sale_pkey");

            entity.ToTable("sale");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CashierId).HasColumnName("cashierId");
            entity.Property(e => e.Date)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date");
            entity.Property(e => e.DiscountAmount)
                .HasDefaultValueSql("0")
                .HasColumnName("discountAmount");
            entity.Property(e => e.FinalAmount).HasColumnName("finalAmount");
            entity.Property(e => e.LoyaltyCardId).HasColumnName("loyaltyCardId");
            entity.Property(e => e.TotalAmount).HasColumnName("totalAmount");

            entity.HasOne(d => d.Cashier).WithMany(p => p.Sales)
                .HasForeignKey(d => d.CashierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sale_cashierId_fkey");

            entity.HasOne(d => d.LoyaltyCard).WithMany(p => p.Sales)
                .HasForeignKey(d => d.LoyaltyCardId)
                .HasConstraintName("sale_loyaltyCardId_fkey");
        });

        modelBuilder.Entity<Saleitem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("saleitem_pkey");

            entity.ToTable("saleitem");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.PriceAtSale).HasColumnName("priceAtSale");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.SaleId).HasColumnName("saleId");
            entity.Property(e => e.SupplyItemId).HasColumnName("supplyItemId");

            entity.HasOne(d => d.Sale).WithMany(p => p.Saleitems)
                .HasForeignKey(d => d.SaleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("saleitem_saleId_fkey");

            entity.HasOne(d => d.SupplyItem).WithMany(p => p.Saleitems)
                .HasForeignKey(d => d.SupplyItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("saleitem_supplyItemId_fkey");
        });

        modelBuilder.Entity<Supply>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("supply_pkey");

            entity.ToTable("supply");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Count).HasColumnName("count");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.SupplierName)
                .HasMaxLength(255)
                .HasColumnName("supplierName");
        });

        modelBuilder.Entity<Supplyitem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("supplyitem_pkey");

            entity.ToTable("supplyitem");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cost).HasColumnName("cost");
            entity.Property(e => e.ExpiryDate).HasColumnName("expiryDate");
            entity.Property(e => e.ProductId).HasColumnName("productId");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.QuantityRemain).HasColumnName("quantityRemain");
            entity.Property(e => e.SupplyId).HasColumnName("supplyId");

            entity.HasOne(d => d.Product).WithMany(p => p.Supplyitems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("supplyitem_productId_fkey");

            entity.HasOne(d => d.Supply).WithMany(p => p.Supplyitems)
                .HasForeignKey(d => d.SupplyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("supplyitem_supplyId_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("User_pkey");

            entity.ToTable("User");

            entity.HasIndex(e => e.Login, "User_login_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CashierNumber).HasColumnName("cashierNumber");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("fullName");
            entity.Property(e => e.Login)
                .HasMaxLength(100)
                .HasColumnName("login");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.RoleId).HasColumnName("roleId");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("User_roleId_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
