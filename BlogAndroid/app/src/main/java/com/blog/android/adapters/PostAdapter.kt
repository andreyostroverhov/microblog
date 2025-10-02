package com.blog.android.adapters

import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ImageView
import android.widget.TextView
import androidx.recyclerview.widget.RecyclerView
import com.blog.android.R
import com.blog.android.models.Post
import com.bumptech.glide.Glide
import java.text.SimpleDateFormat
import java.util.*

class PostAdapter(
    private val posts: MutableList<Post>,
    private val onLikeClick: (Post) -> Unit
) : RecyclerView.Adapter<PostAdapter.PostViewHolder>() {

    class PostViewHolder(itemView: View) : RecyclerView.ViewHolder(itemView) {
        val textViewTitle: TextView = itemView.findViewById(R.id.textViewTitle)
        val textViewAuthor: TextView = itemView.findViewById(R.id.textViewAuthor)
        val textViewContent: TextView = itemView.findViewById(R.id.textViewContent)
        val textViewDate: TextView = itemView.findViewById(R.id.textViewDate)
        val imageViewPost: ImageView = itemView.findViewById(R.id.imageViewPost)
        val buttonLike: com.google.android.material.button.MaterialButton = itemView.findViewById(R.id.buttonLike)
    }

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): PostViewHolder {
        val view = LayoutInflater.from(parent.context)
            .inflate(R.layout.item_post, parent, false)
        return PostViewHolder(view)
    }

    override fun onBindViewHolder(holder: PostViewHolder, position: Int) {
        val post = posts[position]
        
        holder.textViewTitle.text = post.title
        holder.textViewAuthor.text = "Автор: ${post.author.username}"
        holder.textViewContent.text = post.content
        holder.buttonLike.text = "❤️ ${post.likesCount}"
        
        // Format date
        val dateFormat = SimpleDateFormat("dd.MM.yyyy HH:mm", Locale.getDefault())
        val date = try {
            SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss", Locale.getDefault()).parse(post.createdAt)
        } catch (e: Exception) {
            Date()
        }
        holder.textViewDate.text = dateFormat.format(date ?: Date())
        
        // Load image if available
        if (!post.imageUrl.isNullOrEmpty()) {
            holder.imageViewPost.visibility = View.VISIBLE
            Glide.with(holder.itemView.context)
                .load(post.imageUrl)
                .placeholder(R.drawable.ic_launcher_foreground)
                .error(R.drawable.ic_launcher_foreground)
                .into(holder.imageViewPost)
        } else {
            holder.imageViewPost.visibility = View.GONE
        }
        
        // Set like button state
        holder.buttonLike.isSelected = post.isLikedByCurrentUser
        
        holder.buttonLike.setOnClickListener {
            onLikeClick(post)
        }
    }

    override fun getItemCount(): Int = posts.size

    fun updatePosts(newPosts: List<Post>) {
        posts.clear()
        posts.addAll(newPosts)
        notifyDataSetChanged()
    }

    fun updatePost(updatedPost: Post) {
        val index = posts.indexOfFirst { it.id == updatedPost.id }
        if (index != -1) {
            posts[index] = updatedPost
            notifyItemChanged(index)
        }
    }
}
