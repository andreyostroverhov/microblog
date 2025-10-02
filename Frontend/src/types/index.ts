export interface User {
  id: number;
  username: string;
  email: string;
  createdAt: string;
}

export interface Post {
  id: number;
  title: string;
  content: string;
  imageUrl?: string;
  likesCount: number;
  createdAt: string;
  updatedAt: string;
  author: User;
  isLikedByCurrentUser: boolean;
}

export interface CreatePost {
  title: string;
  content: string;
  imageUrl?: string;
}

export interface UpdatePost {
  title: string;
  content: string;
  imageUrl?: string;
}

export interface CreateUser {
  username: string;
  email: string;
  password: string;
}

export interface LoginUser {
  username: string;
  password: string;
}

export interface AuthResponse {
  token: string;
}
