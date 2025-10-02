using BlogAPI.DTOs;

namespace BlogAPI.Services
{
    public interface IPostService
    {
        Task<IEnumerable<PostDto>> GetAllPostsAsync(int? userId = null);
        Task<PostDto?> GetPostByIdAsync(int id, int? userId = null);
        Task<PostDto> CreatePostAsync(CreatePostDto createPostDto, int authorId);
        Task<PostDto?> UpdatePostAsync(int id, UpdatePostDto updatePostDto, int userId);
        Task<bool> DeletePostAsync(int id, int userId);
        Task<bool> ToggleLikeAsync(int postId, int userId);
    }
}
