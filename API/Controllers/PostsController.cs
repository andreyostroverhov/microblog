using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BlogAPI.DTOs;
using BlogAPI.Services;

namespace BlogAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPosts()
        {
            var userId = GetCurrentUserId();
            var posts = await _postService.GetAllPostsAsync(userId);
            return Ok(posts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPost(int id)
        {
            var userId = GetCurrentUserId();
            var post = await _postService.GetPostByIdAsync(id, userId);
            if (post == null)
            {
                return NotFound();
            }
            return Ok(post);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostDto createPostDto)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var post = await _postService.CreatePostAsync(createPostDto, userId.Value);
            return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePost(int id, [FromBody] UpdatePostDto updatePostDto)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var post = await _postService.UpdatePostAsync(id, updatePostDto, userId.Value);
            if (post == null)
            {
                return NotFound();
            }

            return Ok(post);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePost(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var result = await _postService.DeletePostAsync(id, userId.Value);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPost("{id}/like")]
        [Authorize]
        public async Task<IActionResult> ToggleLike(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var result = await _postService.ToggleLikeAsync(id, userId.Value);
            if (!result)
            {
                return NotFound();
            }

            return Ok();
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userIdClaim != null && int.TryParse(userIdClaim, out int userId) ? userId : null;
        }
    }
}
