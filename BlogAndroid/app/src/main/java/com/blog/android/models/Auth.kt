package com.blog.android.models

data class CreateUser(
    val username: String,
    val email: String,
    val password: String
)

data class LoginUser(
    val username: String,
    val password: String
)

data class AuthResponse(
    val token: String
)
