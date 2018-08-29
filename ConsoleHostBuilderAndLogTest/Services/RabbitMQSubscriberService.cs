using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using ConsoleHostBuilderAndLogTest.Models;
using Microsoft.Extensions.Options;
using ConsoleHostBuilderAndLogTest.ConfigModels;
using Base.Core.Entities.Log;

namespace ConsoleHostBuilderAndLogTest.Services
{
    class RabbitMQSubscriberService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IApplicationLifetime _appLifetime;
        private readonly IConfiguration _config;
        private readonly HostBuilderContext _context;
        private readonly IOptions<ConfigRabbitMQ> _configRabbitMQ;
        private  string _rabbitMQHost;
        private  string _rabbitMQQueueNamePageAccessLog;
        private string _rabbitMQQueueNamePageActivityLog;

        public RabbitMQSubscriberService(
            ILogger<RabbitMQSubscriberService> logger, 
            IApplicationLifetime appLifetime,
            IConfiguration config,
            HostBuilderContext context,
            IOptions<ConfigRabbitMQ> configRabbitMQ)
        {
            _logger = logger;
            _appLifetime = appLifetime;
            _config = config;
            _context = context;
            _configRabbitMQ = configRabbitMQ;
        }

        private void PageEntryLogger(IConnection connection, IModel channel)
        {

            //var queueName = _config.GetSection("RabbitMQLog:PageLogQueue").GetSection("PageLogQueue:").Value;
            /*var queueName = _config.GetSection("RabbitMQLog:PageLogQueue").Value;
            Console.WriteLine(" [x] configden alınan değer => {0} ", queueName);
            queueName = _config["RabbitMQLog:PageLogQueue"];
            Console.WriteLine(" [x] configden alınan değer2 => {0} ", queueName);
            queueName = _context.Configuration.GetSection("RabbitMQLog").GetSection("PageLogQueue").Value;
            Console.WriteLine(" [x] configden alınan değer3 => {0} ", queueName);
            queueName = _configRabbitMQ.Value.PageLogQueue;
            Console.WriteLine(" [x] configden alınan değer4 => {0} ", queueName);
            queueName = "pageLogRabbit";*/
            /*using (var channel = connection.CreateModel())
            {*/

            _rabbitMQQueueNamePageAccessLog = _config.GetSection("RabbitMQLog").GetSection("PageLogQueue").Value;
            Console.WriteLine(" [x] RabbitMQSubscriberService=>PageEntryLogger configden alınan değer5 => {0} ", _rabbitMQQueueNamePageAccessLog);

            if (!string.IsNullOrEmpty(_rabbitMQQueueNamePageAccessLog))
            {
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
                    Console.WriteLine(" [x] RabbitMQSubscriberService=>PageEntryLogger Received {0} Host {1} Controller {2} Action {3} sessionID {4} publicKey {5} userName {6} userIP {7}", pageAccess.UserName,
                                                                                                           pageAccess.Host,
                                                                                                           pageAccess.Controller,
                                                                                                           pageAccess.Action,
                                                                                                           pageAccess.SessionID,
                                                                                                           pageAccess.UserPublicKey,
                                                                                                           pageAccess.UserName,
                                                                                                           pageAccess.UserIP);
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    _logger.LogDebug($"[x] RabbitMQSubscriberService=>PageEntryLogger Received {pageAccess.UserName} Host {pageAccess.Host} Controller {pageAccess.Controller} Action {pageAccess.Action} SessionID {pageAccess.SessionID}");
                };

                channel.BasicConsume(queue: _rabbitMQQueueNamePageAccessLog,
                                     autoAck: false,
                                     consumer: consumer);
            } else
            {
                Console.WriteLine("RabbitMQ page access log queueName okuma hatası code: RMQ0002");
                _logger.LogError($"RabbitMQ page access log queueName okuma hatası code: RMQ0002");
            }
        }

        private void PageActivityLogger(IConnection connection, IModel channel)
        {
            _rabbitMQQueueNamePageActivityLog = _config.GetSection("RabbitMQLog").GetSection("PageActivityLogQueue").Value;

            Console.WriteLine(" [x] RabbitMQSubscriberService=>PageActivityLogger configden alınan değer5 => {0} ", _rabbitMQQueueNamePageActivityLog);

            if (!string.IsNullOrEmpty(_rabbitMQQueueNamePageActivityLog))
            {
                channel.QueueDeclare(queue: _rabbitMQQueueNamePageActivityLog,
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
                    Console.WriteLine(" [x] RabbitMQSubscriberService=>PageActivityLogger PageActivityLogger Registered Consumer tag ==> {0} ", consumerTag);
                    _logger.LogDebug($"[x] RabbitMQSubscriberService=>PageActivityLogger PageActivityLogger Registered Consumer tag ==> {consumerTag} ");
                };

                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = System.Text.Encoding.UTF8.GetString(body);
                    var pageActivity = JsonConvert.DeserializeObject<PageActivityLogModel>(message);
                    Console.WriteLine(" [x] RabbitMQSubscriberService=>PageActivityLogger Received {0} Host {1} Controller {2} Action {3} sessionID {4} publicKey {5} userName {6} userIP {7}", pageActivity.UserName,
                                                                                                           pageActivity.Host,
                                                                                                           pageActivity.Controller,
                                                                                                           pageActivity.Action,
                                                                                                           pageActivity.SessionID,
                                                                                                           pageActivity.UserPublicKey,
                                                                                                           pageActivity.UserName,
                                                                                                           pageActivity.UserIP);
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    _logger.LogDebug($"[x] RabbitMQSubscriberService=>PageActivityLogger Received {pageActivity.UserName} Host {pageActivity.Host} Controller {pageActivity.Controller} Action {pageActivity.Action} SessionID {pageActivity.SessionID}");
                    
                };
                channel.BasicConsume(queue: _rabbitMQQueueNamePageActivityLog,
                                     autoAck: false,
                                     consumer: consumer);
            }
            else
            {
                Console.WriteLine("RabbitMQ page access log queueName okuma hatası code: RMQ0002");
                _logger.LogError($"RabbitMQ page access log queueName okuma hatası code: RMQ0002");
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _appLifetime.ApplicationStarted.Register(OnStarted);
            _appLifetime.ApplicationStopping.Register(OnStopping);
            _appLifetime.ApplicationStopped.Register(OnStopped);
            _rabbitMQHost = _config.GetSection("RabbitMQLog").GetSection("HostName").Value;
            
            if(!string.IsNullOrEmpty(_rabbitMQHost))
            {
            var factory = new ConnectionFactory() {
                    HostName = _rabbitMQHost,
                    UserName = "guest",
                    Password = "guest",
                    //VirtualHost = "/",
                    AutomaticRecoveryEnabled = true,
                    RequestedHeartbeat = 30

            };
                //var connection = factory.CreateConnection();
                try
                {
                    using (var connection = factory.CreateConnection())
                    {
                        using (var channel = connection.CreateModel())
                        {

                            this.PageEntryLogger(connection, channel);
                            this.PageActivityLogger(connection, channel);
                            //this.ExceptionLogger(connection, channel);

                            //Console.WriteLine(" Press [enter] to exit.");
                            Console.ReadLine();
                            //return Task.CompletedTask;
                        }
                    }
                }
                catch (Exception ex)
                {
                    //throw new RabbitMQSubscribeException(ex);
                    _logger.LogError($"{ex.Message}");
                }
                //return Task.CompletedTask;
            } else
            {
                Console.WriteLine("RabbitMQ host okuma hatası code: RMQ0001");
                _logger.LogError($"RabbitMQ host okuma hatası code: RMQ0001");
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void OnStarted()
        {
            _logger.LogInformation("rabbitMQ OnStarted has been called.");
            // Perform post-startup activities here
        }

        private void OnStopping()
        {
            _logger.LogInformation("rabbitMQ OnStopping has been called.");
            // Perform on-stopping activities here
        }

        private void OnStopped()
        {
            _logger.LogInformation("rabbitMQ OnStopped has been called.");
            // Perform post-stopped activities here
        }
    }
}
