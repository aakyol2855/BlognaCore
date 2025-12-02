using System.Collections.Generic;

namespace BlognaCorev2.Models;

public class PageListViewModel<T>
{
    public List<T> Items { get; set; }
    public int TotalItems { get; set; }
    public int PageSize { get; set; }
    public int CurrentPage { get; set; }
    public string Query { get; set; } 
    public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
    public List<int> AvailableSizes { get; set; } = new List<int> { 5,15, 20, 25 }; 
    public bool  HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages ;
    public bool HasPage => TotalPages > 0;
    
    public PageListViewModel(List<T> Items , int totalItems , int pageSize , int currentPage,string query)
    {
        this.Items = Items;
        this.TotalItems = totalItems;
        this.PageSize =  pageSize;
        this.CurrentPage = currentPage;
        this.Query = query;
    }
}