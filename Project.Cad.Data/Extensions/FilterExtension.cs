using MongoDB.Driver;
using System;
using System.Linq.Expressions;

namespace Project.Cad.Data.Extensions;

public static class FilterExtension
{
    public static IFindFluent<TFrom, TTo> Where<TFrom, TTo>(this IFindFluent<TFrom, TTo> i, Expression<Func<TFrom, bool>> filterExp) where TFrom : class
    {
        i.Filter &= Builders<TFrom>.Filter.Where(filterExp);
        return i;
    }
}
