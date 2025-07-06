using Microsoft.Azure.Cosmos;

namespace DAYA.Cloud.Framework.V2.Cosmos.Migration;

public static class ContainerUtility
{
    public static async Task<T[]> ToListAsync<T>(this Container container, string sql)
    {
        var queryDefinition = new QueryDefinition(sql);
        return await ToListAsync<T>(container, queryDefinition);
    }

    public static async Task<T[]> ToListAsync<T>(this Container container, QueryDefinition query)
    {
        var queryResult = container.GetItemQueryIterator<T>(query);

        var items = new List<T>();
        while (queryResult.HasMoreResults)
        {
            var currentResultSet = await queryResult.ReadNextAsync();
            foreach (var item in currentResultSet)
            {
                items.Add(item);
            }
        }

        return items.ToArray();
    }

    public static async Task<T> ToQuerySingleAsync<T>(this Container container, string sql)
    {
        return await ToQuerySingleAsync<T>(container, new QueryDefinition(sql));
    }

    public static async Task<T> ToQuerySingleAsync<T>(this Container container, QueryDefinition queryDefinition)
    {
        var results = container.GetItemQueryIterator<T>(queryDefinition);

        var item = await results.ReadNextAsync();

        return item.Resource.FirstOrDefault();
    }
}
