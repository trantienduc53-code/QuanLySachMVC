using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLySachMVC.Data;
using QuanLySachMVC.Models;

namespace QuanLySachMVC.Controllers
{
    public class SachesController : Controller
    {
        private readonly QuanLySachDbContext _context;

        public SachesController(QuanLySachDbContext context)
        {
            _context = context;
        }

        // 1. READ: Hiển thị danh sách + Tìm kiếm theo Tiêu đề / Tác giả
        public async Task<IActionResult> Index(string searchString)
        {
            var dsSach = from s in _context.Saches select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                dsSach = dsSach.Where(s => s.TieuDe.Contains(searchString) || s.TacGia.Contains(searchString));
            }

            ViewData["CurrentFilter"] = searchString;
            return View(await dsSach.ToListAsync());
        }

        // 2. DETAILS: Xem chi tiết 1 cuốn sách
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var sach = await _context.Saches.FirstOrDefaultAsync(m => m.MaSach == id);
            if (sach == null) return NotFound();

            return View(sach);
        }

        // 3. CREATE (GET): Mở trang form nhập sách mới
        public IActionResult Create()
        {
            return View();
        }

        // 3. CREATE (POST): Lưu sách mới vào CSDL SQL Server
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TieuDe,TacGia,TheLoai,GiaBan,NgayXuatBan,MoTa")] Sach sach)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sach);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm sách mới thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(sach);
        }

        // 4. EDIT (GET): Mở form chỉnh sửa sách
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var sach = await _context.Saches.FindAsync(id);
            if (sach == null) return NotFound();

            return View(sach);
        }

        // 4. EDIT (POST): Cập nhật thông tin sách
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaSach,TieuDe,TacGia,TheLoai,GiaBan,NgayXuatBan,MoTa")] Sach sach)
        {
            if (id != sach.MaSach) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sach);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật sách thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SachExists(sach.MaSach)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(sach);
        }

        // 5. DELETE (GET): Trang hiển thị xác nhận xóa
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var sach = await _context.Saches.FirstOrDefaultAsync(m => m.MaSach == id);
            if (sach == null) return NotFound();

            return View(sach);
        }

        // 5. DELETE (POST): Xác nhận thực hiện xóa khỏi Database
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sach = await _context.Saches.FindAsync(id);
            if (sach != null)
            {
                _context.Saches.Remove(sach);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã xóa sách thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool SachExists(int id)
        {
            return _context.Saches.Any(e => e.MaSach == id);
        }
    }
}