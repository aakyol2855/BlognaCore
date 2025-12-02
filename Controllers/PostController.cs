using BlognaCorev2.Models;
using BlognaCorev2.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Protocols;

namespace BlognaCorev2.Controllers;

[Authorize]
public class PostsController :  Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IPostCreationService _postCreationService;
    private readonly IPostRepository _postRepository;

    public PostsController(IPostCreationService postCreationService,IPostRepository postRepository , UserManager<IdentityUser> userManager)
    {
        _postCreationService = postCreationService;
        _postRepository = postRepository;
        _userManager = userManager;
        //claimbased autherization
    }
    
    public async Task<IActionResult> Index()
    {
        return View("Index");
    }
    
    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var post = await _postRepository.GetPostByIdAsync(id);
        if (post == null) return NotFound();
        if (!post.Ispublished && !User.Identity.IsAuthenticated)
        {
            return Forbid(); // veya RedirectToAction("Login", "Account", new { area = "Identity" });
        }
        return View(post);
    }
        
    public IActionResult Create()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(Post post)
    {        
        var authorName = User.FindFirstValue(ClaimTypes.Name) ?? "Anonim";
        var currentUser = await _userManager.GetUserAsync(User);
        var userId = currentUser.Id;
        if (currentUser == null) return Forbid();
        if (!ModelState.IsValid) return View(post);
        
        await _postCreationService.AddNewPostAsync(post, userId, authorName);

        if (post.Ispublished)
        {
            TempData["SuccessMessage"] = "Gönderi oluşturuldu";
            return RedirectToAction("Index","Home");
        }
        
        TempData["SuccessMessage"] = "Gönderi oluşturuldu";
        return RedirectToAction("MyPosts");
    }
    
    public async Task<IActionResult> Edit(int id)
    {
        var post = await _postRepository.GetPostByIdAsync(id);
        if (post == null) return NotFound();
        if (post.Ispublished)
        {
            TempData["ErrorMessage"] = "Bu gönderi şu anda yayında olduğu için düzenlenemez. Düzenlemek için önce taslağa almalısınız.";
            return RedirectToAction("MyPosts");
        }
        return View(post);     

    }

    [HttpPost]
    public async Task<IActionResult> Edit(Post post)
    {
        var postToUpdate = await _postRepository.GetPostByIdAsync(post.Id);
        if (postToUpdate == null) return NotFound();
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null) return Forbid();
        var isOwner = await _postRepository.IsPostOwnerAsync(postToUpdate.Id, currentUser.Id);
        if (!isOwner)
        {
            return Forbid();
        }
        if (postToUpdate.Ispublished)
        {
            TempData["ErrorMessage"] = "Bu gönderi yayında olduğu için düzenlenemez. Önce taslağa almalısınız.";
            return RedirectToAction("MyPosts");
        }

        post.UserId = postToUpdate.UserId;
        post.Author = postToUpdate.Author;
        
        postToUpdate.Title = post.Title;
        postToUpdate.Content = post.Content;
        postToUpdate.UpdatedAt = DateTime.Now;
        postToUpdate.Ispublished = post.Ispublished;

        if (!ModelState.IsValid) return View(post);
        
        await _postRepository.UpdatePostAsync(postToUpdate);
        
        TempData["SuccessMessage"] = $"'{post.Title}' başlıklı gönderiniz başarıyla güncellendi.";
        Console.WriteLine("Is published => "+postToUpdate.Ispublished+" Title => "+postToUpdate.Title);
        Console.WriteLine("----------------------------------");
        return RedirectToAction("MyPosts");
    }

    public async Task<IActionResult> Delete(int id)
    {
        var fetchedPost = await _postRepository.GetPostByIdAsync(id);
        if (fetchedPost == null) return NotFound();
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null) return Forbid();
        var isOwner = await _postRepository.IsPostOwnerAsync(fetchedPost.Id, currentUser.Id);
        if (!isOwner) return Forbid();
        return View(fetchedPost);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        
        var postToDelete = await _postRepository.GetPostByIdAsync(id.Value);
        if (postToDelete == null)
        {
            return NotFound();
        }
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null) return Forbid();

        var isOwner = await _postRepository.IsPostOwnerAsync(postToDelete.Id , currentUser.Id);
        if (!isOwner)
        {
            return Forbid();
        }
        if (postToDelete != null)
        {
            await _postRepository.DeletePostAsync(postToDelete.Id);
        }
        TempData["SuccessMessage"] = $"'{postToDelete.Title}' başlıklı gönderiniz başarıyla silindi.";
        return RedirectToAction("MyPosts");
    }

    public async Task<IActionResult> TogglePublishedAction(int id)
    {
        var fetchedPost = await _postRepository.GetPostByIdAsync(id);
        if (fetchedPost == null) return NotFound();
    
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null) return Forbid();

        var isOwner = await _postRepository.IsPostOwnerAsync(fetchedPost.Id, currentUser.Id);
        if (!isOwner) return Forbid();
        
        fetchedPost.Ispublished = !fetchedPost.Ispublished;
        fetchedPost.UpdatedAt = DateTime.Now;
        await _postRepository.UpdatePostAsync(fetchedPost);
        if (fetchedPost.Ispublished == true)
        {
            TempData["SuccessMessage"] = $"'{fetchedPost.Title}' Başlıklı gönderiniz yayınlandı.";
        }else if (fetchedPost.Ispublished == false)
        {
            TempData["SuccessMessage"] = $"'{fetchedPost.Title}' Başlıklı gönderiniz taslağa alındı..";
        }
        return RedirectToAction("MyPosts");
    }

    [HttpPost]
    public async Task<IActionResult> Search(String query) {
        var publishedPosts = await _postRepository.GetPublishedPostAsync() ?? new List<Post>();

        if (string.IsNullOrWhiteSpace(query)) 
        {
            return RedirectToAction("Index","Home");
        }
        

        List<Post> resultPosts = new List<Post>();
        
        if (publishedPosts == null) return NotFound();
        for (int i = 0; i < publishedPosts.Count; i++)
        {
            var title = publishedPosts[i].Title.ToLower();
            var content = publishedPosts[i].Content.ToLower();
            var q = query.ToLower();
            var b1 = title.Contains(q);
            var b2 = content.Contains(q);
            if (b1 || b2)
            { 
                resultPosts.Add(publishedPosts[i]);
            }
        }
        return View("~/Views/Home/Index.cshtml",resultPosts);
    }
    
    [HttpGet]
    public async Task<IActionResult> MyPosts(PostQueryModel queryModel)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) 
        {
            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }   

        var rawPosts = await _postRepository.GetPostsByQueryAndIdAsybc(queryModel, userId);
        return View(rawPosts); 
    }
}
