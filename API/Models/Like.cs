namespace BlogAPI.Models
{
    public class Like
    {
        public int Id { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Внешние ключи
        public int UserId { get; set; }
        public int PostId { get; set; }
        
        // Навигационные свойства
        public User User { get; set; } = null!;
        public Post Post { get; set; } = null!;
    }
}
