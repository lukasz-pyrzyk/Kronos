//
// Copyright (c) 2011 Google, Inc.
// Copyright (c) 2014 Gustavo J Knuppe (https://github.com/knuppe)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sub-license, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
// CityHash, by Geoff Pike and Jyrki Alakuijala
//
// Ported to C# by Gustavo J Knuppe (https://github.com/knuppe)
//
//   - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
//   - May you do good and not evil.                                         -
//   - May you find forgiveness for yourself and forgive others.             -
//   - May you share freely, never taking more than you give.                -
//   - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
//
// Project site: https://github.com/knuppe/cityhash
// Original code: https://code.google.com/p/cityhash/
//

using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Kronos.Core.Hashing
{
    /// <summary>
    /// CityHash provides hash functions for strings. The functions mix the
    /// input bits thoroughly but are not suitable for cryptography.
    /// </summary>
    /// <remarks>
    /// This class can be inherited and it exposes some internal functions (if you want to have fun).
    /// More info at the project site: <see href="https://github.com/knuppe/cityhash"/>
    /// </remarks>
    public class CityHash
    {
        // Some primes between 2^63 and 2^64 for various uses.
        private const ulong k0 = 0xc3a5c85c97cb3127UL;
        private const ulong k1 = 0xb492b66fbe98f273UL;
        private const ulong k2 = 0x9ae16a3b2f90404fUL;

        // Magic numbers for 32-bit hashing.  Copied from Murmur3.
        private const uint c1 = 0xcc9e2d51;
        private const uint c2 = 0x1b873593;

        /// <summary>
        /// A 32-bit to 32-bit integer hash copied from Murmur3.
        /// </summary>
        private static uint FMix(uint h)
        {
            h ^= h >> 16;
            h *= 0x85ebca6b;
            h ^= h >> 13;
            h *= 0xc2b2ae35;
            h ^= h >> 16;
            return h;
        }

        private static ulong Rotate(ulong val, int shift)
        {
            // Avoid shifting by 64: doing so yields an undefined result.
            return shift == 0 ? val : ((val >> shift) | (val << (64 - shift)));
        }

        private static uint Rotate32(uint value, int shift)
        {
            return shift == 0 ? value : ((value >> shift) | (value << (32 - shift)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint Mur(uint a, uint h)
        {
            // Helper from Murmur3 for combining two 32-bit values.
            a *= c1;
            a = Rotate32(a, 17);
            a *= c2;
            h ^= a;
            h = Rotate32(h, 19);
            return h * 5 + 0xe6546b64;
        }
        private static uint BSwap32(uint value)
        {
            return
                (value >> 24) |
                (value & 0x00ff0000) >> 8 |
                (value & 0x0000ff00) << 8 |
                (value << 24);
        }

        private static ulong BSwap64(ulong value)
        {
            return
                (value >> 56) |
                (value & 0x00ff000000000000) >> 40 |
                (value & 0x0000ff0000000000) >> 24 |
                (value & 0x000000ff00000000) >> 8 |
                (value & 0x00000000ff000000) << 8 |
                (value & 0x0000000000ff0000) << 24 |
                (value & 0x000000000000ff00) << 40 |
                (value << 56);
        }

        /// <summary>
        /// Returns a 32-bit unsigned integer converted from four bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="index">The starting position within value.</param>
        /// <returns>A 32-bit unsigned integer formed by four bytes beginning at <paramref name="index"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint Fetch32(byte[] value, int index = 0)
        {
            return BitConverter.ToUInt32(value, index);
        }

        /// <summary>
        /// Returns a 32-bit unsigned integer converted from four bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="index">The starting position within value.</param>
        /// <returns>A 32-bit unsigned integer formed by four bytes beginning at <paramref name="index"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint Fetch32(byte[] value, uint index = 0)
        {
            return BitConverter.ToUInt32(value, (int)index);
        }

        /// <summary>
        /// Returns a 64-bit unsigned integer converted from eight bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="index">The starting position within <paramref name="value"/>.</param>
        /// <returns>A 64-bit unsigned integer formed by the eight bytes beginning at <paramref name="index"/>.</returns>
        private static ulong Fetch64(byte[] value, int index = 0)
        {
            return BitConverter.ToUInt64(value, index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint Hash32Len0to4(byte[] value)
        {
            var l = (uint)value.Length;
            var b = 0u;
            var c = 9u;
            for (var i = 0; i < l; i++)
            {
                b = b * c1 + (uint)((sbyte)value[i]);
                c ^= b;
            }

            return FMix(Mur(b, Mur(l, c)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint Hash32Len5to12(byte[] value)
        {
            uint a = (uint)value.Length, b = a * 5, c = 9, d = b;

            a += Fetch32(value, 0);
            b += Fetch32(value, value.Length - 4);
            c += Fetch32(value, (value.Length >> 1) & 4);

            return FMix(Mur(c, Mur(b, Mur(a, d))));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint Hash32Len13to24(byte[] value)
        {
            var a = Fetch32(value, (value.Length >> 1) - 4);
            var b = Fetch32(value, 4);
            var c = Fetch32(value, value.Length - 8);
            var d = Fetch32(value, value.Length >> 1);
            var e = Fetch32(value, 0);
            var f = Fetch32(value, value.Length - 4);
            var h = (uint)value.Length;

            return FMix(Mur(f, Mur(e, Mur(d, Mur(c, Mur(b, Mur(a, h)))))));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Permute3<T>(ref T a, ref T b, ref T c)
        {
            var temp = a;
            a = c; c = b; b = temp;
        }

        private static ulong ShiftMix(ulong val)
        {
            return val ^ (val >> 47);
        }

        private static void Swap<T>(ref T a, ref T b)
        {
            var temp = a;
            a = b;
            b = temp;
        }

        /// <summary>
        /// A subroutine for CityHash128().  Returns a decent 128-bit hash for strings
        /// of any length representable in signed long.  Based on City and Murmur.
        /// </summary>

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static Uint128 CityMurmur(byte[] value, Uint128 seed, int offset)
        {
            var a = seed.Low;
            var b = seed.High;
            ulong c;
            ulong d;

            var len = value.Length - offset;
            var l = len - 16;

            if (l <= 0)
            {  // len <= 16
                a = ShiftMix(a * k1) * k1;
                c = b * k1 + HashLen0to16(value, offset);
                d = ShiftMix(a + (len >= 8 ? Fetch64(value, offset) : c));
            }
            else
            {  // len > 16

                c = HashLen16(Fetch64(value, offset + len - 8) + k1, a);
                d = HashLen16(b + (ulong)len, c + Fetch64(value, offset + len - 16));
                a += d;

                var p = offset;
                do
                {
                    a ^= ShiftMix(Fetch64(value, p) * k1) * k1;
                    a *= k1;
                    b ^= a;
                    c ^= ShiftMix(Fetch64(value, p + 8) * k1) * k1;
                    c *= k1;
                    d ^= c;

                    p += 16;
                    l -= 16;
                } while (l > 0);

            }
            a = HashLen16(a, c);
            b = HashLen16(d, b);
            return new Uint128(a ^ b, HashLen16(b, a));
        }

        /// <summary>
        /// Computes a 32-bit city hash for the specified string.
        /// </summary>
        /// <param name="value">The string value.</param>
        /// <returns>A 32-bit city hash.</returns>
        /// <remarks>This function encodes the string using the unicode block (ISO/IEC 8859-1).</remarks>
        public static uint CityHash32(string value)
        {
            return CityHash32(Encoding.GetEncoding("ISO-8859-1").GetBytes(value));
        }

        /// <summary>
        /// Computes a 32-bit city hash for the given encoded string.
        /// </summary>
        /// <param name="value">The string value.</param>
        /// <returns>A 32-bit city hash.</returns>
        /// <exception cref="System.ArgumentNullException">value</exception>
        /// <remarks>
        /// The city hash is designed to compute hash for STRINGs only!
        /// The city hash "works" with other types of data, but keep in mind it was not built for it.
        /// </remarks>
        protected internal static uint CityHash32(byte[] value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            var len = (uint)value.Length;

            if (len <= 4)
                return Hash32Len0to4(value);

            if (len <= 12)
                return Hash32Len5to12(value);

            if (len <= 24)
                return Hash32Len13to24(value);

            // len > 24
            uint h = len, g = c1 * len, f = g;
            {
                uint a0 = Rotate32(Fetch32(value, len - 4) * c1, 17) * c2;
                uint a1 = Rotate32(Fetch32(value, len - 8) * c1, 17) * c2;
                uint a2 = Rotate32(Fetch32(value, len - 16) * c1, 17) * c2;
                uint a3 = Rotate32(Fetch32(value, len - 12) * c1, 17) * c2;
                uint a4 = Rotate32(Fetch32(value, len - 20) * c1, 17) * c2;

                h ^= a0;
                h = Rotate32(h, 19);
                h = h * 5 + 0xe6546b64;
                h ^= a2;
                h = Rotate32(h, 19);
                h = h * 5 + 0xe6546b64;

                g ^= a1;
                g = Rotate32(g, 19);
                g = g * 5 + 0xe6546b64;
                g ^= a3;
                g = Rotate32(g, 19);
                g = g * 5 + 0xe6546b64;

                f += a4;
                f = Rotate32(f, 19);
                f = f * 5 + 0xe6546b64;
            }

            for (var i = 0; i < (len - 1) / 20; i++)
            {
                var a0 = Rotate32(Fetch32(value, 20 * i) * c1, 17) * c2;
                var a1 = Fetch32(value, 20 * i + 4);
                var a2 = Rotate32(Fetch32(value, 20 * i + 8) * c1, 17) * c2;
                var a3 = Rotate32(Fetch32(value, 20 * i + 12) * c1, 17) * c2;
                var a4 = Fetch32(value, 20 * i + 16);

                h ^= a0;
                h = Rotate32(h, 18);
                h = h * 5 + 0xe6546b64;

                f += a1;
                f = Rotate32(f, 19);
                f = f * c1;

                g += a2;
                g = Rotate32(g, 18);
                g = g * 5 + 0xe6546b64;

                h ^= a3 + a1;
                h = Rotate32(h, 19);
                h = h * 5 + 0xe6546b64;

                g ^= a4;
                g = BSwap32(g) * 5;

                h += a4 * 5;
                h = BSwap32(h);

                f += a0;

                Permute3(ref f, ref h, ref g);
            }

            g = Rotate32(g, 11) * c1;
            g = Rotate32(g, 17) * c1;

            f = Rotate32(f, 11) * c1;
            f = Rotate32(f, 17) * c1;

            h = Rotate32(h + g, 19);
            h = h * 5 + 0xe6546b64;
            h = Rotate32(h, 17) * c1;

            h = Rotate32(h + f, 19);
            h = h * 5 + 0xe6546b64;
            h = Rotate32(h, 17) * c1;

            return h;
        }

        /// <summary>
        /// Computes the 64-bit city hash for the specified string.
        /// </summary>
        /// <param name="value">The string value.</param>
        /// <returns>The 64-bit city hash.</returns>
        /// <remarks>This function encodes the string using the unicode block (ISO/IEC 8859-1).</remarks>
        public static ulong CityHash64(string value)
        {
            return CityHash64(Encoding.GetEncoding("ISO-8859-1").GetBytes(value));
        }

        /// <summary>
        /// Computes the 64-bit city hash for the encoded string.
        /// </summary>
        /// <param name="value">The encoded string.</param>
        /// <returns>The 64-bit city hash.</returns>
        /// <remarks>
        /// The city hash is designed to compute hash for STRINGs only!
        /// The city hash "works" with other types of data, but keep in mind it was not built for it.
        /// </remarks>
        protected internal static ulong CityHash64(byte[] value)
        {
            if (value.Length <= 16)
                return HashLen0to16(value);

            if (value.Length <= 32)
                return HashLen17to32(value);

            if (value.Length <= 64)
                return HashLen33to64(value);

            // For strings over 64 bytes we hash the end first, and then as we
            // loop we keep 56 bytes of state: v, w, x, y, and z.

            var x = Fetch64(value, value.Length - 40);
            var y = Fetch64(value, value.Length - 16) + Fetch64(value, value.Length - 56);
            var z = HashLen16(
                Fetch64(value, value.Length - 48) + (ulong)value.Length,
                Fetch64(value, value.Length - 24));

            var v = WeakHashLen32WithSeeds(value, value.Length - 64, (ulong)value.Length, z);
            var w = WeakHashLen32WithSeeds(value, value.Length - 32, y + k1, x);

            x = x * k1 + Fetch64(value);

            // Decrease len to the nearest multiple of 64, and operate on 64-byte chunks.

            var pos = 0;
            var len = (value.Length) - 1 & ~63;
            do
            {
                x = Rotate(x + y + v.Low + Fetch64(value, pos + 8), 37) * k1;
                y = Rotate(y + v.High + Fetch64(value, pos + 48), 42) * k1;
                x ^= w.High;
                y += v.Low + Fetch64(value, pos + 40);
                z = Rotate(z + w.Low, 33) * k1;
                v = WeakHashLen32WithSeeds(value, pos, v.High * k1, x + w.Low);
                w = WeakHashLen32WithSeeds(value, pos + 32, z + w.High, y + Fetch64(value, pos + 16));
                Swap(ref z, ref x);

                pos += 64;
                len -= 64;
            } while (len != 0);

            return HashLen16(HashLen16(v.Low, w.Low) + ShiftMix(y) * k1 + z, HashLen16(v.High, w.High) + x);
        }

        /// <summary>
        /// Computes the 64-bit city hash for the specified string and seed.
        /// </summary>
        /// <param name="value">The string value.</param>
        /// <param name="seed">The seed used by the algorithm.</param>
        /// <returns>The 64-bit city hash.</returns>
        /// <remarks>This function encodes the string using the unicode block (ISO/IEC 8859-1).</remarks>
        public static ulong CityHash64(string value, ulong seed)
        {
            return CityHash64(Encoding.GetEncoding("ISO-8859-1").GetBytes(value), k2, seed);
        }
        /// <summary>
        /// Computes the 64-bit city hash for the specified string and seed.
        /// </summary>
        /// <param name="value">The string value.</param>
        /// <param name="seed0">The low-order 64-bits seed used by the algorithm.</param>
        /// <param name="seed1">The high-order 64-bits seed used by the algorithm.</param>
        /// <returns>The 64-bit city hash.</returns>
        /// <remarks>This function encodes the string using the unicode block (ISO/IEC 8859-1).</remarks>
        public static ulong CityHash64(string value, ulong seed0, ulong seed1)
        {
            return CityHash64(Encoding.GetEncoding("ISO-8859-1").GetBytes(value), seed0, seed1);
        }

        /// <summary>
        /// Computes the 64-bit city hash for the specified string and seed.
        /// </summary>
        /// <param name="value">The encoded string.</param>
        /// <param name="seed">The seed used by the algorithm.</param>
        /// <returns>The 64-bit city hash.</returns>
        protected static ulong CityHash64(byte[] value, ulong seed)
        {
            return CityHash64(value, k2, seed);
        }

        /// <summary>
        /// Computes the 64-bit city hash for the specified string and seed.
        /// </summary>
        /// <param name="value">The encoded string.</param>
        /// <param name="seed0">The low-order 64-bits seed used by the algorithm.</param>
        /// <param name="seed1">The high-order 64-bits seed used by the algorithm.</param>
        /// <returns>The 64-bit city hash.</returns>
        protected static ulong CityHash64(byte[] value, ulong seed0, ulong seed1)
        {
            return HashLen16(CityHash64(value) - seed0, seed1);
        }

        /// <summary>
        /// Computes the 128-bit city hash and are tuned for strings of at least a few hundred bytes.
        /// </summary>
        /// <param name="value">The string value.</param>
        /// <returns>The 128-bit city hash.</returns>
        /// <remarks>This function encodes the string using the unicode block (ISO/IEC 8859-1).</remarks>
        public static Uint128 CityHash128(string value)
        {
            return CityHash128(Encoding.GetEncoding("ISO-8859-1").GetBytes(value));
        }

        /// <summary>
        /// Computes the 128-bit city hash and are tuned for strings of at least a few hundred bytes using
        /// the specified <paramref name="seed"/>.
        /// </summary>
        /// <param name="value">The string value.</param>
        /// <param name="seed">The seed used by the city hash algorithm.</param>
        /// <returns>The 128-bit city hash.</returns>
        /// <remarks>This function encodes the string using the unicode block (ISO/IEC 8859-1).</remarks>
        public static Uint128 CityHash128(string value, Uint128 seed)
        {
            return CityHash128(Encoding.GetEncoding("ISO-8859-1").GetBytes(value), seed, 0);
        }

        /// <summary>
        /// Computes a 128-bit city hash for the given encoded string.
        /// </summary>
        /// <param name="value">The encoded string.</param>
        /// <returns>The 128-bit city hash.</returns>
        /// <remarks>
        /// The city hash is designed to compute hash for STRINGs only!
        /// The city hash "works" with other types of data, but keep in mind it was not built for it.
        /// </remarks>
        protected static Uint128 CityHash128(byte[] value)
        {
            return value.Length >= 16
                ? CityHash128(value, new Uint128(Fetch64(value), Fetch64(value, 8) + k0), 16)
                : CityHash128(value, new Uint128(k0, k1), 0);
        }

        /// <summary>
        /// Computes the 128-bit city hash and are tuned for strings of at least a few hundred bytes using
        /// the specified <paramref name="seed"/> starting at a <paramref name="offset"/> position.
        /// </summary>
        /// <param name="value">The encoded string.</param>
        /// <param name="seed">The seed used by the hash alrorithm.</param>
        /// <param name="offset">The offset position in the byte array.</param>
        /// <returns>The 128-bit city hash.</returns>
        /// <remarks>
        /// The city hash is designed to compute hash for STRINGs only!
        /// The city hash "works" with other types of data, but keep in mind it was not built for it.
        /// </remarks>
        protected static Uint128 CityHash128(byte[] value, Uint128 seed, int offset)
        {
            if (value.Length - offset < 128)
                return CityMurmur(value, seed, offset);

            // We expect len >= 128 to be the common case.  Keep 56 bytes of state:
            // v, w, x, y, and z.
            var len = value.Length - offset;
            var x = seed.Low;
            var y = seed.High;
            var z = (ulong)len * k1;
            var v = new Uint128
            {
                Low = Rotate(seed.High ^ k1, 49) * k1 + Fetch64(value, offset)
            };
            v.High = Rotate(v.Low, 42) * k1 + Fetch64(value, offset + 8);

            var w = new Uint128
            {
                Low = Rotate(y + z, 35) * k1 + x,
                High = Rotate(seed.Low + Fetch64(value, offset + 88), 53) * k1
            };


            // This is the same inner loop as CityHash64(), manually unrolled.
            var s = offset;
            do
            {
                x = Rotate(x + y + v.Low + Fetch64(value, s + 8), 37) * k1;
                y = Rotate(y + v.High + Fetch64(value, s + 48), 42) * k1;
                x ^= w.High;
                y += v.Low + Fetch64(value, s + 40);
                z = Rotate(z + w.Low, 33) * k1;
                v = WeakHashLen32WithSeeds(value, s, v.High * k1, x + w.Low);
                w = WeakHashLen32WithSeeds(value, s + 32, z + w.High, y + Fetch64(value, s + 16));

                Swap(ref z, ref x);

                s += 64;

                x = Rotate(x + y + v.Low + Fetch64(value, s + 8), 37) * k1;
                y = Rotate(y + v.High + Fetch64(value, s + 48), 42) * k1;
                x ^= w.High;
                y += v.Low + Fetch64(value, s + 40);
                z = Rotate(z + w.Low, 33) * k1;
                v = WeakHashLen32WithSeeds(value, s, v.High * k1, x + w.Low);
                w = WeakHashLen32WithSeeds(value, s + 32, z + w.High, y + Fetch64(value, s + 16));

                Swap(ref z, ref x);

                s += 64;
                len -= 128;
            } while (len >= 128);

            x += Rotate(v.Low + z, 49) * k0;
            y = y * k0 + Rotate(w.High, 37);
            z = z * k0 + Rotate(w.Low, 27);
            w.Low *= 9;
            v.Low *= k0;

            // If 0 < len < 128, hash up to 4 chunks of 32 bytes each from the end of s.
            for (var tail = 0; tail < len;)
            {
                tail += 32;

                y = Rotate(x + y, 42) * k0 + v.High;
                w.Low += Fetch64(value, s + len - tail + 16);
                x = x * k0 + w.Low;
                z += w.High + Fetch64(value, s + len - tail);
                w.High += v.Low;
                v = WeakHashLen32WithSeeds(value, s + len - tail, v.Low + z, v.High);
                v.Low *= k0;
            }


            // At this point our 56 bytes of state should contain more than
            // enough information for a strong 128-bit hash.  We use two
            // different 56-byte-to-8-byte hashes to get a 16-byte final result.
            x = HashLen16(x, v.Low);
            y = HashLen16(y + z, w.Low);

            return new Uint128
            {
                Low = HashLen16(x + v.High, w.High) + y,
                High = HashLen16(x + w.High, y + v.High)
            };
        }

        private static ulong HashLen16(ulong u, ulong v)
        {
            return Hash128to64(new Uint128(u, v));
        }

        private static ulong HashLen16(ulong u, ulong v, ulong mul)
        {
            // Murmur-inspired hashing.
            var a = (u ^ v) * mul;
            a ^= (a >> 47);
            var b = (v ^ a) * mul;
            b ^= (b >> 47);
            b *= mul;
            return b;
        }

        private static ulong HashLen0to16(byte[] value, int offset = 0)
        {
            var len = (uint)(value.Length - offset);

            if (len >= 8)
            {

                var mul = k2 + (ulong)len * 2;
                var a = Fetch64(value, offset) + k2;
                var b = Fetch64(value, value.Length - 8);
                var c = Rotate(b, 37) * mul + a;
                var d = (Rotate(a, 25) + b) * mul;

                return HashLen16(c, d, mul);
            }

            if (len >= 4)
            {
                var mul = k2 + (ulong)len * 2;
                ulong a = Fetch32(value, offset);
                return HashLen16(len + (a << 3), Fetch32(value, (int)(offset + len - 4)), mul);
            }

            if (len > 0)
            {
                var a = value[offset];
                var b = value[offset + (len >> 1)];
                var c = value[offset + (len - 1)];

                var y = a + ((uint)b << 8);
                var z = len + ((uint)c << 2);

                return ShiftMix((y * k2 ^ z * k0)) * k2;
            }

            return k2;
        }

        /// <summary>
        /// This probably works well for 16-byte strings as well, but it may be overkill in that case.
        /// </summary>
        private static ulong HashLen17to32(byte[] value)
        {
            var len = (ulong)value.Length;

            var mul = k2 + len * 2ul;
            var a = Fetch64(value) * k1;
            var b = Fetch64(value, 8);
            var c = Fetch64(value, value.Length - 8) * mul;
            var d = Fetch64(value, value.Length - 16) * k2;

            return HashLen16(Rotate(a + b, 43) + Rotate(c, 30) + d, a + Rotate(b + k2, 18) + c, mul);
        }

        /// <summary>
        /// Return an 8-byte hash for 33 to 64 bytes.
        /// </summary>
        private static ulong HashLen33to64(byte[] value)
        {
            var mul = k2 + (ulong)value.Length * 2ul;
            var a = Fetch64(value) * k2;
            var b = Fetch64(value, 8);
            var c = Fetch64(value, value.Length - 24);
            var d = Fetch64(value, value.Length - 32);
            var e = Fetch64(value, 16) * k2;
            var f = Fetch64(value, 24) * 9;
            var g = Fetch64(value, value.Length - 8);
            var h = Fetch64(value, value.Length - 16) * mul;

            var u = Rotate(a + g, 43) + (Rotate(b, 30) + c) * 9;
            var v = ((a + g) ^ d) + f + 1;
            var w = BSwap64((u + v) * mul) + h;
            var x = Rotate(e + f, 42) + c;
            var y = (BSwap64((v + w) * mul) + g) * mul;
            var z = e + f + c;

            a = BSwap64((x + z) * mul + y) + b;
            b = ShiftMix((z + a) * mul + d + h) * mul;
            return b + x;
        }

        private static ulong Hash128to64(Uint128 x)
        {
            const ulong kMul = 0x9ddfea08eb382d69UL;

            var a = (x.Low ^ x.High) * kMul;
            a ^= (a >> 47);

            var b = (x.High ^ a) * kMul;
            b ^= (b >> 47);
            b *= kMul;

            return b;
        }

        /// <summary>
        /// Return a 16-byte hash for 48 bytes. Quick and dirty.
        /// Callers do best to use "random-looking" values for a and b.
        /// </summary>
        private static Uint128 WeakHashLen32WithSeeds(ulong w, ulong x, ulong y, ulong z, ulong a, ulong b)
        {
            a += w;
            b = Rotate(b + a + z, 21);

            var c = a;
            a += x;
            a += y;

            b += Rotate(a, 44);

            return new Uint128(a + z, b + c);
        }

        private static Uint128 WeakHashLen32WithSeeds(byte[] value, int offset, ulong a, ulong b)
        {
            return WeakHashLen32WithSeeds(
                Fetch64(value, offset),
                Fetch64(value, offset + 8),
                Fetch64(value, offset + 16),
                Fetch64(value, offset + 24),
                a,
                b);
        }
    }
}