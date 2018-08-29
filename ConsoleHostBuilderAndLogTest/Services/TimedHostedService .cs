using System;
using System.Threading;
using System.Threading.Tasks;
using Base.Core.Entities.Log;
using ConsoleHostBuilderAndLogTest.ConfigModels;
using ConsoleHostBuilderAndLogTest.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ConsoleHostBuilderAndLogTest.Services
{
    #region snippet1
    internal class TimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private Timer _timer;
        private Timer _timer2;
        private PageAccessLogContext _context;
        private string _rabbitMQHost;
        private string _rabbitMQQueueNamePageAccessLog;
        private readonly IConfiguration _config;

        public TimedHostedService(ILogger<TimedHostedService> logger,
                                   PageAccessLogContext context,
                                    IConfiguration config,
                                    IOptions<ConfigRabbitMQ> configRabbitMQ
                                   )
        {
            _logger = logger;
            _context = context;
            _config = config;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(5));
            _timer2 = new Timer(DoWorkEF, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(15));

            return Task.CompletedTask;
        }

        private void DoWorkEF(object state)
        {
            _context.PageAccessLogs.AddAsync(new Models.PageAccessLog
            {
                Action = "test action",
                ActionTest = "test action deneme",
                Controller = "Controller test",
                AccessDate = DateTime.Now
            });
            _context.SaveChangesAsync();
            Console.WriteLine("Timed Background Service DoWorkEF() func is working.");
            _logger.LogInformation("Timed Background Service DoWorkEF() func is working.");
        }

        private void PageEntryLogger(IConnection connection, IModel channel)
        {

            _rabbitMQQueueNamePageAccessLog = _config.GetSection("RabbitMQLog").GetSection("PageLogQueue").Value;

            Console.WriteLine(" [x] RabbitMQSubscriberService=>PageEntryLogger configden alınan değer5 => {0} ", _rabbitMQQueueNamePageAccessLog);
            //queueName = "pageLogRabbit";

            if (!string.IsNullOrEmpty(_rabbitMQQueueNamePageAccessLog))
            {
                Console.WriteLine(" [x] RabbitMQSubscriberService=>PageEntryLogger !string.IsNullOrEmpty");
                channel.QueueDeclare(queue: _rabbitMQQueueNamePageAccessLog,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
                //new sharedQueue
                //var consumer = new QueueingBasicConsumer(channel);
                var consumer = new EventingBasicConsumer(channel);

                consumer.Registered += (model, ea) =>
                {
                    var consumerTag = ea.ConsumerTag;
                    Console.WriteLine(" [x] RabbitMQSubscriberService=>PageEntryLogger Registered Consumer tag ==> {0} ", consumerTag);
                    _logger.LogDebug($"[x] RabbitMQSubscriberService=>PageEntryLogger Registered Consumer tag ==> {consumerTag} ");
                };


                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = System.Text.Encoding.UTF8.GetString(body);
                    //var pageAccess = JsonConvert.DeserializeObject<RabbitMQPageAccessLogModel>(message);
                    var pageAccess = JsonConvert.DeserializeObject<PageAccessLogModel>(message);
                    //Console.WriteLine(user);
                    //Console.WriteLine(" [x] Received {0}", message);
                    Console.WriteLine(" [x] RabbitMQSubscriberService=>PageEntryLogger Received {0} Host {1} Controller {2} Action {3} sessionID {4} publicKey {5} userName {6} userIP {7}", pageAccess.UserName,
                                                                                                           pageAccess.Host,
                                                                                                           pageAccess.Controller,
                                                                                                           pageAccess.Action,
                                                                                                           pageAccess.SessionID,
                                                                                                           pageAccess.UserPublicKey,
                                                                                                           pageAccess.UserName,
                                                                                                           pageAccess.UserIP);
                    _logger.LogDebug($"[x] RabbitMQSubscriberService=>PageEntryLogger Received {pageAccess.UserName} Host {pageAccess.Host} Controller {pageAccess.Controller} Action {pageAccess.Action} SessionID {pageAccess.SessionID}");

                };

                channel.BasicConsume(queue: _rabbitMQQueueNamePageAccessLog,
                                     autoAck: true,
                                     consumer: consumer);
            }
            else
            {
                Console.WriteLine("RabbitMQ page access log queueName okuma hatası code: RMQ0002");
                _logger.LogError($"RabbitMQ page access log queueName okuma hatası code: RMQ0002");
            }


        }


        private void DoWork(object state)
        {
            Console.WriteLine("Timed Background Service is working.");
            _logger.LogInformation("Timed Background Service is working.");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Timed Background Service is stopping.");
            _logger.LogInformation("Timed Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
    #endregion
}
