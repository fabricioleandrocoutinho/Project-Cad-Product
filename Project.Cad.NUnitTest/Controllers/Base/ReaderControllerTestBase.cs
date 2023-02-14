using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using NUnit.Framework;
using Project.Cad.Data.DB;
using Project.Cad.Data.Interfaces.Repository;
using Project.Cad.Data.Models;
using Project.Cad.Domain.Interface;
using Project.Cad.Domain.Services;
using Project.Cad.NUnitTest.ReaderService;
using Project.Cad.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Project.Cad.NUnitTest.Controllers.Base;

[ExcludeFromCodeCoverage]
public abstract class ReaderControllerTestBase<T>
        where T : IMongoEntity, new()
{
    protected readonly MongoConnectionTest _connection;
    protected readonly IMongoCollection<T> _collection;
    protected readonly CrudRepository<T> _repository;
    protected readonly ICrudService<T> _service;
    protected readonly ReaderController<T> _controller;

    protected List<T> Itens { get; set; }

    protected ReaderControllerTestBase(Func<ICrudService<T>, ReaderController<T>> controllerFactor)
    {
        _connection = new MongoConnectionTest();
        _repository = new CrudRepository<T>(_connection);
        _collection = _connection.GetCollection<T>();
        _service = new CrudService<T>(_repository);
        _controller = controllerFactor(_service);
    }

    protected ReaderControllerTestBase(
        Func<ICrudRepository<T>, ICrudService<T>> serviceFactory,
        Func<ICrudService<T>, ReaderController<T>> controllerFactor)
    {
        _connection = new MongoConnectionTest();
        _repository = new CrudRepository<T>(_connection);
        _collection = _connection.GetCollection<T>();
        _service = serviceFactory(_repository);
        _controller = controllerFactor(_service);
    }

    [OneTimeSetUp]
    public async Task SetupInternal()
    {
        await DeleteAllAsync();
    }

    [OneTimeTearDown]
    public async Task DisposeAllInternal()
    {
        await DeleteAllAsync();
        _connection.Dispose();
    }

    protected async Task<T> GetByIdAsync(string id)
    {
        var actionResult = await _controller.GetByIdAsync(id);
        return (T)((OkObjectResult)actionResult.Result).Value;
    }

    protected async Task DeleteAllAsync()
    {
        await _collection.DeleteManyAsync(_ => true);
    }

    protected async Task<QueryResponse<T>> GetAsync(T entity = default, DefaultFilter filter = null)
    {
        var actionResult = await _controller.GetAsync(entity, filter);
        return (QueryResponse<T>)((OkObjectResult)actionResult.Result).Value;
    }

}

