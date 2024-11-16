using System.IO.Compression;

namespace MobileProject.Utility
{
    public static class GzipCompression
    {
        public static byte[] DecompressGzip(this byte[] data)
        {
            using var compressedStream = new MemoryStream(data);
            using var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress);
            using var resultStream = new MemoryStream();
            zipStream.CopyTo(resultStream);
            return resultStream.ToArray();
        }

    }
}
