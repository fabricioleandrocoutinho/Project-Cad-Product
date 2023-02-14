using Project.Cad.Data.Entities;
using Project.Cad.Data.Entities.Api.Collections;
using Project.Cad.Data.Exceptions;
using Project.Cad.Data.Interfaces.Repository;
using System.Threading.Tasks;

namespace Project.Cad.Domain.Services;

public class ProductService : CrudService<Product>
{
    public ProductService(ICrudRepository<Product> readerRepository) : base(readerRepository)
    {
    }

    public override Task<string> CreateAsync(Product entity)
    {
        ThrowIfParametersInvalid(entity, isUpdate: false);
        return base.CreateAsync(entity);
    }

    public override Task<bool> UpdateAsync(Product entity)
    {
        ThrowIfParametersInvalid(entity, isUpdate: true);
        return base.UpdateAsync(entity);
    }

    private void ThrowIfParametersInvalid(Product entity, bool isUpdate)
    {
        if (entity is null)
        {
            throw new ApiModelInvalidDataException($"Can not create a null {nameof(Product)}");
        }

        if (isUpdate && string.IsNullOrEmpty(entity.Id))
        {
            throw new ApiModelInvalidDataException($"{nameof(entity.Id)} can not be null or empty");
        }

        if (string.IsNullOrEmpty(entity.Name))
        {
            throw new ApiModelInvalidDataException($"{nameof(entity.Name)} can not be null or empty");
        }

        if (entity.SupplierId is null)
        {
            throw new ApiModelInvalidDataException($"{nameof(entity.SupplierId)} can not be null");
        }

        if (entity.SupplierCode is null)
        {
            throw new ApiModelInvalidDataException($"{nameof(entity.SupplierCode)} can not be null");
        }

        if (entity.ValidateDate is null)
        {
            throw new ApiModelInvalidDataException($"{nameof(entity.ValidateDate)} can not be null");
        }

        if (entity.ManufacturingDate is null)
        {
            throw new ApiModelInvalidDataException($"{nameof(entity.ManufacturingDate)} can not be null");
        }

        if (entity.ProductStatus.Equals(null))
        {
            throw new ApiModelInvalidDataException($"{nameof(entity.ProductStatus)} can not be null");
        }

        if (!entity.ProductStatus.Equals(StatusProduct.ACTIVE) &&
            !entity.ProductStatus.Equals(StatusProduct.INACTIVE) &&
            !entity.ProductStatus.Equals(StatusProduct.BLOCKED))
        {
            throw new ApiModelInvalidDataException($"{nameof(entity.ProductStatus)} Invalid value");
        }

        if (entity.Sku is null)
        {
            throw new ApiModelInvalidDataException($"{nameof(entity.Sku)} can not be null");
        }

        if (entity.ManufacturingDate > entity.ValidateDate)
        {
            throw new ApiModelInvalidDataException($"{nameof(entity.ManufacturingDate)} date must not be less than the validade date");
        }
    }
}
