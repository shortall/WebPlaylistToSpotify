﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace WebPlaylistToSpotify.Auth
{
    /// <summary>
    /// Based on https://github.com/IdentityModel/IdentityModel.OidcClient
    /// </summary>
    public class LoopbackHttpListener : IDisposable
    {
        private const int defaultTimeout = 60 * 5;

        private readonly IWebHost _host;
        private readonly TaskCompletionSource<string> _source = new TaskCompletionSource<string>();
        private readonly string _url;

        public string Url => _url;

        public LoopbackHttpListener(int port)
        {
            _url = $"http://127.0.0.1:{port}/";

            _host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls(_url)
                .Configure(Configure)
                .Build();
            _host.Start();
        }

        public void Dispose()
        {
            Task.Run(async () =>
            {
                await Task.Delay(500);
                _host.Dispose();
            });
        }

        private void Configure(IApplicationBuilder app)
        {
            app.Run(async ctx =>
            {
                if (ctx.Request.Method == "GET")
                {
                    await SetResultAsync(ctx.Request.QueryString.Value!, ctx);
                }
                else if (ctx.Request.Method == "POST")
                {
                    if (!ctx.Request.ContentType!.Equals("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
                    {
                        ctx.Response.StatusCode = 415;
                    }
                    else
                    {
                        using (var sr = new StreamReader(ctx.Request.Body, Encoding.UTF8))
                        {
                            var body = await sr.ReadToEndAsync();
                            await SetResultAsync(body, ctx);
                        }
                    }
                }
                else
                {
                    ctx.Response.StatusCode = 405;
                }
            });
        }

        private async Task SetResultAsync(string value, HttpContext ctx)
        {
            try
            {
                ctx.Response.StatusCode = 200;
                ctx.Response.ContentType = "text/html";
                await ctx.Response.WriteAsync("<h1>You can now return to the application.</h1>");
                await ctx.Response.Body.FlushAsync();

                _source.TrySetResult(value);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                ctx.Response.StatusCode = 400;
                ctx.Response.ContentType = "text/html";
                await ctx.Response.WriteAsync("<h1>Invalid request.</h1>");
                await ctx.Response.Body.FlushAsync();
            }
        }

        public Task<string> WaitForCallbackAsync(int timeoutInSeconds = defaultTimeout)
        {
            Task.Run(async () =>
            {
                await Task.Delay(timeoutInSeconds * 1000);
                _source.TrySetCanceled();
            });

            return _source.Task;
        }
    }
}
