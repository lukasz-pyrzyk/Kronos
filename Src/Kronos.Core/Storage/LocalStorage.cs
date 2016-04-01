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

        private FileStream _indexFile;
        private FileStream _storageFile;

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public int Count => _indexes.Count;

        public LocalStorage()
        {
            InitializeStorageFolder();
            InitializeIndex();
            InitializeStorage();
        }

        public void AddOrUpdate(string key, byte[] obj)
        {
            var index = new RowInfo(key, obj.Length, _storageFile.Position);

            _indexes[key] = index;

            byte[] indexBytes = index.GetBytesForFile();
            _indexFile.Write(indexBytes, 0, indexBytes.Length);
            _indexFile.Flush();

            _storageFile.Write(obj, 0, obj.Length);
            _storageFile.Flush();
        }

        public byte[] TryGet(string key)
        {
            RowInfo row;
            if (_indexes.TryGetValue(key, out row))
            {
                byte[] buffer = new byte[row.Length];
                _storageFile.Seek(row.Offset, SeekOrigin.Begin);
                _storageFile.Read(buffer, 0, row.Length);
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
            Dispose();

            _logger.Info($"Deleting index file {_indexFile}");
            File.Delete(_indexFilePath);

            _logger.Info($"Deleting storage file {_storageFilePath}");
            File.Delete(_storageFilePath);
        }

        public void Dispose()
        {
            _logger.Info("Disposing storage");
            _storageFile.Dispose();
            _indexFile.Dispose();

            _storageFile = null;
            _indexFile = null;

            _logger.Info("Storage disposed");
        }

        private void InitializeIndex()
        {
            _indexFile = OpenOrCreateFile(_indexFilePath);

            StreamReader reader = new StreamReader(_indexFile);

            string line;
            do
            {
                line = reader.ReadLine();
                if (line != null)
                {
                    RowInfo row = new RowInfo(line);
                    _indexes[row.Key] = row;Disposable on 
                }
            } while (line != null);

            _logger.Info($"Index file has beed initialized. Size: {_indexFile.Length}, Position: {_indexFile.Position}");
            _logger.Info($"Loaded {_indexes.Count} keys");
        }

        private void InitializeStorage()
        {
            _storageFile = OpenOrCreateFile(_storageFilePath);
            _storageFile.Seek(_storageFile.Length, SeekOrigin.Begin);

            _logger.Info($"Storage file has beed initialized. Size: {_storageFile.Length}, Position: {_storageFile.Position}");
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
            return File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
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

