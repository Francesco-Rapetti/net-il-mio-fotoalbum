namespace FotoAlbum.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Photo> Photos { get; set; }
    }
}
