using System;
using System.Linq;
using System.Text;
using Kronos.Core.Requests;
using Xunit;

namespace Kronos.Core.Tests.Requests
{
    public class RequestTests
    {
        
        [Fact]
        public void CanGetCorrectByteArrayFromString()
        {
            string lorem = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod";

            byte[] array = new FakeRequest().GetBytes(lorem);
            string loremFromArray = Encoding.UTF8.GetString(array);

            Assert.Equal(lorem, loremFromArray);
        }

        [Fact]
        public void CanGetCorrectByteArrayFromDatetime()
        {
            DateTime now = DateTime.Now;

            byte[] array = new FakeRequest().GetBytes(now);
            long ticks = BitConverter.ToInt64(array, 0);

            DateTime nowFromArray = DateTime.FromBinary(ticks);

            Assert.Equal(now, nowFromArray);
        }

        [Theory]
        [InlineData(1, 551)]
        [InlineData(25151, 12312)]
        public void CanCorrectJoinByteArrays(int first, int second)
        {
            byte[] bytesOne = new byte[first];
            for (int i = 0; i < bytesOne.Length; i++)
            {
                bytesOne[i] = Byte.MaxValue;
            }

            byte[] bytesTwo = new byte[second];
            for (int i = 0; i < bytesOne.Length; i++)
            {
                bytesOne[i] = Byte.MinValue;
            }

            byte[] joined = new FakeRequest().Join(bytesOne, bytesTwo).Skip(sizeof(int)).ToArray();

            Assert.Equal(joined.Length, first + second);

            for (int i = 0; i < first; i++)
            {
                Assert.Equal(joined[i], bytesOne[i]);
            }

            for (int i = 0; i < second; i++)
            {
                Assert.Equal(joined[i + first], bytesTwo[i]);
            }

        }

        internal class FakeRequest : Request
        {
            public override byte[] Serialize()
            {
                return new byte[0];
            }

            public new byte[] GetBytes(string word)
            {
                return base.Serialize(word);
            }

            public new byte[] GetBytes(DateTime datetime)
            {
                return base.Serialize(datetime);
            }

            public new byte[] Join(params byte[][] bytes)
            {
                return base.JoinWithTotalSize(bytes);
            }
        }
    }
}
