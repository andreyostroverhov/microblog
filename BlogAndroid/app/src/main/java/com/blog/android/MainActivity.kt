package com.blog.android

import android.content.Intent
import android.os.Bundle
import android.view.View
import android.widget.ProgressBar
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.blog.android.adapters.PostAdapter
import com.blog.android.api.ApiClient
import com.blog.android.api.TokenManager
import com.blog.android.models.Post
import com.google.android.material.appbar.MaterialToolbar
import com.google.android.material.floatingactionbutton.FloatingActionButton
import kotlinx.coroutines.*


class MainActivity : AppCompatActivity() {
    
    private lateinit var recyclerView: RecyclerView
    private lateinit var progressBar: ProgressBar
    private lateinit var fabLogin: FloatingActionButton
    private lateinit var toolbar: MaterialToolbar
    private lateinit var postAdapter: PostAdapter
    
    private val scope = CoroutineScope(Dispatchers.Main + SupervisorJob())
    
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)
        
        initViews()
        setupRecyclerView()
        setupToolbar()
        setupFab()
        
        loadPosts()
    }
    
    private fun initViews() {
        recyclerView = findViewById(R.id.recyclerViewPosts)
        progressBar = findViewById(R.id.progressBar)
        fabLogin = findViewById(R.id.fabLogin)
        toolbar = findViewById(R.id.toolbar)
    }
    
    private fun setupRecyclerView() {
        postAdapter = PostAdapter(mutableListOf()) { post ->
            likePost(post)
        }
        
        recyclerView.apply {
            layoutManager = LinearLayoutManager(this@MainActivity)
            adapter = postAdapter
        }
    }
    
    private fun setupToolbar() {
        setSupportActionBar(toolbar)
        supportActionBar?.title = "Мой Блог"
    }
    
    private fun setupFab() {
        fabLogin.setOnClickListener {
            startActivity(Intent(this, LoginActivity::class.java))
        }
    }
    
    private fun loadPosts() {
        showLoading(true)
        
        scope.launch {
            try {
                val response = ApiClient.apiService.getAllPosts()
                if (response.isSuccessful) {
                    val posts = response.body() ?: emptyList()
                    postAdapter.updatePosts(posts)
                } else {
                    showError("Ошибка загрузки постов")
                }
            } catch (e: Exception) {
                showError("Ошибка сети: ${e.message}")
            } finally {
                showLoading(false)
            }
        }
    }
    
    private fun likePost(post: Post) {
        val token = TokenManager.getToken()
        if (token == null) {
            Toast.makeText(this, "Войдите в систему для лайков", Toast.LENGTH_SHORT).show()
            startActivity(Intent(this, LoginActivity::class.java))
            return
        }
        
        scope.launch {
            try {
                val response = ApiClient.apiService.toggleLike(post.id)
                if (response.isSuccessful) {
                    // Reload posts to get updated like count
                    loadPosts()
                } else {
                    showError("Ошибка лайка")
                }
            } catch (e: Exception) {
                showError("Ошибка сети: ${e.message}")
            }
        }
    }
    
    private fun showLoading(show: Boolean) {
        progressBar.visibility = if (show) View.VISIBLE else View.GONE
        recyclerView.visibility = if (show) View.GONE else View.VISIBLE
    }
    
    private fun showError(message: String) {
        Toast.makeText(this, message, Toast.LENGTH_LONG).show()
    }
    
    override fun onResume() {
        super.onResume()
        loadPosts()
    }
    
    override fun onDestroy() {
        super.onDestroy()
        scope.cancel()
    }
}
