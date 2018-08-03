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

namespace ConsoleHostBuilderAndLogTest
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

        public RabbitMQSubscriberService(
            ILogger<LifetimeEventsHostedService> logger, 
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
            var queueName = _config.GetSection("RabbitMQLog:PageLogQueue").Value;
            Console.WriteLine(" [x] configden alınan değer => {0} ", queueName);
            queueName = _config["RabbitMQLog:PageLogQueue"];
            Console.WriteLine(" [x] configden alınan değer2 => {0} ", queueName);
            queueName = _context.Configuration.GetSection("RabbitMQLog").GetSection("PageLogQueue").Value;
            Console.WriteLine(" [x] configden alınan değer3 => {0} ", queueName);
            queueName = _configRabbitMQ.Value.PageLogQueue;
            Console.WriteLine(" [x] configden alınan değer4 => {0} ", queueName);
            queueName = "pageLogRabbit";
            /*using (var channel = connection.CreateModel())
            {*/

            _rabbitMQQueueNamePageAccessLog = _config.GetSection("RabbitMQLog").GetSection("PageLogQueue").Value;

            Console.WriteLine(" [x] configden alınan değer5 => {0} ", _rabbitMQQueueNamePageAccessLog);
            //queueName = "pageLogRabbit";

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
                    Console.WriteLine(" [x] Registered Consumer tag ==> {0} ", consumerTag);
                    _logger.LogDebug($"[x] Registered Consumer tag ==> {consumerTag} ");
                };


                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = System.Text.Encoding.UTF8.GetString(body);
                    var pageAccess = JsonConvert.DeserializeObject<RabbitMQPageAccessLogModel>(message);
                    //Console.WriteLine(user);
                    //Console.WriteLine(" [x] Received {0}", message);
                    Console.WriteLine(" [x] Received {0} Host {1} Controller {2} Action {3} sessionID {4}", pageAccess.UserName,
                                                                                                           pageAccess.Host,
                                                                                                           pageAccess.Controller,
                                                                                                           pageAccess.Action,
                                                                                                           pageAccess.SessionID);
                    _logger.LogDebug($"[x] Received {pageAccess.UserName} Host {pageAccess.Host} Controller {pageAccess.Controller} Action {pageAccess.Action} SessionID {pageAccess.SessionID}");
                    
                };

                channel.BasicConsume(queue: _rabbitMQQueueNamePageAccessLog,
                                     autoAck: true,
                                     consumer: consumer);
            } else
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
            var factory = new ConnectionFactory() { HostName = _rabbitMQHost };
                //var connection = factory.CreateConnection();
                try
                {
                    using (var connection = factory.CreateConnection())
                    {
                        using (var channel = connection.CreateModel())
                        {

                            this.PageEntryLogger(connection, channel);
                            //this.ExceptionLogger(connection, channel);

                            Console.WriteLine(" Press [enter] to exit.");
                            Console.ReadLine();
                        }
                    }
                }
                catch (Exception ex)
                {
                    //throw new RabbitMQSubscribeException(ex);
                    _logger.LogError($"{ex.Message}");
                }
                return Task.CompletedTask;
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
