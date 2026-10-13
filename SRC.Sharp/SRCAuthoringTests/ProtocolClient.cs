using SRCAuthoring.Protocol;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SRCAuthoring.Tests
{
    /// <summary>
    /// テストからサーバー（LSP・DAP）とパイプでやりとりする。受け取ったメッセージは順に溜めておく。
    /// </summary>
    public sealed class ProtocolClient : IDisposable
    {
        private readonly AnonymousPipeServerStream toServer = new AnonymousPipeServerStream(PipeDirection.Out);
        private readonly AnonymousPipeServerStream fromServer = new AnonymousPipeServerStream(PipeDirection.In);
        private readonly MessageStream client;
        private readonly List<JsonNode> received = new List<JsonNode>();
        private readonly object sync = new object();
        private readonly Task reader;

        public ProtocolClient()
        {
            ServerInput = new AnonymousPipeClientStream(PipeDirection.In, toServer.ClientSafePipeHandle);
            ServerOutput = new AnonymousPipeClientStream(PipeDirection.Out, fromServer.ClientSafePipeHandle);
            client = new MessageStream(fromServer, toServer);
            reader = Task.Run(async () =>
            {
                try
                {
                    while (true)
                    {
                        var message = await client.ReadAsync();
                        if (message == null)
                        {
                            return;
                        }
                        lock (sync)
                        {
                            received.Add(message);
                            Monitor.PulseAll(sync);
                        }
                    }
                }
                catch (IOException)
                {
                }
                catch (ObjectDisposedException)
                {
                }
            });
        }

        /// <summary>サーバーが読む側</summary>
        public Stream ServerInput { get; }
        /// <summary>サーバーが書く側</summary>
        public Stream ServerOutput { get; }

        public void Send(JsonObject message)
        {
            client.WriteAsync(message).Wait();
        }

        /// <summary>条件に合うメッセージが来るまで待つ（来たものは取り除く）</summary>
        public JsonNode WaitFor(Func<JsonNode, bool> predicate, string description, int timeoutSeconds = 30)
        {
            var deadline = DateTime.UtcNow.AddSeconds(timeoutSeconds);
            lock (sync)
            {
                while (true)
                {
                    var index = received.FindIndex(x => predicate(x));
                    if (index >= 0)
                    {
                        var message = received[index];
                        received.RemoveAt(index);
                        return message;
                    }
                    var rest = deadline - DateTime.UtcNow;
                    if (rest <= TimeSpan.Zero)
                    {
                        throw new TimeoutException($"{description} が来ません。受け取ったもの: " + string.Join("\n", received));
                    }
                    Monitor.Wait(sync, rest);
                }
            }
        }

        public void Dispose()
        {
            toServer.Dispose();
            fromServer.Dispose();
            ServerInput.Dispose();
            ServerOutput.Dispose();
        }
    }
}
