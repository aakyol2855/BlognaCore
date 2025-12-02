using BlognaCorev2.Models;
using Microsoft.AspNetCore.Identity;

namespace BlognaCorev2.Services;

public class PostCreationService:IPostCreationService
{
    public readonly IPostRepository _postRepository;
    public PostCreationService(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }
    
    public async Task AddNewPostAsync(Post post, string userId, string authorName)
    {
        post.UserId = userId;
        post.Author = authorName;
        post.CreatedAt = DateTime.Now;
        post.UpdatedAt = DateTime.Now;
        await _postRepository.AddPostAsync(post);
    }
}