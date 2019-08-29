# Handyman.Azure.Cosmos.Table

This package provides helper types and methods to simplify working with [Azure Table Storage](https://azure.microsoft.com/en-in/services/storage/tables/) or [Azure Cosmos DB Table API](https://docs.microsoft.com/en-us/azure/cosmos-db/table-introduction).

Some of the things provided by this package are

* [CloudTableClientExtensions](#CloudTableClientExtensions)
* [CloudTableExtensions](#CloudTableExtensions)
* [TableResultExtensions](#TableResultExtensions)
* [TableQueryBuilder](#TableQueryBuilder)
* [TableBatchOperationBuilder](#TableBatchOperationBuilder)
* [TableBatchResultExtensions](#TableBatchResultExtensions)

## CloudTableClientExtensions

Pretty much all data access in table storage goes through the CloudTable class. If you are building a soultion where you have data in multiple tables and using dependency injection it's nice to have one class per table, since that makes it really easy to program against. Just take the right class as a dependency and you are good to go.  
Unfortuantelly this is easier said than done but `CloudTableClientExtensions.GetTableReference<T>(tableName)` takes care of the messy parts of formatting storage uris etc.

## CloudTableExtensions

Working with the table storage api is pretty straight forwardbut requires a bit of writing. All the basic data access operations requires you to create a `TableOperation` first and then execute it.  

``` csharp
var operation = TableOperations.Insert(entity);
await table.ExecuteAsync(operation);

```

This has been somewhat simplified. There are extensions methods for all operations following this naming pattern `table.{operation}EntityAsync`.

``` csharp
await table.InsertEntityAsync(entity);
```

## TableResult

Someof the design decision for the `TableResult` class makes it a little harder to work with than it needs to be.  
All extension methods for operations on `CloudTable` return a customized result class where the entity property is strongly typed to the entity type in stead of being of type object. Other helper methods are:

* `void EnsureSuccessStatusCode()` - throws if status code >= 400
* `bool HasSuccessStatusCode()`
* `T GetEntityOrThrow()` - throws if not success status code
* `bool TryGetEntity(out T entity)` - true if success status code, else false & entity = null

## TableQueryBuilder

Trying to compose queries using the primitives provided by microsoft requieres you to do quite a bit of writing.  

The query builder provides a strongly typed api for composing queries and also has support for string starts with filters.

This sample is querying entities where PartitionKey equals `foo`, Number equals `0` or is greater than `5` and we can expect more than 1000 entities found.

``` csharp
var partitionKeyFilter = TableQuery.GenerateFilterCondition(nameof(MyEntity.PartitionKey), QueryComparisons.Equal, "foo");

var numberFilterA = TableQuery.GenerateFilterConditionForInt(nameof(MyEntity.Number), QueryComparisons.Equal, 0);
var numberFilterB = TableQuery.GenerateFilterConditionForInt(nameof(MyEntity.Number), QueryComparisons.GreaterThan, 5);
var numberFilter = TableQuery.CombineFilters(numberFilterA, TableOperators.Or, numberFilterB);

var filter = TableQuery.CombineFilters(partitionKeyFilter, TableOperators.And, numberFilter);

var query = new TableQuery<MyEntity>().Where(filter);

TableContinuationToken continuationToken = null;

var entities = new List<MyEntity>();

do
{
    var segment = await table.ExecuteQuerySegmentedAsync(query, continuationToken);
    entities.AddRange(segment);
    continuationToken = segment.ContinuationToken;
} while (continuationToken != null);

foreach (var entity in entities)
{
    // do work here
}
```

Writing the same query using this package looks like this.

``` csharp
var query = new TableQueryBuilder<MyEntity>()
    .Where(where => where.And(and =>
    {
        and.PartitionKey.Equals("foo");
        and.Or(or =>
        {
            or.Property(x => x.Number).Equals(0);
            or.Property(x => x.Number).GreaterThan(5);
        });
    }))
    .Build();

var entities = await table.QueryEntitiesAsync(query);

foreach (var entity in entities)
{
    // do work here
}
```

## TableBatchOperationBuilder

A batch has a upper limit of 100 operations, with this builder you can add as many opeations as you wich and then get a collection of batch operations.

``` csharp
var batches = new TableBatchOperationBuilder()
    .Delete(entity1)
    .Insert(entity2)
    ...
    .Replace(entity999)
    .Build();

foreach (var batch in batches)
{
    await table.ExecuateBatchAsync(batch);
}
```

## TableBatchResultExtensions

Similar to TableResult
