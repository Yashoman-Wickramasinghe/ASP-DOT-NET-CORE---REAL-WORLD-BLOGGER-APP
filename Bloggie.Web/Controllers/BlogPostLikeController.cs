using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Bloggie.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostLikeController : ControllerBase
    {
        private readonly IBlogPostLikeRepositroy blogPostLikeRepositroy;

        public BlogPostLikeController(IBlogPostLikeRepositroy blogPostLikeRepositroy)
        {
            this.blogPostLikeRepositroy = blogPostLikeRepositroy;
        }

        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> AddLike([FromBody] AddLikeRequest addLikeRequest)
        {
            var model = new BlogPostLike
            {
                BlogPostId = addLikeRequest.BlogPostId,
                UserId = addLikeRequest.UserId
            };

            await blogPostLikeRepositroy.AddLikeForBlog(model);

            return Ok();
        }

        [HttpGet]
        [Route("{blogPostId:Guid}/totalLikes")]
        public async Task<IActionResult> GetTotalLikeForBlog([FromRoute] Guid blogPostId)
        {
            var totalLikes = await blogPostLikeRepositroy.GetTotalLikes(blogPostId);

            return Ok(totalLikes);
        }
    }
}
