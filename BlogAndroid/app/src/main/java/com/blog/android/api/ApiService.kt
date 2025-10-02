package com.blog.android.api

import com.blog.android.models.*
import retrofit2.Response
import retrofit2.http.*

interface ApiService {
    
    // Auth endpoints
    @POST("auth/register")
    suspend fun register(@Body user: CreateUser): Response<AuthResponse>
    
    @POST("auth/login")
    suspend fun login(@Body user: LoginUser): Response<AuthResponse>
    
    @GET("auth/me")
    suspend fun getCurrentUser(): Response<User>
    
    // Posts endpoints
    @GET("posts")
    suspend fun getAllPosts(): Response<List<Post>>
    
    @GET("posts/{id}")
    suspend fun getPost(@Path("id") id: Int): Response<Post>
    
    @POST("posts")
    suspend fun createPost(@Body post: CreatePost): Response<Post>
    
    @PUT("posts/{id}")
    suspend fun updatePost(@Path("id") id: Int, @Body post: UpdatePost): Response<Post>
    
    @DELETE("posts/{id}")
    suspend fun deletePost(@Path("id") id: Int): Response<Void>
    
    @POST("posts/{id}/like")
    suspend fun toggleLike(@Path("id") id: Int): Response<Void>
}
