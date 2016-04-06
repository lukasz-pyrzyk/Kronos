using System;
using System.Text;
using Kronos.Core.Storage;
using Xunit;

namespace Kronos.Core.Tests.Storage
{
    public class RowInfoTests
    {
        [Fact]
        public void Ctor_InitializeProperties()
        {
            const string key = "lorem ipsum";
            const int length = int.MaxValue;
            const long offset = long.MaxValue;

            RowInfo row = new RowInfo(key, length, offset);

            Assert.Equal(row.Key, key);
            Assert.Equal(row.Length, length);
            Assert.Equal(row.Offset, offset);
        }

        [Fact]
        public void Ctor_SplitsLineToProperties()
        {
            const string key = "lorem ipsum";
            const int length = int.MaxValue;
            const long offset = long.MaxValue;

            string line = $"{key};{length};{offset}{Environment.NewLine}";

            RowInfo row = new RowInfo(line);

            Assert.Equal(row.Key, key);
            Assert.Equal(row.Length, length);
            Assert.Equal(row.Offset, offset);
        }

        [Fact]
        public void GetHashCode_ReturnsKeyHashcode()
        {
            const string key = "lorem ipsum";
            const int length = int.MaxValue;
            const long offset = long.MaxValue;

            RowInfo row = new RowInfo(key, length, offset);

            int hashcode = row.GetHashCode();
            Assert.Equal(hashcode, key.GetHashCode());
        }

        [Fact]
        public void ToString_ReturnsKey()
        {
            const string key = "lorem ipsum";
            const int length = int.MaxValue;
            const long offset = long.MaxValue;

            RowInfo row = new RowInfo(key, length, offset);
            
            string toStringValue = row.ToString();
            Assert.Equal(toStringValue, key);
        }

        [Fact]
        public void GetBytesForFile_ReturnsCorrectSerializedProperties()
        {
            const string key = "lorem ipsum";
            const int length = int.MaxValue;
            const long offset = long.MaxValue;

            RowInfo row = new RowInfo(key, length, offset);
            byte[] lineBytes = row.GetBytesForFile();
            string line = Encoding.UTF8.GetString(lineBytes);

            RowInfo deserializedRow = new RowInfo(line);

            Assert.Equal(deserializedRow.Key, key);
            Assert.Equal(deserializedRow.Length, length);
            Assert.Equal(deserializedRow.Offset, offset);
        }
    }
}
