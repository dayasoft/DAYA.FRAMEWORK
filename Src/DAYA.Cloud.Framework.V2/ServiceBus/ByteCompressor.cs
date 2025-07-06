using System.IO;
using System.IO.Compression;

namespace DAYA.Cloud.Framework.V2.ServiceBus
{
    public static class ByteCompressor
    {
        public static byte[] CompressData(this byte[] data)
        {
            using var outputStream = new MemoryStream();
            using (var gzip = new GZipStream(outputStream, CompressionMode.Compress))
            {
                gzip.Write(data, 0, data.Length);
            }
            return outputStream.ToArray();
        }

        public static byte[] DecompressData(this byte[] compressedData)
        {
            using var inputStream = new MemoryStream(compressedData);
            using var gzip = new GZipStream(inputStream, CompressionMode.Decompress);
            using var outputStream = new MemoryStream();
            gzip.CopyTo(outputStream);
            return outputStream.ToArray();
        }
    }
}
