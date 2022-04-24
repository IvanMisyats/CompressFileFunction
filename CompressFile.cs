using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace CompressFileFunction
{
    public static class CompressFile
    {
        private const int BatchLength = 2048;

        public static async Task<MemoryStream> ZipStream(Stream stream, string? fileName = null)
        {
            fileName ??= "archive";

            var outStream = new MemoryStream();
            using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
            {
                var fileInArchive = archive.CreateEntry(fileName, CompressionLevel.Optimal);
                using (var entryStream = fileInArchive.Open())
                {
                    var buffer = new Memory<byte>(new byte[BatchLength]);
                    int readBytes;
                    while ((readBytes = await stream.ReadAsync(buffer)) > 0)
                    {
                        await entryStream.WriteAsync(buffer[..readBytes]);
                    }
                }
            }

            outStream.Position = 0;

            return outStream;
        }
    }
}
