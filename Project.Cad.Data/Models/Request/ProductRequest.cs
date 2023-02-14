using MongoDB.Bson.Serialization.Attributes;
using Project.Cad.Data.Entities;
using Project.Cad.Data.Entities.Api.Collections;
using Project.Cad.Data.Interfaces;
using System;
using System.Text.Json.Serialization;

namespace Project.Cad.Data.Models.Request
{

    public class ProductRequest : IMongoMappeable<Product>
    {
        [JsonPropertyName("sku")]
        public string Sku { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("supplierCode")]
        public int? SupplierCode { get; set; }

        [JsonPropertyName("supplierName")]
        public string SupplierName { get; set; }

        [JsonPropertyName("supplierId")]
        public string SupplierId { get; set; }

        [JsonPropertyName("productStatus")]
        public StatusProduct ProductStatus { get; set; } 

        [JsonPropertyName("manufacturingDate")]
        public DateTime? ManufacturingDate { get; set; }

        [JsonPropertyName("validateDate")]
        public DateTime? ValidateDate { get; set; }

        public Product Map(string id = null)
        {
            return new Product
            {
                Id = id,
                SupplierCode = SupplierCode,
                SupplierName = SupplierName,
                Name = Name,
                Sku = Sku,
                SupplierId = SupplierId,
                ProductStatus = ProductStatus,
                ManufacturingDate = ManufacturingDate,
                ValidateDate = ValidateDate

            };
        }
    }
}
