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
using Microsoft.AspNetCore.Mvc.ModelBinding;

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
                var normalUser = users.Find(user => user.UserName == "user@example.com");
                users.Remove(normalUser);
                
                for (int i = 0; i < 100; i++)
                {
                    int randomUser = new Random().Next(0, users.Count);
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
            //if (User.IsInRole("USER") && (userId == _userManager.GetUserId(User) || userId == null))
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            List<Photo> photos = new List<Photo>();
            if (userId == _userManager.GetUserId(User)) photos.AddRange(_context.Photos.Include(x => x.User).Include(x => x.Categories).Where(x => x.UserId == userId).ToList());
            else photos.AddRange(_context.Photos.Include(x => x.User).Include(x => x.Categories).Where(x => x.UserId == userId && x.IsVisible).ToList());
            return View(photos);
        }

        // GET: Photo/Details/5
        [Authorize(Roles = "USER, ADMIN, SUPERADMIN")]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var photos = _context.Photos.Include(x => x.Categories);
            Photo? photo = photos.Where(photo => photo.Id == id).FirstOrDefault();
            if (photo == null)
            {
                return NotFound();
            }

            return View(photo);
        }

        public IActionResult PreviousPhoto(int id, string userId)
        {
            List<Photo> photos = new List<Photo>();
            if (userId == _userManager.GetUserId(User)) photos.AddRange(_context.Photos.Include(x => x.User).Include(x => x.Categories).Where(x => x.UserId == userId).ToList());
            else photos.AddRange(_context.Photos.Include(x => x.User).Include(x => x.Categories).Where(x => x.UserId == userId && x.IsVisible).ToList());
            int index = photos.FindIndex(x => x.Id == id) - 1;
            try
            {
                Photo photo = photos[index];
                return RedirectToAction("Details", new { id = photo.Id });
            }
            catch (Exception)
            {
                return RedirectToAction("Index", new { userId = userId });
            }
        }

        public IActionResult NextPhoto(int id, string userId)
        {
            List<Photo> photos = new List<Photo>();
            if (userId == _userManager.GetUserId(User)) photos.AddRange(_context.Photos.Include(x => x.User).Include(x => x.Categories).Where(x => x.UserId == userId).ToList());
            else photos.AddRange(_context.Photos.Include(x => x.User).Include(x => x.Categories).Where(x => x.UserId == userId && x.IsVisible).ToList());
            int index = photos.FindIndex(x => x.Id == id) + 1;
            try
            {
                Photo photo = photos[index];
                return RedirectToAction("Details", new {id = photo.Id});
            }
            catch (Exception)
            {
                return RedirectToAction("Index", new { userId = userId });
            }
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
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var photo = _context.Photos.Include(x => x.Categories).FirstOrDefault(x => x.Id == id);
            if (photo == null)
            {
                return NotFound();
            }

            if (photo.UserId != _userManager.GetUserId(User))
            {
                return Forbid();
            }
			PhotoFormModel model = new();
			model.Photo = photo;
			model.Categories = [.. _context.Categories.ToList()];
			model.SelectedCategoriesIds = photo.Categories?.Select(x => x.Id).ToList();
			return View(model);
        }

        // POST: Photo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "ADMIN, SUPERADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, PhotoFormModel data)
        {
            if (data.Photo.UserId != _userManager.GetUserId(User))
            {
                return Forbid();
            }


			if (ModelState.Where(x => x.Key != "ImageFormFile").Any(x => x.Value.ValidationState == ModelValidationState.Invalid))
			{
				PhotoFormModel model = new();
				var photo = _context.Photos.Include(x => x.Categories).FirstOrDefault(x => x.Id == id);
				model.Photo = photo ?? data.Photo;
				model.Categories = [.. _context.Categories.ToList()];
				model.SelectedCategoriesIds = photo?.Categories?.Select(x => x.Id).ToList();
				return View(model);
			}

            Photo PhotoToEdit = _context.Photos.Include(x => x.Categories).FirstOrDefault(x => x.Id == id);
            if (ModelState["ImageFormFile"]?.ValidationState == ModelValidationState.Valid)
            {
			    data.SetImageFileFromFormFile();
			    PhotoToEdit.Img = data.Photo.Img;
            }

            PhotoToEdit.Categories.Clear();
			PhotoToEdit.Name = data.Photo.Name;
			PhotoToEdit.Description = data.Photo.Description;
			PhotoToEdit.IsVisible = data.Photo.IsVisible;
			foreach (int categoryId in data.SelectedCategoriesIds ?? new List<int>())
			{
				PhotoToEdit.Categories?.Add(_context.Categories.Find(categoryId));
			}
			_context.SaveChanges();
			return RedirectToAction("Details", new { Id = id });
		}

        // GET: Photo/Delete/5
        [Authorize(Roles = "ADMIN, SUPERADMIN")]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var photo = _context.Photos
                .FirstOrDefault(m => m.Id == id);
            if (photo == null)
            {
                return NotFound();
            }

            if (photo.UserId != _userManager.GetUserId(User))
            {
                return Forbid();
            }
            _context.Remove(photo);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        private bool PhotoExists(int id)
        {
            return _context.Photos.Any(e => e.Id == id);
        }
    }
}
