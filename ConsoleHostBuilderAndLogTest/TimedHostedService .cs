using System;
using System.Threading;
using System.Threading.Tasks;
using ConsoleHostBuilderAndLogTest.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConsoleHostBuilderAndLogTest
{
    #region snippet1
    internal class TimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private Timer _timer;
        private Timer _timer2;
        private PageAccessLogContext _context;

        public TimedHostedService(ILogger<TimedHostedService> logger,
                                   PageAccessLogContext context
                                   )
        {
            _logger = logger;
            _context = context;
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
