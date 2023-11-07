namespace WebApplication1.Models
{
    public static class BlogDbInitializer
    {
        static public void seed(IApplicationBuilder app)
        {
           var result =  app.ApplicationServices.CreateScope();
           var context =  result.ServiceProvider.GetRequiredService<BlogDbContext>();

            if (!context.Categories.Any())
            {
                context.Categories.Add(new Category() { Name = "Sport" });
                context.Categories.Add(new Category() { Name = "Fun" });
                context.Categories.Add(new Category() { Name = "News" });
                context.Categories.Add(new Category() { Name = "Music" });
                context.Categories.Add(new Category() { Name = "Education" });
                context.SaveChanges();
            }

            if (!context.Tags.Any())
            {
                context.Tags.Add(new Tag() { Name = "one" });
                context.Tags.Add(new Tag() { Name = "two" });
                context.Tags.Add(new Tag() { Name = "three" });
                context.Tags.Add(new Tag() { Name = "four" });
                context.Tags.Add(new Tag() { Name = "five" });
                context.SaveChanges();
            }

            //if (!context.Posts.Any())
            //{
            //    context.Posts.Add(new Post() { Name = "Sport" });
            //    context.Posts.Add(new Post() { Name = "Fun" });
            //    context.Posts.Add(new Post() { Name = "News" });
            //    context.Posts.Add(new Post() { Name = "Music" });
            //}
        }
    }
}
