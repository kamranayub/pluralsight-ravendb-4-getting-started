using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sample.Services;

namespace Sample
{
    public class TalkServiceProvider
    {
        private readonly ITalkService _inMemoryTalks;
        private readonly ITalkService _ravenTalks;
        private readonly ILogger<TalkServiceProvider> _logger;

        public TalkServiceProvider(
            InMemoryTalkService inMemoryTalks,
            RavenTalkService ravenTalks,
            ILogger<TalkServiceProvider> logger)
        {
            _inMemoryTalks = inMemoryTalks;
            _ravenTalks = ravenTalks;
            _logger = logger;

            System.Console.OutputEncoding = System.Text.Encoding.UTF8;
        }


        /// <summary>
        /// Utility to easily fallback to in-memory impl
        /// if Raven is not implemented.
        ///
        /// Logs whether or not we use the Raven service vs. in-memory service
        /// using a LINQ expression.
        ///
        /// Emojis best viewed in Visual Studio Code terminal.
        /// </summary>
        public async Task<T> TryRaven<T>(Expression<Func<ITalkService, Task<T>>> callback)
        {
            var cb = callback.Compile();
            var method = callback.Body as MethodCallExpression;

            if (method == null)
            {
                throw new Exception("Callback must be a method (e.g. x => x.Foo())");
            }

            foreach (var service in new[] { _ravenTalks, _inMemoryTalks })
            {
                var stopwatch = new Stopwatch();
                try
                {
                    stopwatch.Start();
                    var result = await cb(service);

                    if (service is RavenTalkService)
                    {
                        _logger.LogInformation("✔️   Used Raven service to execute method: {0} in {1}ms", 
                            method.Method.Name, stopwatch.ElapsedMilliseconds);
                    }
                    else
                    {
                        _logger.LogInformation("❌   Used In-Memory service to execute method: {0} in {1}ms", 
                            method.Method.Name, stopwatch.ElapsedMilliseconds);
                    }

                    return result;
                }
                catch (NotImplementedException)
                {
                    continue;
                }
                catch
                {
                    throw;
                }
                finally
                {
                    stopwatch.Stop();
                }
            }
            throw new Exception("Could not find implemented method");
        }
    }
}