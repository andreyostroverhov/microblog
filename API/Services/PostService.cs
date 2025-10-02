using Microsoft.EntityFrameworkCore;
using BlogAPI.Data;
using BlogAPI.DTOs;
using BlogAPI.Models;

namespace BlogAPI.Services
{
    public class PostService : IPostService
    {
        private readonly BlogDbContext _context;

        public PostService(BlogDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PostDto>> GetAllPostsAsync(int? userId = null)
        {
            var posts = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Likes)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return posts.Select(p => new PostDto
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
                ImageUrl = p.ImageUrl,
                LikesCount = p.LikesCount,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                Author = new UserDto
                {
                    Id = p.Author.Id,
                    Username = p.Author.Username,
                    Email = p.Author.Email,
                    CreatedAt = p.Author.CreatedAt
                },
                IsLikedByCurrentUser = userId.HasValue && p.Likes.Any(l => l.UserId == userId.Value)
            });
        }

        public async Task<PostDto?> GetPostByIdAsync(int id, int? userId = null)
        {
            var post = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Likes)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
                return null;

            return new PostDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                ImageUrl = post.ImageUrl,
                LikesCount = post.LikesCount,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                Author = new UserDto
                {
                    Id = post.Author.Id,
                    Username = post.Author.Username,
                    Email = post.Author.Email,
                    CreatedAt = post.Author.CreatedAt
                },
                IsLikedByCurrentUser = userId.HasValue && post.Likes.Any(l => l.UserId == userId.Value)
            };
        }

        public async Task<PostDto> CreatePostAsync(CreatePostDto createPostDto, int authorId)
        {
            var post = new Post
            {
                Title = createPostDto.Title,
                Content = createPostDto.Content,
                ImageUrl = createPostDto.ImageUrl,
                AuthorId = authorId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            // Загружаем пост с автором для возврата
            var createdPost = await _context.Posts
                .Include(p => p.Author)
                .FirstAsync(p => p.Id == post.Id);

            return new PostDto
            {
                Id = createdPost.Id,
                Title = createdPost.Title,
                Content = createdPost.Content,
                ImageUrl = createdPost.ImageUrl,
                LikesCount = createdPost.LikesCount,
                CreatedAt = createdPost.CreatedAt,
                UpdatedAt = createdPost.UpdatedAt,
                Author = new UserDto
                {
                    Id = createdPost.Author.Id,
                    Username = createdPost.Author.Username,
                    Email = createdPost.Author.Email,
                    CreatedAt = createdPost.Author.CreatedAt
                },
                IsLikedByCurrentUser = false
            };
        }

        public async Task<PostDto?> UpdatePostAsync(int id, UpdatePostDto updatePostDto, int userId)
        {
            var post = await _context.Posts
                .Include(p => p.Author)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null || post.AuthorId != userId)
                return null;

            post.Title = updatePostDto.Title;
            post.Content = updatePostDto.Content;
            post.ImageUrl = updatePostDto.ImageUrl;
            post.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new PostDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                ImageUrl = post.ImageUrl,
                LikesCount = post.LikesCount,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                Author = new UserDto
                {
                    Id = post.Author.Id,
                    Username = post.Author.Username,
                    Email = post.Author.Email,
                    CreatedAt = post.Author.CreatedAt
                },
                IsLikedByCurrentUser = false
            };
        }

        public async Task<bool> DeletePostAsync(int id, int userId)
        {
            var post = await _context.Posts
                .Include(p => p.Likes)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null || post.AuthorId != userId)
                return false;

            // Удаляем все лайки поста вручную
            _context.Likes.RemoveRange(post.Likes);
            
            // Удаляем сам пост
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleLikeAsync(int postId, int userId)
        {
            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);

            var post = await _context.Posts.FindAsync(postId);
            if (post == null)
                return false;

            if (existingLike != null)
            {
                // Убираем лайк
                _context.Likes.Remove(existingLike);
                post.LikesCount--;
            }
            else
            {
                // Добавляем лайк
                var like = new Like
                {
                    PostId = postId,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Likes.Add(like);
                post.LikesCount++;
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
