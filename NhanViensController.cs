using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using web1.Models;

namespace web1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/NhanViens")]
    public class NhanViensController : Controller
    {
        private readonly ShopGiayContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public NhanViensController(ShopGiayContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _webHostEnvironment = hostEnvironment;

        }

        // GET: Admin/NhanViens
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.NhanViens.ToListAsync());
        }

        // GET: Admin/NhanViens/Details/5
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanVien = await _context.NhanViens
                .FirstOrDefaultAsync(m => m.CusID == id);
            if (nhanVien == null)
            {
                return NotFound();
            }

            return View(nhanVien);
        }

        // GET: Admin/NhanViens/Create
        [Route("Create")]
        public IActionResult Create()
        {
            return View( new NhanVien());
        }

        // POST: Admin/NhanViens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("Create")]

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CusID,HoTen,TaiKhoan,MatKhau,Email,GioiTinh,Img,")] NhanVien nhanVien)
        {
            string uniqueFileName = null;
            if (nhanVien.Img != null)
            {
                string ImageUpLoadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img");

                uniqueFileName = Guid.NewGuid().ToString() + "_" + nhanVien.Img.FileName;

                string filepath = Path.Combine(ImageUpLoadFolder, uniqueFileName);

                using (var fileStream = new FileStream(filepath, FileMode.Create))
                {
                    nhanVien.Img.CopyTo(fileStream);
                }
                nhanVien.Imgpath = "~/wwwroot/img";
                nhanVien.EmpFileName = uniqueFileName;

                if (ModelState.IsValid)
                {
                    _context.NhanViens.Add(nhanVien);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(nhanVien);
        }
    

        // GET: Admin/NhanViens/Edit/5
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanVien = await _context.NhanViens.FindAsync(id);
            if (nhanVien == null)
            {
                return NotFound();
            }
            return View(nhanVien);
        }

        // POST: Admin/NhanViens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("Edit/{id}")]

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CusID,HoTen,TaiKhoan,MatKhau,Email,GioiTinh,Img")] NhanVien nhanVien)
        {
            if (id != nhanVien.CusID)
            {
                return NotFound();
            }
            string uniqueFileName = null;
            if (nhanVien.Img != null)
            {
                string ImageUpLoadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img");

                uniqueFileName = Guid.NewGuid().ToString() + "_" + nhanVien.Img.FileName;

                string filepath = Path.Combine(ImageUpLoadFolder, uniqueFileName);

                using (var fileStream = new FileStream(filepath, FileMode.Create))
                {
                    nhanVien.Img.CopyTo(fileStream);
                }
                nhanVien.Imgpath = "~/wwwroot/img";
                nhanVien.EmpFileName = uniqueFileName;

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(nhanVien);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!NhanVienExists(nhanVien.CusID))
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
            }
            return View(nhanVien);
        }

        // GET: Admin/NhanViens/Delete/5
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanVien = await _context.NhanViens
                .FirstOrDefaultAsync(m => m.CusID == id);
            if (nhanVien == null)
            {
                return NotFound();
            }

            return View(nhanVien);
        }

        // POST: Admin/NhanViens/Delete/5
        [HttpPost]
        [Route("Delete/{id}")]

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nhanVien = await _context.NhanViens.FindAsync(id);
            if (nhanVien != null)
            {
                _context.NhanViens.Remove(nhanVien);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NhanVienExists(int id)
        {
            return _context.NhanViens.Any(e => e.CusID == id);
        }
    }
}
