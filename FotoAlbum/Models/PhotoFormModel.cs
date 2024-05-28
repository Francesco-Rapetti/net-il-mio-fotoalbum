using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;

namespace FotoAlbum.Models
{
    public class PhotoFormModel
    {
        public Photo Photo { get; set; }

        public List<Category>? Categories { get; set; }
        [Required(ErrorMessage = "Select at least one category")]
        public List<int>? SelectedCategoriesIds { get; set; } // Gli ID degli elementi effettivamente selezionati

        [Required(ErrorMessage = "Chose an image file")]
        public IFormFile? ImageFormFile { get; set; }


        public byte[]? SetImageFileFromFormFile()
        {
            if (ImageFormFile == null)
                return null;

            using var stream = new MemoryStream();
            this.ImageFormFile?.CopyTo(stream);
            Photo.Img = stream.ToArray();

            return Photo.Img;
        }
    }
}
