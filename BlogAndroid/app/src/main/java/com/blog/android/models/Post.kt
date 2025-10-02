package com.blog.android.models

data class Post(
    val id: Int,
    val title: String,
    val content: String,
    val imageUrl: String?,
    val likesCount: Int,
    val createdAt: String,
    val updatedAt: String,
    val author: User,
    val isLikedByCurrentUser: Boolean
)

data class CreatePost(
    val title: String,
    val content: String,
    val imageUrl: String?
)

data class UpdatePost(
    val title: String,
    val content: String,
    val imageUrl: String?
)
