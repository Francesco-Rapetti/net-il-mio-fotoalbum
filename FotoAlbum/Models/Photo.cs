using System.ComponentModel.DataAnnotations;

namespace FotoAlbum.Models
{
    public class Photo
    {
        [Key] public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] Img { get; set; }
        public string ImgSrc { get; set; }
        public bool IsVisible { get; set; }
        public List<Category> categories { get; set; }
        public int UserId { get; set; }
    }
}
