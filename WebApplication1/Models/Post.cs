using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Post
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Gde title")]
        [MaxLength(50)]
        //[DataType(DataType.Text)]
        public string Title { get; set; }
        [Required(ErrorMessage = "Gde aloooooo")]
        public string Content { get; set; }
        public DateTime Date { get; set; }
        [Required(ErrorMessage = "Gde ImageUrl")]
        [Display(Name ="Photo url : ")]
        [DataType(DataType.Upload)]
        public string ImageUrl { get; set; }


        public int CategoryId { get; set; }
        public Category Category { get; set; }


        public IEnumerable<PostTag> PostTags { get; set; }
    }
}
