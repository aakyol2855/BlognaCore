using BlognaCorev2.Models;

namespace BlognaCorev2.Services;

public interface IPostCreationService
{
    Task AddNewPostAsync(Post post, string userId, string authorName);
}