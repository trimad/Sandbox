using System;
using System.Buffers.Binary;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Sandbox.Fractals
{
    internal static class StreamingPngWriter
    {
        private static readonly byte[] PngSignature = { 137, 80, 78, 71, 13, 10, 26, 10 };
        private static readonly byte[] Ihdr = Encoding.ASCII.GetBytes("IHDR");
        private static readonly byte[] Idat = Encoding.ASCII.GetBytes("IDAT");
        private static readonly byte[] Iend = Encoding.ASCII.GetBytes("IEND");

        public static void SaveArgbCanvas(string path, int[] canvas, int width, int height)
        {
            if (canvas == null)
            {
                throw new InvalidOperationException("Cannot save a PNG before the canvas has been generated.");
            }

            if (width <= 0 || height <= 0)
            {
                throw new InvalidOperationException($"Cannot save a PNG with invalid dimensions: {width}x{height}.");
            }

            long pixelCount = (long)width * height;
            if (canvas.LongLength < pixelCount)
            {
                throw new InvalidOperationException($"Canvas length {canvas.LongLength:N0} is too small for {width}x{height}.");
            }

            string directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string tempPath = path + ".tmp";
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }

            try
            {
                using (FileStream file = new FileStream(tempPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 1 << 20))
                {
                    file.Write(PngSignature);
                    WriteHeader(file, width, height);

                    using (PngIdatStream idatStream = new PngIdatStream(file))
                    using (ZLibStream zlib = new ZLibStream(idatStream, CompressionLevel.Fastest, leaveOpen: true))
                    {
                        byte[] row = new byte[checked(width * 4 + 1)];
                        for (int y = 0; y < height; y++)
                        {
                            row[0] = 0;
                            int source = y * width;
                            int destination = 1;

                            for (int x = 0; x < width; x++)
                            {
                                int pixel = canvas[source + x];
                                int alpha = pixel >> 24 & 255;
                                row[destination++] = (byte)(pixel >> 16 & 255);
                                row[destination++] = (byte)(pixel >> 8 & 255);
                                row[destination++] = (byte)(pixel & 255);
                                row[destination++] = (byte)(alpha == 0 ? 255 : alpha);
                            }

                            zlib.Write(row);
                        }
                    }

                    WriteChunk(file, Iend, ReadOnlySpan<byte>.Empty);
                }

                File.Move(tempPath, path, overwrite: true);
            }
            catch
            {
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }

                throw;
            }
        }

        private static void WriteHeader(Stream stream, int width, int height)
        {
            Span<byte> data = stackalloc byte[13];
            BinaryPrimitives.WriteInt32BigEndian(data.Slice(0, 4), width);
            BinaryPrimitives.WriteInt32BigEndian(data.Slice(4, 4), height);
            data[8] = 8;
            data[9] = 6;
            data[10] = 0;
            data[11] = 0;
            data[12] = 0;
            WriteChunk(stream, Ihdr, data);
        }

        private static void WriteChunk(Stream stream, byte[] type, ReadOnlySpan<byte> data)
        {
            Span<byte> header = stackalloc byte[4];
            BinaryPrimitives.WriteInt32BigEndian(header, data.Length);
            stream.Write(header);
            stream.Write(type);
            stream.Write(data);

            uint crc = Crc32.Update(0xFFFFFFFF, type);
            crc = Crc32.Update(crc, data) ^ 0xFFFFFFFF;
            BinaryPrimitives.WriteUInt32BigEndian(header, crc);
            stream.Write(header);
        }

        private sealed class PngIdatStream : Stream
        {
            private const int ChunkSize = 1 << 20;

            private readonly Stream output;
            private readonly byte[] buffer = new byte[ChunkSize];
            private int bufferLength;
            private bool disposed;

            public PngIdatStream(Stream output)
            {
                this.output = output;
            }

            public override bool CanRead => false;
            public override bool CanSeek => false;
            public override bool CanWrite => true;
            public override long Length => throw new NotSupportedException();
            public override long Position
            {
                get => throw new NotSupportedException();
                set => throw new NotSupportedException();
            }

            public override void Flush()
            {
                FlushChunk();
                output.Flush();
            }

            public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
            public override void SetLength(long value) => throw new NotSupportedException();

            public override void Write(byte[] source, int offset, int count)
            {
                Write(source.AsSpan(offset, count));
            }

            public override void Write(ReadOnlySpan<byte> source)
            {
                while (!source.IsEmpty)
                {
                    int count = Math.Min(buffer.Length - bufferLength, source.Length);
                    source.Slice(0, count).CopyTo(buffer.AsSpan(bufferLength, count));
                    bufferLength += count;
                    source = source.Slice(count);

                    if (bufferLength == buffer.Length)
                    {
                        FlushChunk();
                    }
                }
            }

            protected override void Dispose(bool disposing)
            {
                if (!disposed && disposing)
                {
                    FlushChunk();
                    disposed = true;
                }

                base.Dispose(disposing);
            }

            private void FlushChunk()
            {
                if (bufferLength == 0)
                {
                    return;
                }

                WriteChunk(output, Idat, buffer.AsSpan(0, bufferLength));
                bufferLength = 0;
            }
        }

        private static class Crc32
        {
            private static readonly uint[] Table = CreateTable();

            public static uint Update(uint crc, ReadOnlySpan<byte> data)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    crc = Table[(crc ^ data[i]) & 0xFF] ^ (crc >> 8);
                }

                return crc;
            }

            private static uint[] CreateTable()
            {
                uint[] table = new uint[256];
                for (uint i = 0; i < table.Length; i++)
                {
                    uint crc = i;
                    for (int bit = 0; bit < 8; bit++)
                    {
                        crc = (crc & 1) == 1 ? 0xEDB88320 ^ (crc >> 1) : crc >> 1;
                    }

                    table[i] = crc;
                }

                return table;
            }
        }
    }
}
