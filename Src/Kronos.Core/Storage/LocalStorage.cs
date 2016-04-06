using System.Collections.Generic;
using System.IO;
using NLog;

namespace Kronos.Core.Storage
{
    public class LocalStorage : IStorage
    {
        public static readonly string StorageFolder = ".\\data";
        public static readonly string StorageFilePath = $"{StorageFolder}\\blob.data";
        public static readonly string IndexFilePath = $"{StorageFolder}\\index.data";

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<string, RowInfo> _indexes = new Dictionary<string, RowInfo>();

        private FileStream _indexFile;
        private FileStream _storageFile;

        public LocalStorage()
        {
            InitializeStorageFolder();
            InitializeIndex();
            InitializeStorage();
        }

        public int Count => _indexes.Count;

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
            File.Delete(IndexFilePath);

            _logger.Info($"Deleting storage file {StorageFilePath}");
            File.Delete(StorageFilePath);
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
            _indexFile = OpenOrCreateFile(IndexFilePath);

            StreamReader reader = new StreamReader(_indexFile);

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

            _logger.Info($"Index file has beed initialized. Size: {_indexFile.Length}, Position: {_indexFile.Position}");
            _logger.Info($"Loaded {_indexes.Count} keys");
        }

        private void InitializeStorage()
        {
            _storageFile = OpenOrCreateFile(StorageFilePath);
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
    }
}