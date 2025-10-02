package com.blog.android

import android.content.Intent
import android.os.Bundle
import android.view.View
import android.widget.ProgressBar
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import com.blog.android.api.ApiClient
import com.blog.android.api.TokenManager
import com.blog.android.models.AuthResponse
import com.blog.android.models.LoginUser
import com.blog.android.R
import com.google.android.material.button.MaterialButton
import com.google.android.material.textfield.TextInputEditText
import kotlinx.coroutines.*

class LoginActivity : AppCompatActivity() {
    
    private lateinit var editTextUsername: TextInputEditText
    private lateinit var editTextPassword: TextInputEditText
    private lateinit var buttonLogin: MaterialButton
    private lateinit var buttonRegister: MaterialButton
    private lateinit var progressBar: ProgressBar
    
    private val scope = CoroutineScope(Dispatchers.Main + SupervisorJob())
    
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_login)
        
        initViews()
        setupClickListeners()
    }
    
    private fun initViews() {
        editTextUsername = findViewById(R.id.editTextUsername)
        editTextPassword = findViewById(R.id.editTextPassword)
        buttonLogin = findViewById(R.id.buttonLogin)
        buttonRegister = findViewById(R.id.buttonRegister)
        progressBar = findViewById(R.id.progressBar)
    }
    
    private fun setupClickListeners() {
        buttonLogin.setOnClickListener {
            login()
        }
        
        buttonRegister.setOnClickListener {
            register()
        }
    }
    
    private fun login() {
        val username = editTextUsername.text.toString().trim()
        val password = editTextPassword.text.toString().trim()
        
        if (username.isEmpty() || password.isEmpty()) {
            Toast.makeText(this, "Заполните все поля", Toast.LENGTH_SHORT).show()
            return
        }
        
        showLoading(true)
        
        scope.launch {
            try {
                val loginUser = LoginUser(username, password)
                val response = ApiClient.apiService.login(loginUser)
                
                if (response.isSuccessful) {
                    val authResponse = response.body()
                    authResponse?.let {
                        TokenManager.saveToken(it.token)
                        Toast.makeText(this@LoginActivity, "Успешный вход!", Toast.LENGTH_SHORT).show()
                        finish()
                    }
                } else {
                    showError("Неверное имя пользователя или пароль")
                }
            } catch (e: Exception) {
                showError("Ошибка сети: ${e.message}")
            } finally {
                showLoading(false)
            }
        }
    }
    
    private fun register() {
        val username = editTextUsername.text.toString().trim()
        val password = editTextPassword.text.toString().trim()
        
        if (username.isEmpty() || password.isEmpty()) {
            Toast.makeText(this, "Заполните все поля", Toast.LENGTH_SHORT).show()
            return
        }
        
        showLoading(true)
        
        scope.launch {
            try {
                val createUser = com.blog.android.models.CreateUser(
                    username = username,
                    email = "$username@example.com", // Простая регистрация
                    password = password
                )
                val response = ApiClient.apiService.register(createUser)
                
                if (response.isSuccessful) {
                    val authResponse = response.body()
                    authResponse?.let {
                        TokenManager.saveToken(it.token)
                        Toast.makeText(this@LoginActivity, "Успешная регистрация!", Toast.LENGTH_SHORT).show()
                        finish()
                    }
                } else {
                    showError("Ошибка регистрации")
                }
            } catch (e: Exception) {
                showError("Ошибка сети: ${e.message}")
            } finally {
                showLoading(false)
            }
        }
    }
    
    private fun showLoading(show: Boolean) {
        progressBar.visibility = if (show) View.VISIBLE else View.GONE
        buttonLogin.isEnabled = !show
        buttonRegister.isEnabled = !show
    }
    
    private fun showError(message: String) {
        Toast.makeText(this, message, Toast.LENGTH_LONG).show()
    }
    
    override fun onDestroy() {
        super.onDestroy()
        scope.cancel()
    }
}
