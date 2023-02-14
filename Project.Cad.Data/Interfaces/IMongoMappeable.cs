using Project.Cad.Data.Interfaces.Repository;

namespace Project.Cad.Data.Interfaces
{
    public interface IMongoMappeable<out T> where T : IMongoEntity
    {
        T Map(string id = null);
    }
}