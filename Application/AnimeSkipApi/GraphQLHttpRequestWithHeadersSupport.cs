using Application.Shared;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;

namespace Application.AnimeSkipApi;

public class GraphQlHttpRequestWithHeadersSupport : GraphQLHttpRequest
{
    public override HttpRequestMessage ToHttpRequestMessage(GraphQLHttpClientOptions options, IGraphQLJsonSerializer serializer)
    {
      var requestMessage = base.ToHttpRequestMessage(options, serializer);
      requestMessage.Headers.Add("x-client-id", Constants.AnimeSkipKey); // todo 
      return requestMessage;
    }
}