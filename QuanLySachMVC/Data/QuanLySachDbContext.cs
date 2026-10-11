using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using QuanLySachMVC.Models;

namespace QuanLySachMVC.Data;

public partial class QuanLySachDbContext : DbContext
{
    public QuanLySachDbContext()
    {
    }

    public QuanLySachDbContext(DbContextOptions<QuanLySachDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Sach> Saches { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
        // Chỉ dùng chuỗi mặc định khi chưa được cấu hình từ Program.cs (appsettings.json),
        // tránh ghi đè chuỗi kết nối trong appsettings.json.
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=QuanLySachDb;Trusted_Connection=True;TrustServerCertificate=True");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Sach>(entity =>
        {
            entity.HasKey(e => e.MaSach).HasName("PK__Sach__B235742D82564181");

            entity.ToTable("Sach");

            entity.Property(e => e.GiaBan).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MoTa).HasMaxLength(500);
            entity.Property(e => e.NgayXuatBan).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TacGia).HasMaxLength(100);
            entity.Property(e => e.TheLoai).HasMaxLength(50);
            entity.Property(e => e.TieuDe).HasMaxLength(200);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
