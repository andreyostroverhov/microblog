import React from 'react';
import { Post } from '../../types';

interface PostCardProps {
  post: Post;
  onLike: (postId: number) => void;
  onEdit: (post: Post) => void;
  onDelete: (postId: number) => void;
  canEdit: boolean;
}

const PostCard: React.FC<PostCardProps> = ({ 
  post, 
  onLike, 
  onEdit, 
  onDelete, 
  canEdit 
}) => {
  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('ru-RU', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  return (
    <div className="post-card">
      <div className="post-header">
        <h3>{post.title}</h3>
        <div className="post-meta">
          <span className="author">Автор: {post.author.username}</span>
          <span className="date">{formatDate(post.createdAt)}</span>
        </div>
      </div>
      
      <div className="post-content">
        <p>{post.content}</p>
        {post.imageUrl && (
          <img src={post.imageUrl} alt={post.title} className="post-image" />
        )}
      </div>
      
      <div className="post-footer">
        <button 
          className={`like-button ${post.isLikedByCurrentUser ? 'liked' : ''}`}
          onClick={() => onLike(post.id)}
        >
          ❤️ {post.likesCount}
        </button>
        
        {canEdit && (
          <div className="post-actions">
            <button 
              className="edit-button"
              onClick={() => onEdit(post)}
            >
              Редактировать
            </button>
            <button 
              className="delete-button"
              onClick={() => onDelete(post.id)}
            >
              Удалить
            </button>
          </div>
        )}
      </div>
    </div>
  );
};

export default PostCard;
