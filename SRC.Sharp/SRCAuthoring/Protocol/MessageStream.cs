using System;
using System.IO;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SRCAuthoring.Protocol
{
    /// <summary>
    /// LSP・DAP で共通の、ヘッダ（Content-Length）付きの JSON メッセージの読み書き。
    /// https://microsoft.github.io/language-server-protocol/specifications/lsp/3.17/specification/#baseProtocol
    /// </summary>
    public sealed class MessageStream
    {
        private readonly Stream input;
        private readonly Stream output;
        private readonly SemaphoreSlim writeLock = new SemaphoreSlim(1, 1);

        public MessageStream(Stream input, Stream output)
        {
            this.input = input;
            this.output = output;
        }

        /// <summary>次のメッセージを読む。入力が終われば null。</summary>
        public async Task<JsonNode> ReadAsync(CancellationToken cancellationToken = default)
        {
            var contentLength = -1;
            while (true)
            {
                var line = await ReadHeaderLineAsync(cancellationToken);
                if (line == null)
                {
                    return null;
                }
                if (line.Length == 0)
                {
                    break;
                }
                var colon = line.IndexOf(':');
                if (colon > 0 && line.Substring(0, colon).Trim().Equals("Content-Length", StringComparison.OrdinalIgnoreCase))
                {
                    contentLength = int.Parse(line.Substring(colon + 1).Trim());
                }
            }
            if (contentLength < 0)
            {
                throw new InvalidDataException("Content-Length がありません");
            }
            var body = new byte[contentLength];
            var read = 0;
            while (read < contentLength)
            {
                var n = await input.ReadAsync(body, read, contentLength - read, cancellationToken);
                if (n == 0)
                {
                    return null;
                }
                read += n;
            }
            return JsonNode.Parse(body);
        }

        private async Task<string> ReadHeaderLineAsync(CancellationToken cancellationToken)
        {
            var buffer = new MemoryStream();
            var one = new byte[1];
            while (true)
            {
                var n = await input.ReadAsync(one, 0, 1, cancellationToken);
                if (n == 0)
                {
                    return buffer.Length == 0 ? null : Encoding.ASCII.GetString(buffer.ToArray());
                }
                if (one[0] == '\n')
                {
                    var text = Encoding.ASCII.GetString(buffer.ToArray());
                    return text.TrimEnd('\r');
                }
                buffer.WriteByte(one[0]);
            }
        }

        public async Task WriteAsync(JsonNode message, CancellationToken cancellationToken = default)
        {
            var body = Encoding.UTF8.GetBytes(message.ToJsonString());
            var header = Encoding.ASCII.GetBytes($"Content-Length: {body.Length}\r\n\r\n");
            await writeLock.WaitAsync(cancellationToken);
            try
            {
                await output.WriteAsync(header, 0, header.Length, cancellationToken);
                await output.WriteAsync(body, 0, body.Length, cancellationToken);
                await output.FlushAsync(cancellationToken);
            }
            finally
            {
                writeLock.Release();
            }
        }
    }
}
