using System.Text;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;

namespace QuartzTest.MqttTest;


#region Reference
//mqtt
//https://www.cnblogs.com/weskynet/p/16441219.html
//https://blog.csdn.net/m0_46601820/article/details/134969551
//https://www.emqx.io/docs/zh/latest/messaging/publish-and-subscribe.html
//quartz
//https://www.cnblogs.com/workcn/p/17491550.html
//https://www.cnblogs.com/youring2/p/quartz_net.html
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
    /*
     docker run -d --name emqx-test -p 31883:1883 -p 38883:8883 -p 38083:8083 -p 38084:8084 -p 38081:8081 -p 48083:18083 -v G:\Tools\emqx\etc:/opt/emqx/etc -v G:\Tools\emqx\data:/opt/emqx/data -v G:\Tools\emqx\log:/opt/emqx/log emqx/emqx:latest
     */
    public void MqttClientStart()
    {
        var clientOptions = new MqttClientOptionsBuilder()
            //.WithWebSocketServer(configure: config =>
            //{
            //    config.WithUri("ws://broker.emqx.io:8083")
            //        .WithKeepAliveInterval(TimeSpan.FromSeconds(60));
            //})
            .WithClientId(Guid.NewGuid().ToString().Replace("-", "").ToUpper())
            .WithCredentials("admin", "public123")
            .WithTcpServer("localhost", 31883)
            .WithCleanSession()
            .WithKeepAlivePeriod(TimeSpan.FromSeconds(100.5))
            .WithTlsOptions(opt => { opt.UseTls(false); })
            .Build();

        _client = new MqttFactory().CreateMqttClient();
        _client.ConnectedAsync += MqttClient_ConnectedAsync;
        _client.DisconnectedAsync += MqttClient_DisconnectedAsync;
        _client.ApplicationMessageReceivedAsync += MqttClient_ApplicationMessageReceivedAsync;

        _client.ConnectAsync(clientOptions);
    }

    private Task MqttClient_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
    {
        var t = arg.ApplicationMessage.Topic.Trim() == "topic_02";
        if (t)
        {
            var msg = Encoding.UTF8.GetString(arg.ApplicationMessage.PayloadSegment);
            Console.WriteLine(msg);
        }
        return Task.CompletedTask;
    }

    private Task MqttClient_ConnectedAsync(MqttClientConnectedEventArgs mqttClientConnectedEventArgs)
    {
        //_client.SubscribeAsync("topic_02", MqttQualityOfServiceLevel.AtLeastOnce);
        //_client.UnsubscribeAsync("topic_02");
        _client.SubscribeAsync("topic_02", MqttQualityOfServiceLevel.AtLeastOnce);
        return Task.CompletedTask;
    }

    private Task MqttClient_DisconnectedAsync(MqttClientDisconnectedEventArgs mqttClientDisconnectedEventArgs)
    {
        return Task.CompletedTask;
    }

    public async Task<bool> Publish(string data)
    {
        var message = new MqttApplicationMessage
        {
            Topic = "topic_02",
            PayloadSegment = Encoding.Default.GetBytes(data),
            QualityOfServiceLevel = MqttQualityOfServiceLevel.AtLeastOnce,
            Retain = true
        };
        var t = await _client!.PublishAsync(message);
        return t.IsSuccess;
    }

    public async Task<bool> Publish(string topic, string data)
    {
        var message = new MqttApplicationMessage
        {
            Topic = topic,
            PayloadSegment = Encoding.Default.GetBytes(data),
            QualityOfServiceLevel = MqttQualityOfServiceLevel.AtLeastOnce,
            Retain = true
        };
        var t = await _client!.PublishAsync(message);
        return t.IsSuccess;
    }
}

public enum TelemetryTypeEnum
{
    ChangeTelemetry = 1,
    TriggerTelemetry,
    SubscribeTelemetry
}