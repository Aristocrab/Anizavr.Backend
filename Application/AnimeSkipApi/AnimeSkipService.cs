using Application.AnimeSkipApi.Entities;
using GraphQL.Client.Abstractions;

namespace Application.AnimeSkipApi;

public class AnimeSkipService
{
    private readonly IGraphQLClient _graphQlClient;

    public AnimeSkipService(IGraphQLClient graphQlClient)
    {
        _graphQlClient = graphQlClient;
    }

    public async Task<ShowsByExternalId?> GetTimestampsById(long id)
    {
        var request = new GraphQlHttpRequestWithHeadersSupport {
            Query = @"
            query GetTimestampsById($id: String!) {
              findShowsByExternalId(service: ANILIST, serviceId: $id) {
                name
                episodes {
                  name
                  timestamps {
                    type {
                      name
                    }
                    at
                  }
                }
              }
            }",
            OperationName = "GetTimestampsById",
            Variables = new {
                id = id.ToString()
            }
        };

        try
        {
            var graphQlResponse = await _graphQlClient.SendQueryAsync<ShowsByExternalId>(request);
            return graphQlResponse.Data;
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
            return null;
        }
    }
}