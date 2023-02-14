using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.Cad.NUnitTest.Utils;

internal static class TestUtils
{
    internal static async Task<List<T>> CreateItensAsync<T>(int count, IMongoCollection<T> collection, Func<int, T> generator)
    {
        var itens = new List<T>();
        for (var i = 0; i < count; i++)
        {
            itens.Add(generator(i));
        }

        await collection.InsertManyAsync(itens);
        return itens;
    }
}
