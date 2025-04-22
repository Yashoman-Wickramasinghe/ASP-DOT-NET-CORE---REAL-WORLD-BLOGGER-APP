using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminTagsController : Controller
    {
        private readonly ITagRepository tagRepository;

        public AdminTagsController(ITagRepository tagRepository)
        {
            this.tagRepository = tagRepository;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }


        [HttpPost]
        [ActionName("Add")]
        public async Task<IActionResult> Add(AddTagRequest addTagRequest)
        {
            // custom validation
            ValidateAddTagRequest(addTagRequest);

            if (ModelState.IsValid == false)
            {
                // show error notification
                return View();
            }
            // Mapping the AddTagRequest to the Tag domain model
            var tag = new Tag
            {
                Name = addTagRequest.Name,
                DisplayName = addTagRequest.DisplayName
            };

            await tagRepository.AddAsync(tag);

            return RedirectToAction("List");
        }

        [HttpGet]
        [ActionName("List")]
        public async Task<IActionResult> List(
            string? searchQuery, 
            string? sortBy, 
            string? sortDirection, 
            int pageSize = 3,
            int pageNumber = 1)
        {
            var totalRecords = await tagRepository.CountAsync();
            var totalPages = Math.Ceiling((decimal)totalRecords / pageSize);

            if (pageNumber > totalPages)
            {
                pageNumber--;
            }

            if (pageNumber < 1) 
            { 
                pageNumber++;
            }

            ViewBag.TotalPages = totalPages;
            ViewBag.SearchQuery = searchQuery;
            ViewBag.SortBy = sortBy;
            ViewBag.SortDirection = sortDirection;
            ViewBag.PageSize = pageSize;
            ViewBag.PageNumber = pageNumber;

            var tags = await tagRepository.GetAllAsync(searchQuery, sortBy, sortDirection, pageSize, pageNumber);
            //tags.Where(x => x.Name == searchQuery);
            return View(tags);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid Id) 
        {
            // Use BloggieDbContext to read the tag by Id
            var tag = await tagRepository.GetAsync(Id);

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
            
            var updatedTag = await tagRepository.UpdateAsync(tag);

            if (updatedTag != null)
            {
                // show success notification
            }
            else 
            {
                // show success notification
            }
            return RedirectToAction("Edit", new { id = editTagRequest.Id});
        }

        [HttpPost]
        public async Task<IActionResult> Delete(EditTagRequest editTagRequest)
        {
            var deletedTag = await tagRepository.DeleteAsync(editTagRequest.Id);

            if (deletedTag != null)
            {
                // show success notification
                return RedirectToAction("List");
            }

            // show error notification
            return View("Edit", new { id = editTagRequest.Id });
        }

        private void ValidateAddTagRequest(AddTagRequest request) 
        {
            if (request.Name is not null && request.DisplayName is not null) 
            {
                if (request.Name == request.DisplayName)
                {
                    ModelState.AddModelError("DisplayName", "Name cannot be the same as DisplayName.");
                }
            }
        }
    }
}
