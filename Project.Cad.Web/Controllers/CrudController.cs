using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.Cad.Data.Exceptions;
using Project.Cad.Data.Interfaces;
using Project.Cad.Data.Interfaces.Repository;
using Project.Cad.Domain.Interface;
using System.Threading.Tasks;

namespace Project.Cad.Web.Controllers
{
    [ApiController]
    [Route("api")]
    public abstract class CrudController<T, TUpdateEntity> : ReaderController<T>
        where T : IMongoEntity, new()
        where TUpdateEntity : IMongoMappeable<T>
    {
        protected CrudController(ICrudService<T> readService) : base(readService)
        {
        }

        [HttpPost("[controller]")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiExceptionResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiExceptionResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<T>> CreateAsync([FromBody] TUpdateEntity entityPost)
        {
            T entity = default;
            if (entityPost is not null)
            {
                entity = entityPost.Map(null);
            }
            return await TryRunPostAsync(() => _readService.CreateAsync(entity));
        }

        [HttpPut("[controller]/{id}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiExceptionResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiExceptionResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<T>> UpdateAsync([FromBody] TUpdateEntity entityPost, string id)
        {
            T entity = default;
            if (entityPost is not null)
            {
                entity = entityPost.Map(id);
            }
            return await TryRunPutAsync(() => _readService.UpdateAsync(entity), entity);
        }

        [HttpDelete("[controller]/{id}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiExceptionResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiExceptionResponse), StatusCodes.Status500InternalServerError)]
        public virtual async Task<ActionResult> DeleteByIdAsync(string id)
        {
            return await TryRunDeleteAsync(() => _readService.DeleteAsync(id), id);
        }
    }
}
