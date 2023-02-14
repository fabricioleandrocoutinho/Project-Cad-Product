using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Project.Cad.Data.Attribute;
using Project.Cad.Data.Interfaces.Repository;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Project.Cad.Data.Entities.Api.Collections;

[ExcludeFromCodeCoverage]
[Collection("product")]
public class Product : IMongoEntity
{
    [BsonId]
    [BsonElement("_id")]
    [JsonPropertyName("id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("sku")]
    [JsonPropertyName("sku")]
    public string Sku { get; set; }

    [BsonElement("name")]
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [BsonElement("supplier_code")]
    [JsonPropertyName("supplierCode")]
    public int? SupplierCode { get; set; }

    [BsonElement("supplier_name")]
    [JsonPropertyName("supplierName")]
    public string SupplierName { get; set; }

    [BsonElement("supplier_id")]
    [JsonPropertyName("supplierId")]
    public string SupplierId { get; set; }

    [BsonElement("product_status")]
    [JsonPropertyName("productStatus")]
    public StatusProduct ProductStatus { get; set; } 

    [BsonElement("manufacturing_date")]
    [JsonPropertyName("manufacturingDate")]
    public DateTime? ManufacturingDate { get; set; }

    [BsonElement("validate_date")]
    [JsonPropertyName("validateDate")]
    public DateTime? ValidateDate { get; set; }
}
