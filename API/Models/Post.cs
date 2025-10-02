using System.ComponentModel.DataAnnotations;

namespace BlogAPI.Models
{
    public class Post
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Content { get; set; } = string.Empty;
        
        public string? ImageUrl { get; set; }
        
        public int LikesCount { get; set; } = 0;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Внешний ключ
        public int AuthorId { get; set; }
        
        // Навигационные свойства
        public User Author { get; set; } = null!;
        public ICollection<Like> Likes { get; set; } = new List<Like>();
    }
}
