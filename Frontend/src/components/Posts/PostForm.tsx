import React, { useState, useEffect } from 'react';
import { Post, CreatePost, UpdatePost } from '../../types';

interface PostFormProps {
  post?: Post;
  onSubmit: (postData: CreatePost | UpdatePost) => void;
  onCancel: () => void;
  loading: boolean;
}

const PostForm: React.FC<PostFormProps> = ({ post, onSubmit, onCancel, loading }) => {
  const [formData, setFormData] = useState<CreatePost>({
    title: '',
    content: '',
    imageUrl: '',
  });

  useEffect(() => {
    if (post) {
      setFormData({
        title: post.title,
        content: post.content,
        imageUrl: post.imageUrl || '',
      });
    }
  }, [post]);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSubmit(formData);
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value,
    });
  };

  return (
    <div className="post-form">
      <h2>{post ? 'Редактировать пост' : 'Создать пост'}</h2>
      <form onSubmit={handleSubmit}>
        <div className="form-group">
          <label htmlFor="title">Заголовок:</label>
          <input
            type="text"
            id="title"
            name="title"
            value={formData.title}
            onChange={handleChange}
            required
          />
        </div>
        
        <div className="form-group">
          <label htmlFor="content">Содержание:</label>
          <textarea
            id="content"
            name="content"
            value={formData.content}
            onChange={handleChange}
            rows={6}
            required
          />
        </div>
        
        <div className="form-group">
          <label htmlFor="imageUrl">URL изображения (необязательно):</label>
          <input
            type="url"
            id="imageUrl"
            name="imageUrl"
            value={formData.imageUrl}
            onChange={handleChange}
            placeholder="https://example.com/image.jpg"
          />
        </div>
        
        <div className="form-actions">
          <button type="submit" disabled={loading}>
            {loading ? 'Сохранение...' : (post ? 'Обновить' : 'Создать')}
          </button>
          <button type="button" onClick={onCancel}>
            Отмена
          </button>
        </div>
      </form>
    </div>
  );
};

export default PostForm;
