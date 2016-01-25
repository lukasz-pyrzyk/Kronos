using System;
using System.Linq;
using Kronos.Core.Requests;
using Xunit;

namespace Kronos.Core.Tests.Requests
{
    public class RequestTests
    {
        [Fact]
        public void CanSerializeAndDeserializeString()
        {
            string lorem = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod";

            var request = new FakeRequest();
            byte[] array = request.GetBytes(lorem);
            string loremFromArray = request.GetString(array);

            Assert.Equal(lorem, loremFromArray);
        }

        [Fact]
        public void CanSerializeAndDeserializeInt()
        {
            int value = int.MaxValue;

            var request = new FakeRequest();
            byte[] array = request.GetBytes(value);
            int valueFromArray = request.GetInt(array);

            Assert.Equal(value, valueFromArray);
        }


        [Fact]
        public void CanSerializeAndDeserializeLong()
        {
            long value = long.MaxValue;

            var request = new FakeRequest();
            byte[] array = request.GetBytes(value);
            long valueFromArray = request.GetLong(array);

            Assert.Equal(value, valueFromArray);
        }


        [Fact]
        public void CanSerializeAndDeserializeDatetime()
        {
            DateTime date = DateTime.Now;

            var request = new FakeRequest();
            byte[] array = request.GetBytes(date);
            DateTime dateFromArray = request.GetDatetime(array);

            Assert.Equal(date, dateFromArray);
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

            public byte[] GetBytes(long value)
            {
                return Serialize(value);
            }

            public byte[] GetBytes(string word)
            {
                return Serialize(word);
            }

            public byte[] GetBytes(DateTime datetime)
            {
                return Serialize(datetime);
            }

            public int GetInt(byte[] stream)
            {
                return DeserializeInt(stream);
            }

            public long GetLong(byte[] stream)
            {
                return DeserializeLong(stream);
            }

            public string GetString(byte[] stream)
            {
                return DeserializeString(stream);
            }

            public DateTime GetDatetime(byte[] stream)
            {
                return DeseriazeDatetime(stream);
            }

            public byte[] Join(params byte[][] bytes)
            {
                return JoinWithTotalSize(bytes);
            }
        }
    }
}
