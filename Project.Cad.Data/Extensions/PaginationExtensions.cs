using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Project.Cad.Data.Extensions;

[ExcludeFromCodeCoverage]
public static class PaginationExtensions
{
    public static IFindFluent<TFrom, TTo> Pagination<TFrom, TTo>(this IFindFluent<TFrom, TTo> i, int page, int limit) where TFrom : class
    {
        return i.Skip((page - 1) * limit).Limit(limit);
    }

    public static IMongoQueryable<T> Pagination<T>(this IMongoQueryable<T> i, int page, int limit) where T : class
    {
        return i.Skip((page - 1) * limit).Take(limit);
    }
}
