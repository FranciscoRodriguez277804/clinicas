namespace ClinicaEspacioAbiertoFrontEnd.Models.Entidades.ValueObjects
{
    public class PaginacionViewModel<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages;
        public IEnumerable<T> Items { get; set; }
    }

}
