using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FotoAlbum.Models
{
    public class Photo
    {
        [Key] public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(500)]
        [Required]
        public string? Description { get; set; }
        public byte[]? Img { get; set; }
        public string ImgSrc => Img != null ? $"data:image/png;base64,{Convert.ToBase64String(Img)}" : $"https://picsum.photos/id/{Random.Shared.Next(0, 1000)}/600/400";
        [Required]
        public bool IsVisible { get; set; }
        public List<Category>? Categories { get; set; }
        [Required]
        public string? UserId { get; set; }
        public IdentityUser? User { get; set; }

        public Photo()
        {

        }

        public Photo(string name, string? description, byte[]? img, string userId, bool isVisible = true)
        {
            this.Name = name;
            this.Description = description;
            this.Img = img;
            this.UserId = userId;
            this.IsVisible = isVisible;
            Categories = new List<Category>();
        }
    }
}
