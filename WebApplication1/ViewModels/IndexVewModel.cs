using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<Post> Posts { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Tag> Tags { get; set; }
        public IEnumerable<Post> RecentPosts { get; set; }
        public int CurrentPages { get; set; }
        public int? SelectedCategoryId { get; set; }
        public int? SelectedTagId { get; set; }
        public int TotalPages { get; set; }
        public int LimitPage { get; set; }
    }
}
