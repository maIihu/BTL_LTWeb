using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using web1.Models;

namespace web1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/KhachHangs")]
    public class KhachhangsController : Controller
    {
        private readonly ShopGiayContext _context;

        public KhachhangsController(ShopGiayContext context)
        {
            _context = context;
        }

        // GET: Admin/Khachhangs
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Khachhangs.ToListAsync());
        }

        // GET: Admin/Khachhangs/Details/5
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachhang = await _context.Khachhangs
                .FirstOrDefaultAsync(m => m.MaKh == id);
            if (khachhang == null)
            {
                return NotFound();
            }

            return View(khachhang);
        }
        [Route("Edit/{id}")]
        // GET: Admin/Khachhangs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachhang = await _context.Khachhangs.FindAsync(id);
            if (khachhang == null)
            {
                return NotFound();
            }
            return View(khachhang);
        }

        // POST: Admin/Khachhangs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("Edit/{id}")]

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaKh,TaiKhoanKh,MatKhau,HoTen,EmailKh,DiaChiKh,DienThoaiKh,NgaySinh,TrangThai")] Khachhang khachhang)
        {
            if (id != khachhang.MaKh)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(khachhang);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KhachhangExists(khachhang.MaKh))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(khachhang);
        }

        // GET: Admin/Khachhangs/Delete/5
        [Route("Delete/{id}")]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachhang = await _context.Khachhangs
                .FirstOrDefaultAsync(m => m.MaKh == id);
            if (khachhang == null)
            {
                return NotFound();
            }

            return View(khachhang);
        }

        // POST: Admin/Khachhangs/Delete/5
        [HttpPost]
        [Route("Delete/{id}")]

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hasOrders = await _context.Donhangs.AnyAsync(d => d.MaKh == id);

            if (hasOrders)
            {
                // Display a message if the customer has orders
                TempData["Message"] = "Không xóa được sản phẩm này";
                return RedirectToAction(nameof(Index));
            }
            var khachhang = await _context.Khachhangs.FindAsync(id);
            if (khachhang != null)
            {
                _context.Khachhangs.Remove(khachhang);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KhachhangExists(int id)
        {
            return _context.Khachhangs.Any(e => e.MaKh == id);
        }
    }
}
