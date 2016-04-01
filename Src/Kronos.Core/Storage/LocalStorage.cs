using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NLog;

namespace Kronos.Core.Storage
{
    public class LocalStorage : IStorage
    {
        private const string StorageFolder = ".\\data";
        private readonly string _storageFilePath = $"{StorageFolder}\\blob.data";
        private readonly string _indexFilePath = $"{StorageFolder}\\index.data";

        private readonly Dictionary<string, RowInfo> _indexes = new Dictionary<string, RowInfo>();
        private readonly FileStream _indexStorage;

        private readonly FileStream _storage;

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public int Count { get; }

        public LocalStorage()
        {
            InitializeStorageFolder();

            _indexStorage = OpenOrCreateFile(_indexFilePath);
            _storage = OpenOrCreateFile(_storageFilePath);

            InitializeIndexes();

            _indexStorage.Seek(_indexStorage.Length, SeekOrigin.Begin);
            _storage.Seek(_storage.Length, SeekOrigin.Begin);
        }

        private void InitializeIndexes()
        {
            StreamReader reader = new StreamReader(_indexStorage);

            string line;
            do
            {
                line = reader.ReadLine();
                if (line != null)
                {
                    RowInfo row = new RowInfo(line);
                    _indexes[row.Key] = row;
                }
            } while (line != null);
        }

        private void InitializeStorageFolder()
        {
            if (!Directory.Exists(StorageFolder))
            {
                _logger.Info("Data directory does not exist. Creating...");
                Directory.CreateDirectory(StorageFolder);
            }
        }

        private static FileStream OpenOrCreateFile(string path)
        {
            return File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite); ;
        }

        public void AddOrUpdate(string key, byte[] obj)
        {
            var index = new RowInfo(key, obj.Length, _storage.Position);

            _indexes[key] = index;

            byte[] indexBytes = index.GetBytesForFile();
            _indexStorage.Write(indexBytes, 0, indexBytes.Length);
            _indexStorage.Flush();

            _storage.Write(obj, 0, obj.Length);
            _storage.Flush();
        }

        public byte[] TryGet(string key)
        {
            RowInfo row;
            if (_indexes.TryGetValue(key, out row))
            {
                byte[] buffer = new byte[row.Length];
                _storage.Seek(row.Offset, SeekOrigin.Begin);
                _storage.Read(buffer, 0, row.Length);
                return buffer;
            }

            return null;
        }

        public bool TryRemove(string key)
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

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
}
