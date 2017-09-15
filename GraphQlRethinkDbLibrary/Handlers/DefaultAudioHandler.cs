using System;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using GraphQL.Conventions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace GraphQlRethinkDbLibrary.Handlers
{
    public abstract class DefaultAudioHandler : SpecialHandler
    {
        public abstract IDefaultAudio GetAudio(Id id);
        public abstract byte[] GetData(Id id, int part);

        public override string Path => "/audio/";
        public override async Task Process(HttpContext context)
        {
            try
            {
                if (string.Compare(context.Request.Method, "GET", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    var idString = context.Request.Path.Value.Substring(Path.Length);
                    var id = new Id(idString);
                    var audio = GetAudio(id);
                    context.Response.Headers.Add("Content-Type", audio.ContentType);
                    var result = new VideoStreamResult(index => GetData(index, audio), "audio/mpeg", audio.Length);
                    await result.ExecuteResultAsync(context);
                    return;
                }
            }
            catch (Exception)
            {
                return;
            }

            context.Response.StatusCode = 400;
        }

        private byte[] GetData(long index, IDefaultAudio defaultAudio)
        {
            var part = index / defaultAudio.BlockSize;
            var data = GetData(defaultAudio.Id, Convert.ToInt32(part));
            return data;
        }

        private class VideoStreamResult
        {
            //public Stream FileStream { get; }
            private Func<long, byte[]> GetDataFunction { get; }

            private string ContentType { get; }

            private long Length { get; }

            // default buffer size as defined in BufferedStream type
            private const string MultipartBoundary = "<qwe123>";

            public VideoStreamResult(Func<long, byte[]> getDataFunction, string contentType, long length)
            {
                GetDataFunction = getDataFunction;
                ContentType = contentType;
                Length = length;
            }

            private static bool IsMultipartRequest(RangeHeaderValue range)
            {
                return range?.Ranges != null && range.Ranges.Count > 1;
            }

            private static bool IsRangeRequest(RangeHeaderValue range)
            {
                return range?.Ranges != null && range.Ranges.Count > 0;
            }

            private async Task WriteVideoAsync(HttpResponse response)
            {
                var bufferingFeature = response.HttpContext.Features.Get<IHttpBufferingFeature>();
                bufferingFeature?.DisableResponseBuffering();

                var length = Length;

                var range = GetRanges(response.HttpContext, length);

                response.ContentType = IsMultipartRequest(range)
                    ? $"multipart/byteranges; boundary={MultipartBoundary}"
                    : ContentType;

                response.Headers.Add("Accept-Ranges", "bytes");

                if (IsRangeRequest(range))
                {
                    response.StatusCode = (int)HttpStatusCode.PartialContent;

                    if (!IsMultipartRequest(range))
                    {
                        response.Headers.Add("Content-Range", $"bytes {range.Ranges.First().From}-{range.Ranges.First().To}/{length}");
                    }

                    foreach (var rangeValue in range.Ranges)
                    {
                        if (IsMultipartRequest(range)) // dunno if multipart works
                        {
                            await response.WriteAsync($"--{MultipartBoundary}");
                            await response.WriteAsync(Environment.NewLine);
                            await response.WriteAsync($"Content-type: {ContentType}");
                            await response.WriteAsync(Environment.NewLine);
                            await response.WriteAsync($"Content-Range: bytes {range.Ranges.First().From}-{range.Ranges.First().To}/{length}");
                            await response.WriteAsync(Environment.NewLine);
                        }

                        await WriteDataToResponseBody(rangeValue, response);

                        if (IsMultipartRequest(range))
                        {
                            await response.WriteAsync(Environment.NewLine);
                        }
                    }

                    if (IsMultipartRequest(range))
                    {
                        await response.WriteAsync($"--{MultipartBoundary}--");
                        await response.WriteAsync(Environment.NewLine);
                    }
                }
                else
                {
                    await WriteDataToResponseBody(new RangeItemHeaderValue(0, Length), response);
                }
            }

            private async Task WriteDataToResponseBody(RangeItemHeaderValue rangeValue, HttpResponse response)
            {
                var startIndex = rangeValue.From ?? 0;
                var endIndex = rangeValue.To ?? 0;

                var totalToSend = endIndex - startIndex;

                var bytesRemaining = totalToSend;
                response.ContentLength = bytesRemaining;

                var currentIndex = startIndex;
                while (bytesRemaining > 0)
                {
                    try
                    {
                        var buffer = GetDataFunction(currentIndex);
                        response.Headers.Remove("Content-Length");
                        response.Headers.Add("Content-Length",buffer.Length.ToString());
                        currentIndex += buffer.Length;
                        await response.Body.WriteAsync(buffer, 0, buffer.Length);

                        bytesRemaining -= buffer.Length;
                    }
                    catch (IndexOutOfRangeException)
                    {
                        await response.Body.FlushAsync();
                        return;
                    }
                    finally
                    {
                        await response.Body.FlushAsync();
                    }
                }
            }

            private static RangeHeaderValue GetRanges(HttpContext context, long contentSize)
            {
                RangeHeaderValue rangesResult = null;

                string rangeHeader = context.Request.Headers["Range"];

                if (!string.IsNullOrEmpty(rangeHeader))
                {
                    // rangeHeader contains the value of the Range HTTP Header and can have values like:
                    //      Range: bytes=0-1            * Get bytes 0 and 1, inclusive
                    //      Range: bytes=0-500          * Get bytes 0 to 500 (the first 501 bytes), inclusive
                    //      Range: bytes=400-1000       * Get bytes 500 to 1000 (501 bytes in total), inclusive
                    //      Range: bytes=-200           * Get the last 200 bytes
                    //      Range: bytes=500-           * Get all bytes from byte 500 to the end
                    //
                    // Can also have multiple ranges delimited by commas, as in:
                    //      Range: bytes=0-500,600-1000 * Get bytes 0-500 (the first 501 bytes), inclusive plus bytes 600-1000 (401 bytes) inclusive

                    // Remove "Ranges" and break up the ranges
                    var ranges = rangeHeader.Replace("bytes=", string.Empty).Split(",".ToCharArray());

                    rangesResult = new RangeHeaderValue();

                    foreach (var range in ranges)
                    {
                        const int start = 0, end = 1;

                        long endByte, startByte;

                        var currentRange = range.Split("-".ToCharArray());

                        if (long.TryParse(currentRange[end], out var parsedValue))
                            endByte = parsedValue;
                        else
                            endByte = contentSize - 1;


                        if (long.TryParse(currentRange[start], out parsedValue))
                            startByte = parsedValue;
                        else
                        {
                            // No beginning specified, get last n bytes of file
                            // We already parsed end, so subtract from total and
                            // make end the actual size of the file
                            startByte = contentSize - endByte;
                            endByte = contentSize - 1;
                        }

                        rangesResult.Ranges.Add(new RangeItemHeaderValue(startByte, endByte));
                    }
                }

                return rangesResult;
            }

            public async Task ExecuteResultAsync(HttpContext context)
            {
                await WriteVideoAsync(context.Response);
            }
        }
    }

    public interface IDefaultAudio
    {
        Id Id { get; }
        string ContentType { get; }
        int BlockSize { get; }
        long Length { get; }
    }
}
