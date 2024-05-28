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
using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FotoAlbum.Controllers
{
    public class PhotoController : Controller
    {
        private readonly PhotoContext _context;
        private readonly IServiceProvider _serviceProvider;
        private readonly UserManager<IdentityUser> _userManager;

        public PhotoController(PhotoContext context, IServiceProvider serviceProvider, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _serviceProvider = serviceProvider;
            _userManager = userManager;
        }



        public async Task<IActionResult> SeedDatabase()
        {
            await Seed(_serviceProvider);
            return RedirectToAction("Index", "Home");
        }

        public async Task Seed(IServiceProvider serviceProvider)
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

                // Seed Categories
                _context.Categories.RemoveRange(_context.Categories);
                var categories = new[] { "Nature", "City", "People", "Sport", "Animals", "Food", "Flowers" };
                foreach (var category in categories)
                    _context.Categories.Add(new Category { Name = category });
                _context.SaveChanges();

                // Seed Photos
                _context.Photos.RemoveRange(_context.Photos);
                var users = await userManager.Users.ToListAsync();
                
                for (int i = 0; i < 100; i++)
                {
                    int randomUser = new Random().Next(0, users.Count - 1);
                    var photo = new Photo
                    {
                        Name = $"Photo {i}",
                        Description = $"Description {i}",
                        IsVisible = true,
                        UserId = users[randomUser].Id,
                    };
                    photo.Categories = new List<Category>();
                    for (int j = 0; j < new Random().Next(1, 5); j++)
                    {
                        photo.Categories.Add(_context.Categories.ToList()[new Random().Next(0, 7)]);
                    }
                    _context.Photos.Add(photo);
                }
                _context.SaveChanges();
            }
        }

        // GET: Photo
        [Authorize(Roles = "USER, ADMIN, SUPERADMIN")]
        public IActionResult Index(string userId)
        {
            if (userId == null)
            {
                userId = _userManager.GetUserId(User);
            }
            if (userId == null)
            {
                return NotFound();
            }

            List<Photo> photos = _context.Photos.Include(x => x.Categories).Include(x => x.User).Where(x => x.UserId == userId).ToList();
            return View(photos);
        }

        // GET: Photo/Details/5
        [Authorize(Roles = "USER, ADMIN, SUPERADMIN")]
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
        [Authorize(Roles = "ADMIN, SUPERADMIN")]
        public IActionResult Create()
        {
            PhotoFormModel model = new();
            model.Photo = new();
            model.Categories = [.. _context.Categories.ToList()];
            model.SelectedCategoriesIds = new List<int>();
            return View(model);
        }

        // POST: Photo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN, SUPERADMIN")]
        public IActionResult Create(PhotoFormModel data)
        {
            if (!ModelState.IsValid)
            {
                data.Categories = _context.Categories.ToList();
                return View(data);
            }

            data.SetImageFileFromFormFile();
            Photo newPhoto = new(data.Photo.Name, data.Photo.Description, data.Photo.Img, data.Photo.UserId, data.Photo.IsVisible);
            foreach (int categoryId in data.SelectedCategoriesIds ?? new List<int>())
            {
                newPhoto.Categories?.Add(_context.Categories.Find(categoryId));
            }
            _context.Photos.Add(newPhoto);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Photo/Edit/5
        [Authorize(Roles = "ADMIN, SUPERADMIN")]
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

            if (photo.UserId != _userManager.GetUserId(User))
            {
                return Forbid();
            }

            return View(photo);
        }

        // POST: Photo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN, SUPERADMIN")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Img,ImgSrc,IsVisible,UserId")] Photo photo)
        {
            if (id != photo.Id)
            {
                return NotFound();
            }

            if (photo.UserId != _userManager.GetUserId(User))
            {
                return Forbid();
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
        [Authorize(Roles = "ADMIN, SUPERADMIN")]
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

            if (photo.UserId != _userManager.GetUserId(User))
            {
                return Forbid();
            }

            return View(photo);
        }

        // POST: Photo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN, SUPERADMIN")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var photo = await _context.Photos.FindAsync(id);
            if (photo != null)
            {
                _context.Photos.Remove(photo);
            }

            if (photo?.UserId != _userManager.GetUserId(User))
            {
                return Forbid();
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
