using Project.Cad.Data.DB;
using Project.Cad.Data.Exceptions;
using Project.Cad.Data.Interfaces.Repository;
using Project.Cad.Data.Models;
using Project.Cad.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Cad.Domain.Services
{
    public class CrudService<T> : ICrudService<T> where T : IMongoEntity, new()
    {
        private readonly ICrudRepository<T> _readerRepository;
        public CrudService(ICrudRepository<T> readerRepository)
        {
            _readerRepository = readerRepository;
        }
        public virtual async Task<QueryResponse<T>> GetAsync(T entityFilter, DefaultFilter pageOrderFilter)
        {
            if (pageOrderFilter is null)
            {
                pageOrderFilter = new DefaultFilter();
            }
            return await _readerRepository.GetAsync(entityFilter, pageOrderFilter);
        }
        public virtual async Task<T> GetByIdAsync(string id)
        {
            ThrowIfIdNullOrEmpty(id);
            return await _readerRepository.GetByIdAsync(id);
        }
        public virtual async Task<string> CreateAsync(T entity)
        {
            return await _readerRepository.CreateAsync(entity);
        }
        public virtual async Task<bool> DeleteAsync(string id)
        {
            ThrowIfIdNullOrEmpty(id);
            return await _readerRepository.DeleteAsync(id);
        }
        public virtual async Task<bool> UpdateAsync(T entity)
        {
            return await _readerRepository.UpdateAsync(entity);
        }
        private static void ThrowIfIdNullOrEmpty(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ApiModelInvalidDataException("Id can not be empty or null");
        }
    }
}