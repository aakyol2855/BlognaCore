using BlognaCorev2.Models;

namespace BlognaCorev2.Services;

public interface IPostRepository
{
    Task<List<Post>> GetAllPostsAsync();

    Task<Post?> GetPostByIdAsync(int id);

    Task AddPostAsync(Post post);

    Task UpdatePostAsync(Post post);

    Task DeletePostAsync(int id);

    Task<bool> IsPostOwnerAsync(int postId, string userId);
    
    Task<IList<Post>> GetPublishedPostAsync();

    Task<IList<Post>> GetPostsByUserIdAsync(string userId);

    Task<PageListViewModel<Post>> GetPostsByQueryAsync(PostQueryModel queryModel);
    Task<PageListViewModel<Post>> GetPostsByQueryAndIdAsybc(PostQueryModel queryModel, string userId);
}