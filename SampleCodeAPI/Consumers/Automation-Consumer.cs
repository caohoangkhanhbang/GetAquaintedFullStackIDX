using DPSinfra.Kafka;
using DPSinfra.Logger;
using Newtonsoft.Json;
using SampleCodeAPI.Model;

namespace AutomationService.Consumers
{
    //action 1
    public class Automation_Consumer1 : IHostedService
    {
        private readonly IConfiguration _config;
        private readonly IProducer _producer;
        private readonly ILogger _logger;
        private readonly IBoxEvent _boxEvent;

        private readonly string connectString;
        private string? topicName;
        private readonly Consumer eventListConsumer;
        public Automation_Consumer1(IConfiguration config, IProducer producer, IBoxEvent boxEvent, ILogger<Automation_Consumer1> logger)
        {
            _config = config;
            _logger = logger;
            _producer = producer;
            _boxEvent = boxEvent;
            connectString = _config.GetValue<string>("AppConfig:ConnectionString") ?? "";

            var groupid1 = "auto-action-delta-dong-bo";
            eventListConsumer = new Consumer(_config, groupid1);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            topicName = "auto.action.delta";
            _ = Task.Run(() =>
            {
                eventListConsumer.SubscribeTopicAsync(topicName, GetMess);
            }, cancellationToken);

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await eventListConsumer.closeAsync();
        }

        public async void GetMess(MessageWrapper message)
        {
            try
            {
                if (message == null)
                {
                    #region ghi log bug
                    var d1 = new GeneralLog()
                    {
                        name = "[AS] ERROR action 1",
                        message = topicName + " - Message is valid"
                    };
                    _logger.LogError(message: JsonConvert.SerializeObject(d1));
                    #endregion
                    return;
                }
                Console.WriteLine("==============================================");
                Console.WriteLine(topicName);
                Console.WriteLine(JsonConvert.SerializeObject(message));
                Console.WriteLine("==============================================");
                OutboxEventMessage outboxEventMessage = JsonConvert.DeserializeObject<OutboxEventMessage>(message.message);
                if (outboxEventMessage == null)
                {
                    #region ghi log bug
                    var d1 = new GeneralLog()
                    {
                        name = "[AS] ERROR action 1",
                        message = topicName + " - Message is valid",
                        data = JsonConvert.SerializeObject(message)
                    };
                    _logger.LogError(message: JsonConvert.SerializeObject(d1));
                    #endregion
                    return;
                }
                var _event = JsonConvert.DeserializeObject<KhoaPhongBanModel>(outboxEventMessage.Payload);
                if (_event == null)
                {
                    #region ghi log bug
                    var d1 = new GeneralLog()
                    {
                        name = "[AS] ERROR action 1",
                        message = topicName + " - Message is valid",
                        data = JsonConvert.SerializeObject(message)
                    };
                    _logger.LogError(JsonConvert.SerializeObject(d1));
                    #endregion
                    return;
                }

                if (_event.ActionCode == "ACT" && _event.ActionData != null)
                {
                    var input = _event.ActionData;
                    //Xử lý sự kiện liên quan
                    if (input.Type == 0)
                    {

                    }
                    else if (input.Type == 1)
                    {

                    }
                    else
                    {

                    }
                    var output = new 
                    {
                        IsSucess = true
                    };
                    #region save to inbox
                    InboxEventMessage inboxEventMessage = new()
                    {
                        ID = outboxEventMessage.ID,
                        EventType = outboxEventMessage.EventType,
                        Status = 1,
                        ErrorMessage = outboxEventMessage.ErrorMessage,
                        RetryCount = outboxEventMessage.Retry_count,
                        SourceID = outboxEventMessage.SourceID,
                    };
                    var inboxResult = await _boxEvent.changeInboxEvent(inboxEventMessage, connectString, true);
                    #endregion

                    #region save to outbox
                    var topicResultAction = "auto.action.result.delta";
                    var data = new
                    {
                        _event.CustomerID,
                        _event.UserID,
                        _event.ActionID,
                        _event.ActionCode,
                        _event.ActionName,
                        ActionData = output
                    };
                    string guid = Guid.NewGuid().ToString();
                    OutboxEventMessage outbox = new()
                    {
                        ID = guid,
                        AggregateType = outboxEventMessage.AggregateType,
                        AggregateID = outboxEventMessage.AggregateID,
                        EventType = outboxEventMessage.EventType,
                        Payload = JsonConvert.SerializeObject(data),
                        Status = 0,
                        Retry_count = 0,
                        SourceID = "ACTION1",
                        ErrorMessage = ""
                    };
                    await _boxEvent.changeOutboxEvent(outbox, connectString, true);
                    var result = await _producer.PublishProducerAsync(topicResultAction, JsonConvert.SerializeObject(outbox));
                    if (result != null) //chưa thấy trả lỗi
                    {
                        outbox.ID = result.id;
                        outbox.Retry_count = result.retry;
                        outbox.Status = result.statusid;
                        outbox.ErrorMessage = result.error ?? "";
                        outbox.EventType = outboxEventMessage.EventType;
                        var khoi = await _boxEvent.changeOutboxEvent(outbox, connectString); //update status outbox
                    }
                    #endregion
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                #region ghi log bug
                var d1 = new GeneralLog()
                {
                    name = "[AS] ERROR action 1",
                    message = topicName + " - Exception",
                    data = JsonConvert.SerializeObject(e)
                };
                _logger.LogError(JsonConvert.SerializeObject(d1));
                #endregion
            }
        }
    }
}
