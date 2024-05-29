using FotoAlbum.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FotoAlbum.Controllers
{
    public class MessageController : Controller
    {
        private readonly PhotoContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MessageController(PhotoContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: MessageController
        public ActionResult Index()
        {
            List<Message> messages = new List<Message>();
            messages.AddRange(_context.Messages.Where(x => x.UserId == _userManager.GetUserId(User)).ToList());
            return View(messages);
        }

        // GET: MessageController/Delete/5
        public ActionResult Delete(int id)
        {
            var message = _context.Messages.Find(id);
            _context.Messages.Remove(message);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: MessageController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
