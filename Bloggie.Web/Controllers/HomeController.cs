using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Bloggie.Web.Models;
using Bloggie.Web.Repositories;
using Bloggie.Web.Models.ViewModels;

namespace Bloggie.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IBlogPostRepository blogPostRepository;
    private readonly ITagRepository tagRepository;

    public HomeController(ILogger<HomeController> logger, IBlogPostRepository blogPostRepository, ITagRepository tagRepository)
    {
        _logger = logger;
        this.blogPostRepository = blogPostRepository;
        this.tagRepository = tagRepository;
    }

    public async Task <IActionResult> Index()
    {
        // getting all blogs
        var blogPost = await blogPostRepository.GetAllAsync();

        // get all tags
        var tags = await tagRepository.GetAllAsync();

        var model = new HomeViewModel
        {
            BlogPosts = blogPost,
            Tags = tags
        };

        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
