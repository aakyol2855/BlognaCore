namespace BlognaCorev2.Models
{
    public class PostViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string ContentSnippet { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsPublished { get; set; }
    }
}