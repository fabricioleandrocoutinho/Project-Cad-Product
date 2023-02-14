using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;
using Project.Cad.Data.Entities.Api.Collections;
using Project.Cad.Data.Models.Request;
using Project.Cad.Domain.Services;
using Project.Cad.NUnitTest.Controllers.Base;
using Project.Cad.NUnitTest.Utils;
using Project.Cad.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using DefaultFilter = Project.Cad.Data.Models.DefaultFilter;

namespace Project.Cad.NUnitTest.Controllers;


[ExcludeFromCodeCoverage]
public class ProductControllerTest : CrudControllerTestBase<Product, ProductRequest>
{
    public ProductControllerTest() : base(
        (repository) => new ProductService(repository),
        (service) => new ProductController(service))
    {
    }

    [OneTimeSetUp]
    public async Task Setup()
    {
        Itens = await CreateItensAsync(2);
    }

    [Test]
    public async Task GetAll_Return_All()
    {
        await DeleteAllAsync();
        Itens = await CreateItensAsync(2);
        var results = await GetAsync(new Product(), new DefaultFilter());
        Assert.AreEqual(2, results.Data.Count());
    }

    [Test]
    public async Task Post_ShouldCreate()
    {
        await TestCreationAsync(new ProductRequest
        {
            SupplierName = "teste",
            SupplierCode = 1,
            Sku = "123456789",
            ProductStatus = Data.Entities.StatusProduct.ACTIVE,
            ManufacturingDate = DateTime.UtcNow,
            ValidateDate = DateTime.UtcNow,
            Name = "product teste",
            SupplierId = "1A"
        });
    }

    [Test]
    public async Task Put_ShouldUpdate()
    {
        await TestUpdateAsync(new ProductRequest
        {
            SupplierCode = 1,
            SupplierName = "teste",
            Sku = "unity",
            Name = "ttt",
            SupplierId = "1",
            ProductStatus = Data.Entities.StatusProduct.INACTIVE,
            ManufacturingDate = DateTime.UtcNow,
            ValidateDate = DateTime.UtcNow
        }, Itens[0].Id);
    }

    [Test]
    public async Task Post_Return_BadRequest_Null_Entity()
    {
        await TestBadRequestForCreateAsync(null);
    }
   
    private async Task TestBadRequestForUpdateAsync(ProductRequest entity, string id)
    {
        var actionResult = await CrudController.UpdateAsync(entity, id);
        Assert.IsInstanceOf<ObjectResult>(actionResult.Result);
    }

    private async Task TestBadRequestForCreateAsync(ProductRequest entity)
    {
        var actionResult = await CrudController.CreateAsync(entity);
        Assert.IsInstanceOf<ObjectResult>(actionResult.Result);
    }

    [Test]
    public async Task Delete_ShouldDelete()
    {
        var createdItens = await CreateItensAsync(1);
        await TestDeleteAsync(createdItens[0].Id);
        var gotEntity = await GetByIdAsync(createdItens[0].Id);
        Assert.Null(gotEntity.Id);
    }

    [Test]
    public async Task Invalid_Id_Delete_Should_Not_Delete()
    {
        var createdItens = await CreateItensAsync(1);
        await CrudController.DeleteByIdAsync("12312");
        var gotEntity = await GetByIdAsync(createdItens[0].Id);
        Assert.NotNull(gotEntity.Id);
    }

    [Test]
    public async Task Delete_Should_Not_Delete()
    {
        var createdItens = await CreateItensAsync(1);
        await CrudController.DeleteByIdAsync(ObjectId.GenerateNewId().ToString());
        var gotEntity = await GetByIdAsync(createdItens[0].Id);
        Assert.NotNull(gotEntity.Id);
    }

    [Test]
    public async Task GetAll_Return_Single()
    {
        var visit = await GetByIdAsync(Itens[0].Id);
        Assert.NotNull(visit);
        Assert.AreEqual(Itens[0].Id, visit.Id);
    }

    private async Task<List<Product>> CreateItensAsync(int count)
    {
        return await TestUtils.CreateItensAsync(count, _collection, i =>
        {
            return new Product
            {
                Id = ObjectId.GenerateNewId().ToString(),
                SupplierCode = i,
                Name = Guid.NewGuid().ToString(),
                Sku = i.ToString(),
                SupplierId = i.ToString()
            };
        });
    }
}
