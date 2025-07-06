using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAYA.Cloud.Framework.V2.Application.Contracts;
using DAYA.Cloud.Framework.V2.Infrastructure.Configuration;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DAYA.Cloud.Framework.V2.Cosmos.Abstractions;

public static class ContainerUtility
{
    public static async Task<T[]> AsListAsync<T>(this Container container, string sql)
    {
        var queryDefinition = new QueryDefinition(sql);
        return await AsListAsync<T>(container, queryDefinition);
    }

    public static async Task<T[]> AsListAsync<T>(this Container container, QueryDefinition query)
    {
        using var scope = ServiceCompositionRoot.CreateScope();
        var logger = scope.ServiceProvider.GetService<ILogger<T>>();

        logger.LogInformation("Executing Query on DB:");
        logger.LogInformation(query.QueryText);
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

    public static async Task<PagedDto<T>> AsPagedAsync<T>(this Container container,
        PageableQuery<T> request,
        string sql,
        string fromWhere)
    {
        return await AsPagedAsync(container,
            request,
            sql,
            new QueryDefinition(fromWhere));
    }

    public static async Task<PagedDto<T>> AsPagedAsync<T>(this Container container,
        PageableQuery<T> request,
        QueryDefinition queryDefinition,
        string fromWhere)
    {
        var pagableQueryDefinition = new QueryDefinition(queryDefinition.QueryText + $" OFFSET {request.PageSize * (request.PageNumber - 1)} LIMIT {request.PageSize}");
        var @params = queryDefinition.GetQueryParameters();
        foreach (var queryParameter in @params)
        {
            pagableQueryDefinition = pagableQueryDefinition.WithParameter(queryParameter.Name, queryParameter.Value);
        }

        using var scope = ServiceCompositionRoot.CreateScope();
        var logger = scope.ServiceProvider.GetService<ILogger<T>>();

        logger.LogInformation("Executing Query on DB:");
        logger.LogInformation(queryDefinition.QueryText);
        var queryResultSetIterator = container.GetItemQueryIterator<T>(pagableQueryDefinition);

        var items = new List<T>();
        while (queryResultSetIterator.HasMoreResults)
        {
            var currentResultSet = await queryResultSetIterator.ReadNextAsync();
            foreach (var item in currentResultSet)
            {
                items.Add(item);
            }
        }

        var totalItemsSql = $"SELECT VALUE COUNT(1) {fromWhere}";
        var totalItemsQuery = new QueryDefinition(totalItemsSql);
        foreach (var queryParameter in @params)
        {
            totalItemsQuery = totalItemsQuery.WithParameter(queryParameter.Name, queryParameter.Value);
        }

        var pagedDto = new PagedDto<T>
        {
            Items = items.ToArray(),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalItems = await QuerySingleAsync<int>(container, totalItemsQuery),
        };
        pagedDto.TotalPages = (int)Math.Ceiling(pagedDto.TotalItems * 1.0 / pagedDto.PageSize);

        return pagedDto;
    }

    public static async Task<PagedDto<T>> AsPagedAsync<T>(this Container container,
        PageableQuery<T> request,
        string sql,
        QueryDefinition fromWhere)
    {
        var queryDefinition = new QueryDefinition(sql + $" OFFSET {request.PageSize * (request.PageNumber - 1)} LIMIT {request.PageSize}");
        var queryResultSetIterator = container.GetItemQueryIterator<T>(queryDefinition);

        var items = new List<T>();
        while (queryResultSetIterator.HasMoreResults)
        {
            var currentResultSet = await queryResultSetIterator.ReadNextAsync();
            foreach (var item in currentResultSet)
            {
                items.Add(item);
            }
        }

        using var scope = ServiceCompositionRoot.CreateScope();
        var logger = scope.ServiceProvider.GetService<ILogger<T>>();

        logger.LogInformation("Executing Query on DB:");
        logger.LogInformation(queryDefinition.QueryText);

        var totalItemsSql = $"SELECT VALUE COUNT(1) {fromWhere.QueryText}";
        var queryParameters = fromWhere.GetQueryParameters();
        var totalItemsQuery = new QueryDefinition(totalItemsSql);
        foreach (var queryParameter in queryParameters)
        {
            totalItemsQuery = totalItemsQuery.WithParameter(queryParameter.Name, queryParameter.Value);
        }
        var pagedDto = new PagedDto<T>
        {
            Items = items.ToArray(),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalItems = await QuerySingleAsync<int>(container, totalItemsQuery),
        };
        pagedDto.TotalPages = (int)Math.Ceiling(pagedDto.TotalItems * 1.0 / pagedDto.PageSize);

        return pagedDto;
    }

    public static async Task<T> QuerySingleAsync<T>(this Container container, string sql)
    {
        return await QuerySingleAsync<T>(container, new QueryDefinition(sql));
    }

    public static async Task<T> QuerySingleAsync<T>(this Container container, QueryDefinition queryDefinition)
    {
        using var scope = ServiceCompositionRoot.CreateScope();
        var logger = scope.ServiceProvider.GetService<ILogger<T>>();

        logger.LogInformation("Executing Query on DB:");
        logger.LogInformation(queryDefinition.QueryText);
        var results = container.GetItemQueryIterator<T>(queryDefinition);

        var item = await results.ReadNextAsync();

        return item.Resource.FirstOrDefault();
    }
}