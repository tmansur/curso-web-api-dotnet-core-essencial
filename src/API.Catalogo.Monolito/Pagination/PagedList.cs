namespace API.Catalogo.Pagination
{
  public class PagedList<T> : List<T> where T : class
  {
    public int CurrentPage { get; private set; }
    public int TotalPages { get; private set; }
    public int PageSize { get; private set; }
    /// <summary>
    /// Total de elementos existentes
    /// </summary>
    public int TotalCount { get; private set; }
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;

    public PagedList(List<T> items, int count, int pageNumber, int pageSize)
    {
      TotalCount = count;
      PageSize = pageSize;
      CurrentPage = pageNumber;
      TotalPages = (int)Math.Ceiling(count / (double)pageSize);

      AddRange(items);
    }

    public static PagedList<T> ToPagedList(IQueryable<T> source, int pageNumber, int pageSize)
    {
      var count = source.Count(); //faz count no total de elementos existente no BD
      var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(); //busca elementos da página atual

      return new PagedList<T>(items, count, pageNumber, pageSize);  
    }
  }
}
