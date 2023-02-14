
namespace Project.Cad.Data.Interfaces.Repository
{
    //
    // Resumo:
    //     Defines a entity that can be search and insert into mongodb
    public interface IMongoEntity
    {
        //
        // Resumo:
        //     Primary key of the entity
        string Id { get; set; }
    }
}
