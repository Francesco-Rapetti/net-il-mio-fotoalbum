using System.ComponentModel.DataAnnotations;

namespace FotoAlbum.Models
{
    public class Message
    {
        [Key] public int Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        public string UserId { get; set; }

        public Message()
        {

        }

        public Message(string email, string text, string userId)
        {
            Email = email;
            Text = text;
            UserId = userId;
        }
    }
}
