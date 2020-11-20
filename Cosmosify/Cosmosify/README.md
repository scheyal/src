# Cosmosify
CRUD operations interface to Cosmos DB

## Interface
The inteface is a thin shell over CosmosDB documents CRUD operations. See code for details.

## Usage
The program is a rudimentary CLI test over the API.
Usage is very basic:
```
Usage:
Cosmosify -k key -u url -d db -c coll -i id -o opType (GET|POST|DELETE|REPLACE)
```
Where
* key: Master Cosmos DB key
* Url: Base URL for cosmos db. Example: https://dbname.documents.azure.com:443
* db: Database Id
* coll: Collection Id
* opType: one of CRUD operations (GET|POST|DELETE|REPLACE)

To query for a list of all docs in db/collection, specify -o GET with -i "".
