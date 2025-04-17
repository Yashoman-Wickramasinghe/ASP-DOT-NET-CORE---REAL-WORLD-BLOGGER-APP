using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bloggie.Web.Controllers
{
    public class BlogsController : Controller
    {
        private readonly IBlogPostRepository blogPostRepository;
        private readonly IBlogPostLikeRepositroy blogPostLikeRepositroy;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IBlogPostCommentRepository blogPostCommentRepository;

        public BlogsController(IBlogPostRepository blogPostRepository, IBlogPostLikeRepositroy blogPostLikeRepositroy, SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IBlogPostCommentRepository blogPostCommentRepository) 
        {
            this.blogPostRepository = blogPostRepository;
            this.blogPostLikeRepositroy = blogPostLikeRepositroy;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.blogPostCommentRepository = blogPostCommentRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string urlHandle)
        {

            var liked = false;

            var blogPost = await blogPostRepository.GetByUrlHandleAsync(urlHandle);

            var blogPostDetailsViewModel = new BlogDetailsViewModel();

            if (blogPost != null)
            {
                var totalLikes = await blogPostLikeRepositroy.GetTotalLikes(blogPost.Id);

                if (signInManager.IsSignedIn(User)) 
                {
                    // Get like for this blog for this user - The signed in user already liked this blog post
                    var likesForBlog = await blogPostLikeRepositroy.GetLikesForBlog(blogPost.Id);

                    var userId = userManager.GetUserId(User);

                    if (userId != null)
                    { 
                        var likeFromUser = likesForBlog.FirstOrDefault(x => x.UserId == Guid.Parse(userId));
                        liked = likeFromUser != null;
                    }
                }

                // get comments for blog post
                var blogCommentsDomainModel = await blogPostCommentRepository.GetCommentsByBlogPostIdAsync(blogPost.Id);

                var blogCommentsForView = new List<BlogCommentViewModel>();

                foreach (var comment in blogCommentsDomainModel)
                {
                    blogCommentsForView.Add(new BlogCommentViewModel { 
                        Description = comment.Description,
                        DateAdded = comment.DateAdded,
                        UserName = (await userManager.FindByIdAsync(comment.UserId.ToString())).UserName
                    });
                }

                // Convert Domain Model into View Model
                blogPostDetailsViewModel = new BlogDetailsViewModel
                {
                    Id = blogPost.Id,
                    Content = blogPost.Content,
                    PageTitle = blogPost.PageTitle,
                    Author = blogPost.Author,
                    FeaturedImageUrl = blogPost.FeaturedImageUrl,
                    Heading = blogPost.Heading,
                    PublishedDate = blogPost.PublishedDate,
                    ShortDescription = blogPost.ShortDescription,
                    UrlHandle = blogPost.UrlHandle,
                    Visible = blogPost.Visible,
                    Tags = blogPost.Tags,
                    TotalLikes = totalLikes,
                    Liked = liked,
                    Comments = blogCommentsForView
                };

            }

            return View(blogPostDetailsViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(BlogDetailsViewModel blogDetailsViewModel)
        {
            if (signInManager.IsSignedIn(User)) 
            {
                var domainModel = new BlogPostComment
                {
                    BlogPostId = blogDetailsViewModel.Id,
                    Description = blogDetailsViewModel.CommentDescription,
                    UserId = Guid.Parse(userManager.GetUserId(User)),
                    DateAdded = DateTime.Now
                };

                await blogPostCommentRepository.AddAsync(domainModel);
                return RedirectToAction("Index", "Blogs", new {UrlHandle = blogDetailsViewModel.UrlHandle });
            }

            return Forbid();
        }
    }
}
