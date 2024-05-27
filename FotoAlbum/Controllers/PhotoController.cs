using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FotoAlbum;
using FotoAlbum.Models;
using Microsoft.AspNetCore.Identity;

namespace FotoAlbum.Controllers
{
    public class PhotoController : Controller
    {
        private readonly PhotoContext _context;
        private readonly IServiceProvider _serviceProvider;

        public PhotoController(PhotoContext context, IServiceProvider serviceProvider)
        {
            _context = context;
            _serviceProvider = serviceProvider;
        }



        public async Task<IActionResult> SeedDatabase()
        {
            await Seed(_serviceProvider);
            return RedirectToAction("Index", "Home");
        }

        public static async Task Seed(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                // Seed Roles
                var roles = new[] { "USER", "ADMIN", "SUPERADMIN" };
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                // Seed User
                var user = new IdentityUser
                {
                    UserName = "user@example.com",
                    Email = "user@example.com",
                    EmailConfirmed = true
                };

                if (userManager.Users.All(u => u.Id != user.Id))
                {
                    var userUser = await userManager.FindByEmailAsync(user.Email);
                    if (userUser == null)
                    {
                        await userManager.CreateAsync(user, "Password123!");
                        await userManager.AddToRoleAsync(user, "USER");
                    }
                }

                // Seed Admin
                var admin = new IdentityUser
                {
                    UserName = "admin@example.com",
                    Email = "admin@example.com",
                    EmailConfirmed = true
                };

                if (userManager.Users.All(u => u.Id != admin.Id))
                {
                    var userAdmin = await userManager.FindByEmailAsync(admin.Email);
                    if (userAdmin == null)
                    {
                        await userManager.CreateAsync(admin, "Password123!");
                        await userManager.AddToRoleAsync(admin, "ADMIN");
                    }
                }

                // Seed Super Admin
                var superAdmin = new IdentityUser
                {
                    UserName = "superadmin@example.com",
                    Email = "superadmin@example.com",
                    EmailConfirmed = true
                };

                if (userManager.Users.All(u => u.Id != superAdmin.Id))
                {
                    var superAdminUser = await userManager.FindByEmailAsync(superAdmin.Email);
                    if (superAdminUser == null)
                    {
                        await userManager.CreateAsync(superAdmin, "Password123!");
                        await userManager.AddToRoleAsync(superAdmin, "SUPERADMIN");
                    }
                }
            }
        }

        // GET: Photo
        public async Task<IActionResult> Index()
        {
            return View(await _context.Photos.ToListAsync());
        }

        // GET: Photo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var photo = await _context.Photos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (photo == null)
            {
                return NotFound();
            }

            return View(photo);
        }

        // GET: Photo/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Photo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Img,ImgSrc,IsVisible,UserId")] Photo photo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(photo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(photo);
        }

        // GET: Photo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var photo = await _context.Photos.FindAsync(id);
            if (photo == null)
            {
                return NotFound();
            }
            return View(photo);
        }

        // POST: Photo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Img,ImgSrc,IsVisible,UserId")] Photo photo)
        {
            if (id != photo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(photo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhotoExists(photo.Id))
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
            return View(photo);
        }

        // GET: Photo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var photo = await _context.Photos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (photo == null)
            {
                return NotFound();
            }

            return View(photo);
        }

        // POST: Photo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var photo = await _context.Photos.FindAsync(id);
            if (photo != null)
            {
                _context.Photos.Remove(photo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PhotoExists(int id)
        {
            return _context.Photos.Any(e => e.Id == id);
        }
    }
}
