using System.ComponentModel;

namespace ErSoftDev.Common.Utilities
{
    public abstract class PagedResultBase
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int RowCount { get; set; }
        public int PageCount
        {
            get => (int)Math.Ceiling((double)RowCount / PageSize);
            set { }
        }

        public int FirstRowOnPage
        {
            get => (PageIndex) * PageSize + 1;
            set { }
        }

        public int LastRowOnPage
        {
            get => Math.Min((PageIndex + 1) * PageSize, RowCount);
            set { }
        }
    }

    public class PagedResult<T> : PagedResultBase where T : class
    {
        public IList<T> Rows { get; set; }

        public PagedResult()
        {
            Rows = new List<T>();
        }
    }

    public class PagingRequest
    {
        public int PageSize { get; set; } = 10;
        public int PageNumber { get; set; } = 0;
        public string OrderBy { get; set; }
        public OrderType OrderType { get; set; } = OrderType.Asc;
    }

    public enum OrderType
    {
        [Description("Desc")]
        Desc,
        [Description("Asc")]
        Asc
    }
}
