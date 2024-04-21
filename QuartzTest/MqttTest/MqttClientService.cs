using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;

namespace QuartzTest.MqttTest;


#region Reference
//https://www.cnblogs.com/weskynet/p/16441219.html
#endregion
public class MqttClientService
{
    public IMqttClient? _client;

    public MqttClientService()
    {
        if (_client is null)
        {
            MqttClientStart();
        }
    }

    public void MqttClientStart()
    {
        var clientOptions = new MqttClientOptionsBuilder()
            .WithWebSocketServer(configure: config =>
            {
                config.WithUri("ws://broker.emqx.io:8083")
                    .WithKeepAliveInterval(TimeSpan.FromSeconds(60));
            })
            .WithClientId("testClient1")
            .WithCleanSession()
            .WithTlsOptions(opt => { opt.UseTls(false); })
            .Build();

        _client = new MqttFactory().CreateMqttClient();
        _client.ConnectedAsync += MqttClient_ConnectedAsync;
        _client.DisconnectedAsync += MqttClient_DisconnectedAsync;
        _client.ApplicationMessageReceivedAsync += MqttClient_ApplicationMessageReceivedAsync;

        _client.SubscribeAsync("topic_01", MqttQualityOfServiceLevel.AtLeastOnce);
        _client.UnsubscribeAsync("topic_01");
        _client.SubscribeAsync("topic_01", MqttQualityOfServiceLevel.AtLeastOnce);
        _client.ConnectAsync(clientOptions);
    }

    private Task MqttClient_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
    {
        return Task.CompletedTask;
    }

    private Task MqttClient_ConnectedAsync(MqttClientConnectedEventArgs mqttClientConnectedEventArgs)
    {
        return Task.CompletedTask;
    }

    private Task MqttClient_DisconnectedAsync(MqttClientDisconnectedEventArgs mqttClientDisconnectedEventArgs)
    {
        return Task.CompletedTask;
    }
}