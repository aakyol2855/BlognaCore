using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using BlognaCorev2.Data;
using BlognaCorev2.Models;
using Microsoft.AspNetCore.Components.Web;

namespace BlognaCorev2.Services;

public class EfCorePostRepository:IPostRepository
{
    private readonly ApplicationDbContext _context;

    public EfCorePostRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Post>> GetAllPostsAsync()
    {
        return await _context.Posts.ToListAsync();
    }

    public async Task<Post?> GetPostByIdAsync(int id)
    {
        return await _context.Posts.FindAsync(id);
    }

    public async Task AddPostAsync(Post post)
    {
        _context.Posts.AddAsync(post);
        await _context.SaveChangesAsync();
    }

    public async Task UpdatePostAsync(Post post)
    {
        post.UpdatedAt = DateTime.Now;
        _context.Posts.Update(post);
        _context.SaveChangesAsync();
    }

    public async Task DeletePostAsync(int id)
    {
        var postToDelete =await _context.Posts.FindAsync(id);
        _context.Posts.Remove(postToDelete);
        _context.SaveChangesAsync();
    }

    public async Task<bool> IsPostOwnerAsync(int postId , string userId)
    {
        var owner =await _context.Posts.AnyAsync(post=> post.Id == postId && post.UserId == userId);
        return owner;
    }

    public async Task<IList<Post>> GetPublishedPostAsync()
    {
        return await _context.Posts.Where(post => post.Ispublished == true).ToListAsync();
    }

    public async Task<IList<Post>> GetPostsByUserIdAsync(string userId)
    {
        return  await _context.Posts.Where(post => post.UserId == userId).ToListAsync();
    }

    public async Task<PageListViewModel<Post>> GetPostsByQueryAsync(PostQueryModel queryModel)
    {
        var posts = _context.Posts.Where(p => p.Ispublished).AsQueryable();

        if (!string.IsNullOrWhiteSpace(queryModel.SearchTerm))
        {
            posts = posts.Where(p => p.Title.Contains(queryModel.SearchTerm.ToLower())
                                || p.Content.ToLower().Contains(queryModel.SearchTerm.ToLower())
                                );
        }
        
        var totalItems = await posts.CountAsync();
        posts = posts.OrderByDescending(p=>p.UpdatedAt);
        var skipAmount = (queryModel.PageIndex - 1) * queryModel.PageSize;

        posts = posts.Skip(skipAmount);
        posts = posts.Take(queryModel.PageSize);
        var pagedItems = await posts.ToListAsync();
        PageListViewModel<Post> returnPage = new PageListViewModel<Post>(pagedItems,totalItems,queryModel.PageSize,queryModel.PageIndex,queryModel.SearchTerm);
        return returnPage;
    }

    public async Task<PageListViewModel<Post>> GetPostsByQueryAndIdAsybc(PostQueryModel queryModel,string userId)
    {
        var posts = _context.Posts.Where(p => p.UserId == userId).AsQueryable();

        if (!string.IsNullOrWhiteSpace(queryModel.SearchTerm))
        {
            posts = posts.Where(p => p.Title.Contains(queryModel.SearchTerm.ToLower())
                                     || p.Content.ToLower().Contains(queryModel.SearchTerm.ToLower())
            );
        }
        
        var totalItems = await posts.CountAsync();
        posts = posts.OrderByDescending(p=>p.UpdatedAt);
        var skipAmount = (queryModel.PageIndex - 1) * queryModel.PageSize;

        posts = posts.Skip(skipAmount);
        posts = posts.Take(queryModel.PageSize);
        var pagedItems = await posts.ToListAsync();
        PageListViewModel<Post> returnPage = new PageListViewModel<Post>(pagedItems,totalItems,queryModel.PageSize,queryModel.PageIndex,queryModel.SearchTerm);
        return returnPage;
    }
}

