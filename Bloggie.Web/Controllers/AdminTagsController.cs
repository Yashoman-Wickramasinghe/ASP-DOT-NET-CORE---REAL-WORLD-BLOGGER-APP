using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Controllers
{
    public class AdminTagsController : Controller
    {
        private readonly BloggieDbContext _bloggieDbContext;
        public AdminTagsController(BloggieDbContext bloggieDbContext)
        {
            _bloggieDbContext = bloggieDbContext;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddTagRequest addTagRequest)
        {
            // Mapping the AddTagRequest to the Tag domain model
            var tag = new Tag
            {
                Name = addTagRequest.Name,
                DisplayName = addTagRequest.DisplayName
            };

            await _bloggieDbContext.Tags.AddAsync(tag);
            await _bloggieDbContext.SaveChangesAsync();

            return RedirectToAction("List");
        }

        [HttpGet]
        [ActionName("List")]
        public async Task<IActionResult> List()
        {
            // Use BloggieDbContext to read the tags
            var tags = await _bloggieDbContext.Tags.ToListAsync();

            return View(tags);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid Id) 
        {
            // Use BloggieDbContext to read the tag by Id
            var tag = await _bloggieDbContext.Tags.FirstOrDefaultAsync(t => t.Id == Id);

            if (tag != null)
            {
                // bind the tag to the EditTagRequest ViewModel
                var editTagRequest = new EditTagRequest
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    DisplayName = tag.DisplayName
                };
                return View(editTagRequest);
            }

            return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditTagRequest editTagRequest) 
        { 
            // Convert back into the tag to Main Domain Model
            var tag = new Tag
            {
                Id = editTagRequest.Id,
                Name = editTagRequest.Name,
                DisplayName = editTagRequest.DisplayName
            };

            // Validate the tag
            var existingTag = await _bloggieDbContext.Tags.FindAsync(tag.Id);

            if(existingTag != null)
            {
                existingTag.Name = tag.Name;
                existingTag.DisplayName = tag.DisplayName;

                // save the changes
               await _bloggieDbContext.SaveChangesAsync();

                // show success notification
                return RedirectToAction("Edit", new { id = editTagRequest.Id });
            }
            // show success error notification
            return RedirectToAction("Edit", new { id = editTagRequest.Id});
        }

        [HttpPost]
        public async Task<IActionResult> Delete(EditTagRequest editTagRequest)
        {
            var tag = await _bloggieDbContext.Tags.FindAsync(editTagRequest.Id);

            if (tag != null) 
            {
                _bloggieDbContext.Tags.Remove(tag);
                await _bloggieDbContext.SaveChangesAsync();

                // show success notification
                return RedirectToAction("List");
            }

            // show error notification
            return View("Edit", new { id = editTagRequest.Id });
        }
    }
}
