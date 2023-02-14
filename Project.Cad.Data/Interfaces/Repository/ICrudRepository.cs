using Project.Cad.Data.DB;
using Project.Cad.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Cad.Data.Interfaces.Repository
{
    public interface ICrudRepository<T> where T : IMongoEntity, new()
    {
        Task<T> GetByIdAsync(string id);
        Task<QueryResponse<T>> GetAsync(T entityFilter, DefaultFilter pageOrderFilter);
        Task<string> CreateAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(string id);
    }
}
