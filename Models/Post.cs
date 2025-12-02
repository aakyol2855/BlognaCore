using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace BlognaCorev2.Models;

public class Post
{
    public int Id { get; set; }
    [Required]
    public string Title { get; set; }=string.Empty;
    [Required]
    public string Content { get; set; }=string.Empty;
    [Required]
    public string Author { get; set; }="Anonymous";
    public DateTime CreatedAt { get; set; }=DateTime.Now;
    public DateTime UpdatedAt { get; set; }
    public bool Ispublished { get; set; } = false;
    public string UserId { get; set; } = String.Empty;
    
}