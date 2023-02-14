using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;
using Project.Cad.Data.Models;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Project.Cad.Data.Interfaces.Repository;
using Project.Cad.Data.Attribute;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Project.Cad.NUnitTest")]
namespace Project.Cad.Data.DB
{
    public class CrudRepository<T> : ICrudRepository<T> where T : IMongoEntity, new()
    {
        internal readonly IMongoCollection<T> _collection;

        public CrudRepository(IMongoConnection mongoConnection)
        {
            var att = (CollectionAttribute)System.Attribute.GetCustomAttribute(typeof(T), typeof(CollectionAttribute));

            if (att == null)
            {
                _collection = mongoConnection.GetCollection<T>(ToSnakeCase(typeof(T).Name));
            }
            else
            {
                _collection = mongoConnection.GetCollection<T>(att.Name);
            }
        }

        public async Task<QueryResponse<T>> GetAsync(T entityFilter, DefaultFilter pageOrderFilter)
        {
            var filter = GenerateFilterOfEntity(entityFilter);
            var countTask = filter is null ? _collection.EstimatedDocumentCountAsync() : _collection.CountDocumentsAsync(filter);

            var query = GetFindQuery(filter)
                .Skip((pageOrderFilter.Page) * pageOrderFilter.Size)
                .Limit(pageOrderFilter.Size);

            var sort = GenerateSort(pageOrderFilter);

            if (sort != null)
            {
                query.Sort(sort);
            }

            var listTask = query.ToListAsync();

            await Task.WhenAll(listTask, countTask);

            var totalPages = (countTask.Result / pageOrderFilter.Size) - 1;

            if (totalPages < 0) totalPages = 0;

            return new QueryResponse<T>
            {
                CurrentPage = pageOrderFilter.Page,
                Data = listTask.Result,
                TotalItems = countTask.Result,
                TotalPages = totalPages
            };
        }

        private IFindFluent<T, T> GetFindQuery(FilterDefinition<T> filter)
        {
            if (filter is null)
            {
                return _collection.Find(_ => true);
            }
            return _collection.Find(filter);
        }

        public Task<T> GetByIdAsync(string id)
        {
            var idNameAttr = GetPropNameAttribute();

            if (HasObjectIdAttribute(GetIdPropInfo()))
            {
                return _collection.Find(Builders<T>.Filter.Eq(idNameAttr.ElementName, ObjectId.Parse(id))).FirstOrDefaultAsync();
            }
            return _collection.Find(Builders<T>.Filter.Eq(idNameAttr.ElementName, id)).FirstOrDefaultAsync();
        }

        private static PropertyInfo GetIdPropInfo()
        {
            return typeof(T).GetProperties().FirstOrDefault(p => System.Attribute.GetCustomAttribute(p, typeof(BsonIdAttribute)) != null);
        }

        private static BsonElementAttribute GetPropNameAttribute()
        {
            var idProp = GetIdPropInfo();
            return (BsonElementAttribute)System.Attribute.GetCustomAttribute(idProp, typeof(BsonElementAttribute));
        }

        private static bool HasObjectIdAttribute(PropertyInfo propInfo)
        {
            var bsonIdAtt = (BsonRepresentationAttribute)System.Attribute.GetCustomAttribute(propInfo, typeof(BsonRepresentationAttribute));
            return bsonIdAtt?.Representation == BsonType.ObjectId;
        }

        public async Task<string> CreateAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
            return entity.Id;
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            var result = await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq(e => e.Id, entity.Id), entity);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _collection.DeleteOneAsync(e => e.Id == id);
            return result.DeletedCount > 0;
        }

        internal static string ToSnakeCase(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var startUnderscores = Regex.Match(input, @"^_+");
            return startUnderscores + Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
        }

        internal static SortDefinition<T> GenerateSort(DefaultFilter pageOrderFilter)
        {
            if (string.IsNullOrEmpty(pageOrderFilter.Order))
            {
                return null;
            }

            var props = pageOrderFilter.Order.Split(",");
            SortDefinition<T> sortDefinition = null;
            foreach (var prop in props)
            {
                var (propName, sort) = GetPropSort(prop);
                if (sort == "desc")
                {
                    if (sortDefinition != null)
                    {
                        sortDefinition = sortDefinition.Descending((FieldDefinition<T>)propName);
                    }
                    else
                    {
                        sortDefinition = Builders<T>.Sort.Descending((FieldDefinition<T>)propName);
                    }
                }
                else
                {
                    if (sortDefinition != null)
                    {
                        sortDefinition = sortDefinition.Ascending((FieldDefinition<T>)propName);
                    }
                    else
                    {
                        sortDefinition = Builders<T>.Sort.Ascending((FieldDefinition<T>)propName);
                    }
                }
            }

            return sortDefinition;
        }

        internal static (string, string) GetPropSort(string prop)
        {
            var splitProp = prop.Trim().Split(" ");
            if (splitProp.Contains("asc"))
            {
                return (GetBsonName(splitProp[0]), "asc");
            }
            return (GetBsonName(splitProp[0]), "desc");
        }

        private static string GetBsonName(string propName)
        {
            var prop = typeof(T).GetProperties().FirstOrDefault(p => p.Name.ToLower() == propName.ToLower());

            if (prop is null)
            {
                throw new InvalidOperationException($"Property {propName} could not be found of entity {typeof(T).Name}");
            }

            var attr = (BsonElementAttribute)System.Attribute.GetCustomAttribute(prop, typeof(BsonElementAttribute));

            ThrowIfBsonAttributeNull(attr, prop.Name);

            return attr.ElementName;
        }

        private static void ThrowIfBsonAttributeNull(BsonElementAttribute attr, string propName)
        {
            if (attr is null)
            {
                throw new InvalidOperationException($"Could not find mongoDB name for property {propName} of entity {typeof(T).Name}");
            }
        }

        private static FilterDefinition<T> GenerateFilterOfEntity(T entityFilter)
        {
            if (entityFilter is null)
            {
                return null;
            }

            FilterDefinition<T> filter = null;
            AddFilterForObj(ref filter, entityFilter, null);
            return filter;
        }

        private static void AddFilterForObj(ref FilterDefinition<T> filter, object entity, string parentName)
        {
            var props = entity.GetType().GetProperties();

            foreach (var prop in props)
            {
                var value = prop.GetValue(entity);
                var attr = (BsonElementAttribute)System.Attribute.GetCustomAttribute(prop, typeof(BsonElementAttribute));
                if (value is not null)
                {
                    AppendPropsToFilter(ref filter, parentName, prop, value, attr);
                }
            }
        }

        private static void AppendPropsToFilter(ref FilterDefinition<T> filter, string parentName, PropertyInfo prop, object value, BsonElementAttribute attr)
        {
            ThrowIfBsonAttributeNull(attr, prop.Name);

            if (IsClass(value))
            {
                AddFilterForObj(ref filter, value, AppendNames(parentName, attr.ElementName));
                return;
            }

            if (value is DateTime date && date.Hour == 0 && date.Minute == 0 && date.Second == 0)
            {
                AppendIfNotNull(ref filter, Builders<T>.Filter.Gte(AppendNames(parentName, attr.ElementName), value));
                AppendIfNotNull(ref filter, Builders<T>.Filter.Lte(AppendNames(parentName, attr.ElementName), date.AddDays(1)));
                return;
            }

            if (HasObjectIdAttribute(prop))
            {
                value = ObjectId.Parse(value.ToString());
            }

            AppendFilter(ref filter, value, AppendNames(parentName, attr.ElementName));
        }

        private static string AppendNames(params string[] names)
        {
            var notNullNames = names.Where(n => !string.IsNullOrEmpty(n)).ToArray();

            if (notNullNames.Length == 1)
            {
                return notNullNames[0];
            }
            return string.Join('.', notNullNames);
        }

        private static bool IsClass(object prop)
        {
            var type = prop.GetType();
            return
                type != typeof(string) &&
                type.IsClass &&
                type != typeof(DateTime)
                && !typeof(IEnumerable).IsAssignableFrom(type);
        }

        private static void AppendFilter(ref FilterDefinition<T> filter, object value, string colName)
        {
            AppendIfNotNull(ref filter, Builders<T>.Filter.Eq(colName, value));
        }

        private static void AppendIfNotNull(ref FilterDefinition<T> filter, FilterDefinition<T> newFilter)
        {
            if (filter is null)
            {
                filter = newFilter;
                return;
            }
            filter &= newFilter;
        }
    }
}
