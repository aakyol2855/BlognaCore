using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BlognaCorev2.Models;
using BlognaCorev2.Services;

namespace BlognaCorev2.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IPostRepository _postRepository;

    public HomeController(ILogger<HomeController> logger , IPostRepository postRepository)
    {
        _logger = logger;
        _postRepository = postRepository;
    }

    public async Task<ViewResult> Index(PostQueryModel queryModel)
    {
        var publishedPosts = await _postRepository.GetPostsByQueryAsync(queryModel);
        return View(publishedPosts);
    }

    public IActionResult About()
    {
        return View();
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

    public IActionResult Contact()
    {
        return View();
    }

    public IActionResult Search()
    {
        return RedirectToAction("Search","Posts");
    }
}