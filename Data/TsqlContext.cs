using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Tsql3s2a.Models;

namespace Tsql3s2a.Data;

public partial class TsqlContext : DbContext
{
    public TsqlContext()
    {
    }

    public TsqlContext(DbContextOptions<TsqlContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<CustOrder> CustOrders { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<EmpOrder> EmpOrders { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Num> Nums { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<OrderTotalsByYear> OrderTotalsByYears { get; set; }

    public virtual DbSet<OrderValue> OrderValues { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Score> Scores { get; set; }

    public virtual DbSet<Shipper> Shippers { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<Test> Tests { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AS");

        modelBuilder.Entity<CustOrder>(entity =>
        {
            entity.ToView("CustOrders", "Sales");
        });

        modelBuilder.Entity<EmpOrder>(entity =>
        {
            entity.ToView("EmpOrders", "Sales");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasOne(d => d.Mgr).WithMany(p => p.InverseMgr).HasConstraintName("FK_Employees_Employees");
        });

        modelBuilder.Entity<Num>(entity =>
        {
            entity.Property(e => e.N).ValueGeneratedNever();
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasOne(d => d.Cust).WithMany(p => p.Orders).HasConstraintName("FK_Orders_Customers");

            entity.HasOne(d => d.Emp).WithMany(p => p.Orders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Employees");

            entity.HasOne(d => d.Shipper).WithMany(p => p.Orders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Shippers");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.Property(e => e.Qty).HasDefaultValue((short)1);

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetails_Orders");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetails_Products");
        });

        modelBuilder.Entity<OrderTotalsByYear>(entity =>
        {
            entity.ToView("OrderTotalsByYear", "Sales");
        });

        modelBuilder.Entity<OrderValue>(entity =>
        {
            entity.ToView("OrderValues", "Sales");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Categories");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Suppliers");
        });

        modelBuilder.Entity<Score>(entity =>
        {
            entity.HasOne(d => d.Test).WithMany(p => p.Scores)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Scores_Tests");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
