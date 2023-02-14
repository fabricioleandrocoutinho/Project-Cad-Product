using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using NUnit.Framework;
using Project.Cad.Data.Interfaces;
using Project.Cad.Data.Interfaces.Repository;
using Project.Cad.Domain.Interface;
using Project.Cad.Web.Controllers;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Project.Cad.NUnitTest.Controllers.Base;

[ExcludeFromCodeCoverage]
public abstract class CrudControllerTestBase<T, TUpdateEntity> : ReaderControllerTestBase<T>
    where T : IMongoEntity, new()
    where TUpdateEntity : IMongoMappeable<T>
{
    protected CrudController<T, TUpdateEntity> CrudController { get => (CrudController<T, TUpdateEntity>)_controller; }

    protected CrudControllerTestBase(Func<ICrudService<T>, CrudController<T, TUpdateEntity>> controllerFactory) : base(controllerFactory)
    {
    }

    protected CrudControllerTestBase(
        Func<ICrudRepository<T>, ICrudService<T>> serviceFactory,
        Func<ICrudService<T>, CrudController<T, TUpdateEntity>> controllerFactory) : base(serviceFactory, controllerFactory)
    {
    }

    protected async Task TestCreationAsync(TUpdateEntity entity)
    {
        var sizeBefore = await _collection.CountDocumentsAsync(new BsonDocument());
        var actionResult = await CrudController.CreateAsync(entity);
        var sizeAfter = await _collection.CountDocumentsAsync(new BsonDocument());

        Assert.Greater(sizeAfter, sizeBefore);
        Assert.IsInstanceOf<CreatedResult>(actionResult.Result);

        var createResult = (CreatedResult)actionResult.Result;
        Assert.NotNull(createResult.Location);
        Assert.Null(createResult.Value);
    }

    protected async Task<T> TestUpdateAsync(TUpdateEntity entity, string id)
    {
        var actionResult = await CrudController.UpdateAsync(entity, id);
        Assert.IsInstanceOf<OkObjectResult>(actionResult.Result);

        var updateResult = (OkObjectResult)actionResult.Result;
        Assert.NotNull(updateResult.Value);
        Assert.IsInstanceOf<T>(updateResult.Value);

        var tValue = (T)updateResult.Value;
        Assert.NotNull(tValue.Id);
        return tValue;
    }

    protected async Task TestDeleteAsync(string id)
    {
        var actionResult = await CrudController.DeleteByIdAsync(id);
        Assert.IsInstanceOf<OkResult>(actionResult);
    }
}