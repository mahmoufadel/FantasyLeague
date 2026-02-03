using Dapr.Client;

namespace WebApi.Services;

public class DapprClient
{
    private readonly DaprClient _daprClient;

    public DapprClient(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }

    public Task PublishEventAsync<T>(string pubsubName, string topicName, T data, CancellationToken cancellationToken = default)
    {
        return _daprClient.PublishEventAsync(pubsubName, topicName, data, cancellationToken: cancellationToken);
    }
}
