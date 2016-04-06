using System;
using System.Text;

namespace Kronos.Core.Storage
{
    internal class RowInfo
    {
        public RowInfo(string key, int length, long offset)
        {
            Key = key;
            Length = length;
            Offset = offset;
        }

        public RowInfo(string line)
        {
            string[] splitted = line.Split(';');
            Key = splitted[0];
            Length = Convert.ToInt32(splitted[1]);
            Offset = Convert.ToInt64(splitted[2]);
        }

        public string Key { get; }
        public int Length { get; }
        public long Offset { get; }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        public override string ToString()
        {
            return Key;
        }

        public byte[] GetBytesForFile()
        {
            string line = $"{Key};{Length};{Offset}{Environment.NewLine}";
            return Encoding.UTF8.GetBytes(line);
        }
    }
}
