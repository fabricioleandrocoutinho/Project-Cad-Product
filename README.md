# Project-Cad-Product
**Name**:Project Cad Product
**Type**: API

# Introdution

This project is responsible for registering the products. it has a swagger so that the user can register the products

# Video
  Presentation video is in the root folder, named Video - Presentation of Projecto.rar

## Route
{odin-base-url}/swagger/index.html

## Business Rule

The microservice only performs the queries/update and delete in the collection from the swagger and performs resync when executed by the user. There is no kind of schedule to perform any task, so n.

Response Formats : JSON

## EndPoint collection query

all microservice collections have query by {id} and {paginate} of the records, below endpoint

example:

```
/{collection name}: paginate
/{collection name}: {id}
```
# collection  list:

| collection | GET | PUT | DELETE | POST |
|--|--|--|--|--|
| Product | x | x | x | x |



## Getting Started

To perform the project settings independent of the environment, it is necessary to inform the variables below according to the environment that was tested. Some dependencies are not stored in `Nuget`. It may be on `nexus` or `Azure artifacts`. If it is the case, check our wiki to know how to allow downloads from `Nexus` or `Azure Artifacts`.


# MongoDB Connection
|Variable | Description |
|--|--|
|DATABASE_NAME | > Name of collection in database |
|DATABASE_CONNECTIONSTRING | > database connection string |

# Other Settings
|Variable | Description |
|--|--|
UTC_OFFSET | > time zone |

# Technologies

- ASP.NET 
- .NET 6.0
- MongoDB
- NUnit
