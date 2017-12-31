using System;

namespace Benchmark.Hashes
{
    /// <summary>
    /// https://raw.githubusercontent.com/EventStore/EventStore/release-v4.0.2/src/EventStore.Core/Index/Hashes/XXHashUnsafe.cs
    /// </summary>
    public class XxHashUnsafe
    {
        private const uint Seed = 0xc58f1a7b;

        private const uint Prime1 = 2654435761U;
        private const uint Prime2 = 2246822519U;
        private const uint Prime3 = 3266489917U;
        private const uint Prime4 = 668265263U;
        private const int Prime5 = 0x165667b1;

        public unsafe UInt32 Hash(string s)
        {
            fixed (char* input = s)
            {
                return Hash((byte*)input, (uint)s.Length * sizeof(char), Seed);
            }
        }

        public unsafe uint Hash(byte[] data)
        {
            fixed (byte* input = &data[0])
            {
                return Hash(input, (uint)data.Length, Seed);
            }
        }

        public unsafe uint Hash(byte[] data, int offset, uint len, uint seed)
        {
            fixed (byte* input = &data[offset])
            {
                return Hash(input, len, seed);
            }
        }

        private unsafe static uint Hash(byte* data, uint len, uint seed)
        {
            if (len < 16)
                return HashSmall(data, len, seed);

            uint v1 = seed + Prime1;
            uint v2 = v1 * Prime2 + len;
            uint v3 = v2 * Prime3;
            uint v4 = v3 * Prime4;

            uint* p = (uint*)data;
            uint* limit = (uint*)(data + len - 16);

            while (p < limit)
            {
                v1 += Rotl32(v1, 13); v1 *= Prime1; v1 += *p; p++;
                v2 += Rotl32(v2, 11); v2 *= Prime1; v2 += *p; p++;
                v3 += Rotl32(v3, 17); v3 *= Prime1; v3 += *p; p++;
                v4 += Rotl32(v4, 19); v4 *= Prime1; v4 += *p; p++;
            }

            p = limit;
            v1 += Rotl32(v1, 17); v2 += Rotl32(v2, 19); v3 += Rotl32(v3, 13); v4 += Rotl32(v4, 11);
            v1 *= Prime1; v2 *= Prime1; v3 *= Prime1; v4 *= Prime1;
            v1 += *p; p++; v2 += *p; p++; v3 += *p; p++; v4 += *p;
            v1 *= Prime2; v2 *= Prime2; v3 *= Prime2; v4 *= Prime2;
            v1 += Rotl32(v1, 11); v2 += Rotl32(v2, 17); v3 += Rotl32(v3, 19); v4 += Rotl32(v4, 13);
            v1 *= Prime3; v2 *= Prime3; v3 *= Prime3; v4 *= Prime3;

            uint crc = v1 + Rotl32(v2, 3) + Rotl32(v3, 6) + Rotl32(v4, 9);
            crc ^= crc >> 11;
            crc += (Prime4 + len) * Prime1;
            crc ^= crc >> 15;
            crc *= Prime2;
            crc ^= crc >> 13;
            return crc;
        }

        private unsafe static uint HashSmall(byte* data, uint len, uint seed)
        {
            byte* p = data;
            byte* bEnd = data + len;
            byte* limit = bEnd - 4;

            uint idx = seed + Prime1;
            uint crc = Prime5;

            while (p < limit)
            {
                crc += (*(uint*)p) + idx;
                idx++;
                crc += Rotl32(crc, 17) * Prime4;
                crc *= Prime1;
                p += 4;
            }

            while (p < bEnd)
            {
                crc += (*p) + idx;
                idx++;
                crc *= Prime1;
                p++;
            }

            crc += len;

            crc ^= crc >> 15;
            crc *= Prime2;
            crc ^= crc >> 13;
            crc *= Prime3;
            crc ^= crc >> 16;

            return crc;
        }

        private static UInt32 Rotl32(UInt32 x, int r)
        {
            return (x << r) | (x >> (32 - r));
        }
    }
}
