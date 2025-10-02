import React from 'react';
import { Post } from '../../types';
import PostCard from './PostCard';

interface PostListProps {
  posts: Post[];
  onLike: (postId: number) => void;
  onEdit: (post: Post) => void;
  onDelete: (postId: number) => void;
  currentUserId?: number;
}

const PostList: React.FC<PostListProps> = ({ 
  posts, 
  onLike, 
  onEdit, 
  onDelete, 
  currentUserId 
}) => {
  if (posts.length === 0) {
    return (
      <div className="no-posts">
        <p>Пока нет постов. Создайте первый пост!</p>
      </div>
    );
  }

  return (
    <div className="post-list">
      {posts.map((post) => (
        <PostCard
          key={post.id}
          post={post}
          onLike={onLike}
          onEdit={onEdit}
          onDelete={onDelete}
          canEdit={currentUserId === post.author.id}
        />
      ))}
    </div>
  );
};

export default PostList;
