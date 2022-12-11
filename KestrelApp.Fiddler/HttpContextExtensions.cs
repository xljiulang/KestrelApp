using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace KestrelApp.Fiddler
{
    public static class HttpContextExtensions
    {
        /// <summary>
        /// 序列化请求对象
        /// </summary>
        /// <param name="context"></param>
        /// <param name="writer"></param>
        /// <returns></returns>
        public static async ValueTask SerializeRequestAsync(this HttpContext context, TextWriter writer)
        {
            var request = context.Request;
            await writer.WriteLineAsync($"{request.Method} {request.GetEncodedPathAndQuery()} {request.Protocol}");

            foreach (var header in request.Headers)
            {
                await writer.WriteLineAsync($"{header.Key}:{header.Value}");
            }

            var reader = new HttpStreamReader(request.Body, request.ContentType);
            await reader.ReadAsync(writer);
        }

        /// <summary>
        /// 序列化响应对象
        /// </summary>
        /// <param name="context"></param>
        /// <param name="writer"></param>
        /// <returns></returns>
        public static async ValueTask SerializeResponseAsync(this HttpContext context, TextWriter writer)
        {
            var response = context.Response;
            var reason = context.Features.Get<IHttpResponseFeature>()?.ReasonPhrase;
            if (string.IsNullOrEmpty(reason))
            {
                reason = ReasonPhrases.GetReasonPhrase(response.StatusCode);
            }

            await writer.WriteLineAsync($"{context.Request.Protocol} {response.StatusCode} {reason}");
            foreach (var header in response.Headers)
            {
                await writer.WriteLineAsync($"{header.Key}:{header.Value}");
            }

            var stream = response.Body;
            if (DecompressionProvider.TryGet(response.Headers, out var provider))
            {
                stream = provider(stream);
            }

            var reader = new HttpStreamReader(stream, response.ContentType);
            await reader.ReadAsync(writer);
        }


        private static class DecompressionProvider
        {
            private static readonly Dictionary<string, Func<Stream, Stream>> providers = new(StringComparer.OrdinalIgnoreCase)
            {
                ["br"] = (stream) => new BrotliStream(stream, CompressionMode.Decompress, leaveOpen: true),
                ["gzip"] = (stream) => new GZipStream(stream, CompressionMode.Decompress, leaveOpen: true),
                ["deflate"] = (stream) => new DeflateStream(stream, CompressionMode.Decompress, leaveOpen: true),
            };

            public static bool TryGet(IHeaderDictionary headers, [MaybeNullWhen(false)] out Func<Stream, Stream> provider)
            {
                var encoding = headers.ContentEncoding;
                if (StringValues.IsNullOrEmpty(encoding))
                {
                    provider = null;
                    return false;
                }

                return providers.TryGetValue(encoding!, out provider);
            }
        }

        private class HttpStreamReader : HttpRequestStreamReader
        {
            public HttpStreamReader(Stream stream, string? contentType)
               : base(stream, GetEncoding(contentType))
            {
            }

            private static Encoding GetEncoding(string? contentType)
            {
                if (string.IsNullOrEmpty(contentType))
                {
                    return Encoding.UTF8;
                }

                var match = Regex.Match(contentType, @"(?<=charset\s*=\s*)[\w-]+", RegexOptions.IgnoreCase);
                return match.Success ? Encoding.GetEncoding(match.Value) : Encoding.UTF8;
            }

            public async ValueTask ReadAsync(TextWriter writer, CancellationToken cancellationToken = default)
            {
                using var owner = ArrayPool<char>.Shared.RentArrayOwner(8 * 1024);

                while (true)
                {
                    var memory = owner.AsMemory();
                    var length = await base.ReadAsync(memory, cancellationToken);
                    if (length == 0)
                    {
                        break;
                    }

                    var data = memory.Slice(0, length);
                    await writer.WriteAsync(data, cancellationToken);
                }
            }

            protected override void Dispose(bool disposing)
            {
            }
        }
    }
}
