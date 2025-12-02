namespace BlognaCorev2.Models;

public class PostQueryModel
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 5;
    public string SearchTerm { get; set; }
}