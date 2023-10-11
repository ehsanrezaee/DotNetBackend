using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace ErSoftDev.Common.Utilities
{
    public static class PagingExtensions
    {
        public static async Task<PagedResult<T>> GetPaged<T>(this IQueryable<T> query, int pageNumber, int pageSize, CancellationToken cancellationToken) where T : class
        {
            var pagedResult = new PagedResult<T>
            {
                PageIndex = pageNumber,
                PageSize = pageSize,
                RowCount = await query.CountAsync(cancellationToken)
            };
            var skip = (pageNumber) * pageSize;
            pagedResult.Rows = await query.Skip(skip).Take(pageSize).ToListAsync(cancellationToken);

            return pagedResult;
        }
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string orderBy, string orderType)
        {
            var expression = source.Expression;
            var parameter = Expression.Parameter(typeof(T), "x");
            var selector = Expression.PropertyOrField(parameter, orderBy);
            var method = string.Equals(orderType, "desc", StringComparison.OrdinalIgnoreCase) ?
                ("OrderByDescending") :
                ("OrderBy");
            expression = Expression.Call(typeof(Queryable), method,
                new[] { source.ElementType, selector.Type },
                expression, Expression.Quote(Expression.Lambda(selector, parameter)));

            return source.Provider.CreateQuery<T>(expression);
        }
    }



}
