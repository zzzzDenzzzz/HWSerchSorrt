namespace WebApplication1.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public IEnumerable<PostTag> PostTags { get; set; }
    }
}
