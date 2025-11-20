
namespace parsing_Jrn_Ej.response
{
    public class PaginatedResponse<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalData { get; set; }
        public int TotalPages { get; set; }
        public List<T>? Data { get; set; }
    }

}