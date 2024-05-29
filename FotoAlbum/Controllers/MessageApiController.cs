using FotoAlbum.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FotoAlbum.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MessageApiController : ControllerBase
    {
        private readonly PhotoContext _context;

        public MessageApiController(PhotoContext context)
        {
            _context = context;
        }

        // GET: api/<MessageApiController>
        [HttpGet]
        public IActionResult GetAllMessages()
        {
            return Ok(_context.Messages.ToList());
        }

        // GET api/<MessageApiController>/5
        [HttpGet("{id}")]
        public IActionResult GetMessage(int id)
        {
            return Ok(_context.Messages.Find(id));
        }

        // POST api/<MessageApiController>
        [HttpPost]
        public IActionResult InsertMessage([FromBody] Message message)
        {
            _context.Messages.Add(new Message
            {
                Email = message.Email,
                Text = message.Text,
                UserId = message.UserId
            });
            _context.SaveChanges();

            return Ok(true);
        }

        // DELETE api/<MessageApiController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _context.Messages.Remove(_context.Messages.Find(id));
            _context.SaveChanges();
        }
    }
}
