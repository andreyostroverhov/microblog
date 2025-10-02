import axios from 'axios';
import { Post, CreatePost, UpdatePost, CreateUser, LoginUser, AuthResponse, User } from '../types';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:7000/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Добавляем токен к запросам
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export const authAPI = {
  register: async (userData: CreateUser): Promise<AuthResponse> => {
    const response = await api.post('/auth/register', userData);
    return response.data;
  },

  login: async (userData: LoginUser): Promise<AuthResponse> => {
    const response = await api.post('/auth/login', userData);
    return response.data;
  },

  getCurrentUser: async (): Promise<User> => {
    const response = await api.get('/auth/me');
    return response.data;
  },
};

export const postsAPI = {
  getAllPosts: async (): Promise<Post[]> => {
    const response = await api.get('/posts');
    return response.data;
  },

  getPost: async (id: number): Promise<Post> => {
    const response = await api.get(`/posts/${id}`);
    return response.data;
  },

  createPost: async (postData: CreatePost): Promise<Post> => {
    const response = await api.post('/posts', postData);
    return response.data;
  },

  updatePost: async (id: number, postData: UpdatePost): Promise<Post> => {
    const response = await api.put(`/posts/${id}`, postData);
    return response.data;
  },

  deletePost: async (id: number): Promise<void> => {
    await api.delete(`/posts/${id}`);
  },

  toggleLike: async (id: number): Promise<void> => {
    await api.post(`/posts/${id}/like`);
  },
};

export default api;
