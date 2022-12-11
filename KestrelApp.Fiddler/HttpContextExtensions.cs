using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace KestrelApp.Fiddler
{
    public static class HttpContextExtensions
    {
        public static async ValueTask SerializeRequestAsync(this HttpContext context, TextWriter writer)
        {
            var request = context.Request;
            await writer.WriteLineAsync($"{request.Method} {request.GetEncodedPathAndQuery()} {request.Protocol}");

            foreach (var header in request.Headers)
            {
                await writer.WriteLineAsync($"{header.Key}:{header.Value}");
            }

            var reader = new BodyStreamReader(request.Body);
            var bodyText = await reader.ReadToEndAsync();
            await writer.WriteLineAsync(bodyText);
        }

        public static async ValueTask SerializeResponseAsync(this HttpContext context, TextWriter writer)
        { 
            var response = context.Response;
            var feature = context.Features.Get<IHttpResponseFeature>();

            await writer.WriteLineAsync($"{context.Request.Protocol} {response.StatusCode} {feature?.ReasonPhrase}");
            foreach (var header in response.Headers)
            {
                await writer.WriteLineAsync($"{header.Key}:{header.Value}");
            }

            var stream = response.Body;
            if (DecompressionProvider.TryGet(response, out var provider))
            {
                stream = provider(stream);
            }

            var reader = new BodyStreamReader(stream);
            var bodyText = await reader.ReadToEndAsync();
            await writer.WriteLineAsync(bodyText);
        }


        private static class DecompressionProvider
        {
            private static readonly Dictionary<string, Func<Stream, Stream>> providers = new(StringComparer.OrdinalIgnoreCase)
            {
                ["br"] = (stream) => new BrotliStream(stream, CompressionMode.Decompress, leaveOpen: false),
                ["gzip"] = (stream) => new GZipStream(stream, CompressionMode.Decompress, leaveOpen: false),
                ["deflate"] = (stream) => new DeflateStream(stream, CompressionMode.Decompress, leaveOpen: false),
            };

            public static bool TryGet(HttpResponse response, [MaybeNullWhen(false)] out Func<Stream, Stream> provider)
            {
                var encoding = response.Headers.ContentEncoding;
                return TryGet(encoding, out provider);
            }

            public static bool TryGet(StringValues encoding, [MaybeNullWhen(false)] out Func<Stream, Stream> provider)
            {
                if (StringValues.IsNullOrEmpty(encoding))
                {
                    provider = null;
                    return false;
                }

                return providers.TryGetValue(encoding!, out provider);
            }
        }

        private class BodyStreamReader : StreamReader
        {
            public BodyStreamReader(Stream stream)
                : base(stream)
            {
            }

            protected override void Dispose(bool disposing)
            {
            }
        }
    }
}
