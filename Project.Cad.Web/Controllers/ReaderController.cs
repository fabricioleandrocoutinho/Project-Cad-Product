using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.Cad.Data.DB;
using Project.Cad.Data.Exceptions;
using Project.Cad.Data.Models;
using Project.Cad.Domain.Interface;
using System.Threading.Tasks;
using System;
using Project.Cad.Data.Interfaces.Repository;

namespace Project.Cad.Web.Controllers
{
    [ApiController]
    [Route("api")]
    public abstract class ReaderController<T> : ControllerBase where T : IMongoEntity, new()
    {
        protected ICrudService<T> _readService;

        protected ReaderController(ICrudService<T> readService)
        {
            _readService = readService;
        }

        [Produces("application/json")]
        [HttpGet("[controller]:paginate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(type: typeof(QueryResponse<>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiExceptionResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiExceptionResponse), StatusCodes.Status500InternalServerError)]
        public virtual async Task<ActionResult<QueryResponse<T>>> GetAsync([FromQuery] T entityFilter, [FromQuery] DefaultFilter pageOrderFilter)
        {
            return await TryRunAsync(() => _readService.GetAsync(entityFilter, pageOrderFilter));
        }

        [HttpGet("[controller]/{id}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiExceptionResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiExceptionResponse), StatusCodes.Status500InternalServerError)]
        public virtual async Task<ActionResult<T>> GetByIdAsync(string id)
        {
            return await TryRunAsync(() => _readService.GetByIdAsync(id));
        }

        protected async Task<ActionResult> TryRunDeleteAsync(Func<Task<bool>> action, string id)
        {
            try
            {
                var isDeleted = await action();
                if (isDeleted) return Ok();
                return BadRequest($"No {nameof(T)} was found with id: {id}");
            }
            catch (Exception ex)
            {
                return CatchError(ex);
            }
        }

        protected async Task<ActionResult> TryRunPostAsync(Func<Task<string>> action)
        {
            try
            {
                var id = await action();
                return Created(BuildCreatedUrl(id), null);
            }
            catch (Exception ex)
            {
                return CatchError(ex);
            }
        }

        protected async Task<ActionResult<T>> TryRunPutAsync(Func<Task<bool>> action, T entity)
        {
            try
            {
                var updated = await action();

                if (updated)
                {
                    return Ok(entity);
                }
                return BadRequest($"No {nameof(T)} was found with id: {entity.Id}");
            }
            catch (Exception ex)
            {
                return CatchError(ex);
            }
        }

        protected async Task<ActionResult<U>> TryRunAsync<U>(Func<Task<U>> action)
        {
            try
            {
                var result = await action();

                if (result is not null)
                {
                    return Ok(result);
                }
                return Ok(new T());
            }
            catch (Exception ex)
            {
                return CatchError(ex);
            }
        }

        private string BuildCreatedUrl(string id)
        {
            var url = "http";

            if (Request is not null && Request.IsHttps)
            {
                url += "s";
            }

            url += ":/";

            return string.Join('/', url, Request?.Host.Value, Request?.Path.Value[1..], id);
        }

        private ActionResult CatchError(Exception ex)
        {
            var response = new ApiExceptionResponse
            {
                Type = ex.GetType().Name,
                Detail = ex.Message,
                Error = ex.Message,
                TraceId = HttpContext?.TraceIdentifier
            };

            if (ex is ApiModelInvalidDataException)
            {
                return BadRequest(response);
            }

            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }
}