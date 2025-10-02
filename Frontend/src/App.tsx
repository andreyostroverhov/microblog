import React, { useState, useEffect } from 'react';
import './App.css';
import Header from './components/Header';
import LoginForm from './components/Auth/LoginForm';
import RegisterForm from './components/Auth/RegisterForm';
import PostList from './components/Posts/PostList';
import PostForm from './components/Posts/PostForm';
import { Post, User, CreatePost, UpdatePost } from './types';
import { authAPI, postsAPI } from './services/api';

type AuthMode = 'login' | 'register';

function App() {
  const [user, setUser] = useState<User | null>(null);
  const [posts, setPosts] = useState<Post[]>([]);
  const [authMode, setAuthMode] = useState<AuthMode>('login');
  const [showPostForm, setShowPostForm] = useState(false);
  const [editingPost, setEditingPost] = useState<Post | null>(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    const token = localStorage.getItem('token');
    if (token) {
      loadUser();
    }
    loadPosts();
  }, []);

  const loadUser = async () => {
    try {
      const userData = await authAPI.getCurrentUser();
      setUser(userData);
    } catch (error) {
      localStorage.removeItem('token');
    }
  };

  const loadPosts = async () => {
    try {
      const postsData = await postsAPI.getAllPosts();
      setPosts(postsData);
    } catch (error) {
      console.error('Ошибка загрузки постов:', error);
    }
  };

  const handleLogin = (token: string) => {
    loadUser();
  };

  const handleRegister = (token: string) => {
    loadUser();
  };

  const handleLogout = () => {
    localStorage.removeItem('token');
    setUser(null);
  };

  const handleCreatePost = () => {
    setEditingPost(null);
    setShowPostForm(true);
  };

  const handleEditPost = (post: Post) => {
    setEditingPost(post);
    setShowPostForm(true);
  };

  const handleSubmitPost = async (postData: CreatePost | UpdatePost) => {
    setLoading(true);
    try {
      if (editingPost) {
        const updatedPost = await postsAPI.updatePost(editingPost.id, postData as UpdatePost);
        setPosts(posts.map(p => p.id === editingPost.id ? updatedPost : p));
      } else {
        const newPost = await postsAPI.createPost(postData as CreatePost);
        setPosts([newPost, ...posts]);
      }
      setShowPostForm(false);
      setEditingPost(null);
    } catch (error) {
      console.error('Ошибка сохранения поста:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleDeletePost = async (postId: number) => {
    if (window.confirm('Вы уверены, что хотите удалить этот пост?')) {
      try {
        await postsAPI.deletePost(postId);
        setPosts(posts.filter(p => p.id !== postId));
      } catch (error) {
        console.error('Ошибка удаления поста:', error);
      }
    }
  };

  const handleLikePost = async (postId: number) => {
    try {
      await postsAPI.toggleLike(postId);
      loadPosts(); // Перезагружаем посты для обновления лайков
    } catch (error) {
      console.error('Ошибка лайка:', error);
    }
  };

  if (!user) {
    return (
      <div className="App">
        <div className="auth-container">
          {authMode === 'login' ? (
            <LoginForm
              onLogin={handleLogin}
              onSwitchToRegister={() => setAuthMode('register')}
            />
          ) : (
            <RegisterForm
              onRegister={handleRegister}
              onSwitchToLogin={() => setAuthMode('login')}
            />
          )}
        </div>
      </div>
    );
  }

  return (
    <div className="App">
      <Header
        user={user}
        onLogout={handleLogout}
        onCreatePost={handleCreatePost}
      />
      
      <main className="main-content">
        {showPostForm ? (
          <PostForm
            post={editingPost || undefined}
            onSubmit={handleSubmitPost}
            onCancel={() => {
              setShowPostForm(false);
              setEditingPost(null);
            }}
            loading={loading}
          />
        ) : (
          <PostList
            posts={posts}
            onLike={handleLikePost}
            onEdit={handleEditPost}
            onDelete={handleDeletePost}
            currentUserId={user.id}
          />
        )}
      </main>
    </div>
  );
}

export default App;
