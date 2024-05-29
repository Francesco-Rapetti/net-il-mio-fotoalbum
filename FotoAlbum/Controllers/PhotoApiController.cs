using FotoAlbum.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FotoAlbum.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PhotoApiController : ControllerBase
    {
        private readonly PhotoContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PhotoApiController(PhotoContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/<ValuesController>
        [HttpGet]
        public IActionResult Get()
        {
            var photos = _context.Photos.Include(x => x.User).ToList();
            return Ok(photos);
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public IActionResult Get(string title)
        {
            return Ok(_context.Photos.Where(x => x.Name.Contains(title)).ToList());
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "SUPERADMIN")]
        public IActionResult HidePhoto(int id)
        {
            Photo photoToUpdate = _context.Photos.Find(id);

            if (photoToUpdate != null)
            {
                photoToUpdate.IsVisible = false;
                _context.SaveChanges();
                return Ok(true);
            }

            return Ok(false);

        }
    }
}
