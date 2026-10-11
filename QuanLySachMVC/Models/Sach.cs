using System;
using System.Collections.Generic;

namespace QuanLySachMVC.Models;

public partial class Sach
{
    public int MaSach { get; set; }

    public string TieuDe { get; set; } = null!;

    public string TacGia { get; set; } = null!;

    public string TheLoai { get; set; } = null!;

    public decimal GiaBan { get; set; }

    public DateOnly NgayXuatBan { get; set; }

    public string? MoTa { get; set; }
}
