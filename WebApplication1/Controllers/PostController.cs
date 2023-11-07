using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Extensions;
using WebApplication1.Helpers;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    public class PostController : Controller
    {
        private readonly BlogDbContext blogDbContext;

        public PostController(BlogDbContext blogDbContext)
        {
            this.blogDbContext = blogDbContext;
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {

            var posts = blogDbContext.Posts.Find(id);

            ViewBag.categories = new SelectList(blogDbContext.Categories, "Id", "Name");

            var selectedTagIds = blogDbContext.PostTags.Where(x => x.PostId == id).Select(x => x.TagId);
            ViewBag.tags = new MultiSelectList(blogDbContext.Tags, "Id", "Name", selectedTagIds);

            return View(posts);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {

            var posts = blogDbContext.Posts.Find(id);
            return View(posts);
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var posts = blogDbContext.Posts.Find(id);
            blogDbContext.Posts.Remove(posts);
            await blogDbContext.SaveChangesAsync();
            TempData["status"] = "Post DELETED!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Post post, IFormFile Image, int[] tags)
        {
            if (Image != null)
            {
                var path = await FileUploadHelper.UploadAsync(Image);
                post.ImageUrl = path;
            }

            post.Date = DateTime.Now;

            blogDbContext.Posts.Update(post);
            await blogDbContext.SaveChangesAsync();

            var postWithTags = blogDbContext.Posts.Include(x => x.PostTags).FirstOrDefault(x => x.Id == post.Id);
            blogDbContext.UpdateManyToMany(
                postWithTags.PostTags,
                tags.Select(x => new PostTag { PostId = post.Id, TagId = x }),
                x => x.TagId
                );
            await blogDbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.categories = new SelectList(blogDbContext.Categories, "Id", "Name");
            ViewBag.tags = new MultiSelectList(blogDbContext.Tags, "Id", "Name");
            return View();
        }

        [HttpGet]
        public IActionResult Index(int? categoryId = null, int? tagId = null, int page = 1)
        {
            var posts = blogDbContext.Posts.Include(x => x.PostTags).ThenInclude(x => x.Tag).Include(x => x.Category).OrderByDescending(x => x.Id);

            if (categoryId != null && tagId != null)
            {
                posts = (IOrderedQueryable<Post>)posts.Where(x => x.CategoryId == categoryId && x.PostTags.Any(pt => pt.TagId == tagId));
            }
            else if (categoryId != null)
            {
                posts = (IOrderedQueryable<Post>)posts.Where(x => x.CategoryId == categoryId);
            }
            else if (tagId != null)
            {
                posts = (IOrderedQueryable<Post>)posts.Where(x => x.PostTags.Any(pt => pt.TagId == tagId));
            }
            else
            {
                ;
            }

            var model = new IndexViewModel()
            {
                Categories = blogDbContext.Categories,
                Posts = posts,
                Tags = blogDbContext.Tags,
                RecentPosts = blogDbContext.Posts.OrderByDescending(x => x.Id).Take(3),
                CurrentPages = page,
                TotalPages = 10,
                SelectedCategoryId = categoryId,
                SelectedTagId = tagId
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var posts = blogDbContext.Posts
                .Include(x => x.PostTags).ThenInclude(x => x.Tag)
                .Include(x => x.Category)
                .FirstOrDefault(posts => posts.Id == id);
            return View(posts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Post post, IFormFile Image, int[] tags)
        {
            post.ImageUrl = await FileUploadHelper.UploadAsync(Image);
            if (Image != null)
            {
                var filename = $"{Guid.NewGuid()}{Path.GetExtension(Image.FileName)}";
                using var fs = new FileStream(@$"wwwroot/uploads/{filename}", FileMode.Create);
                await Image.CopyToAsync(fs);
                post.ImageUrl = @$"/uploads/{filename}";

            }

            TempData["status"] = "New post added!";
            post.Date = DateTime.Now;
            await blogDbContext.Posts.AddAsync(post);
            await blogDbContext.SaveChangesAsync();

            blogDbContext.PostTags.AddRange(tags.Select(x => new PostTag { PostId = post.Id, TagId = x }));

            await blogDbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
