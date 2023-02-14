using Microsoft.AspNetCore.Mvc;
using Project.Cad.Data.Entities.Api.Collections;
using Project.Cad.Data.Models.Request;
using Project.Cad.Domain.Interface;

namespace Project.Cad.Web.Controllers
{
    [ApiController]
    public class  ProductController : CrudController<Product, ProductRequest>
    {
        public ProductController(ICrudService<Product> crudService) : base(crudService)
        {
        }
    }
}
