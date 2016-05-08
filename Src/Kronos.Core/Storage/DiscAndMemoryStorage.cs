using System;
using System.Collections.Generic;
using System.IO;
using NLog;

namespace Kronos.Core.Storage
{
    public class DiscAndMemoryStorage : IStorage
    {
        private const string FileExtension = "dat";
        public static readonly string StorageFolder = ".\\data";
        public static readonly string IndexFilePath = $"{StorageFolder}\\index.{FileExtension}";

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<string, string> _indexes = new Dictionary<string, string>();
        private readonly Action<string, byte[]> _createFile;
        private readonly Func<string, byte[]> _readFile;
        private readonly Action<string> _deleteFile;

        public DiscAndMemoryStorage() : this(File.WriteAllBytes, File.ReadAllBytes, File.Delete)
        {
        }

        internal DiscAndMemoryStorage(
            Action<string, byte[]> createFile,
            Func<string, byte[]> readFile,
            Action<string> deleteFile)
        {
            _createFile = createFile;
            _readFile = readFile;
            _deleteFile = deleteFile;

            InitializeStorageFolder();
        }

        public int Count => _indexes.Count;

        public void AddOrUpdate(string key, byte[] obj)
        {
            _logger.Debug($"Adding key {key} with {obj} bytes");
            string fileName = Guid.NewGuid().ToString();

            // add file to index dictionary
            _indexes[key] = fileName;

            try
            {
                // save file into disc
                _createFile($@"{StorageFolder}\{fileName}.{FileExtension}", obj);
            }
            catch (IOException ex)
            {
                _logger.Error(ex);
                _indexes.Remove(key);
            }
        }

        public byte[] TryGet(string key)
        {
            _logger.Debug($"Returning key {key}");
            try
            {
                string fileName;
                if (_indexes.TryGetValue(key, out fileName))
                {
                    byte[] bytes = _readFile($@"{StorageFolder}\{fileName}.{FileExtension}");
                    _logger.Debug($"File with key {key} found, returning {bytes} bytes");
                    return bytes;
                }

                _logger.Debug($"Key {key} not found");
            }
            catch (IOException ex)
            {
                _logger.Error(ex);
            }

            return null;
        }

        public bool TryRemove(string key)
        {
            _logger.Debug($"Removing key {key}");
            try
            {
                string fileName;
                if (_indexes.TryGetValue(key, out fileName))
                {
                    _deleteFile($@"{StorageFolder}\{fileName}.{FileExtension}");
                    _logger.Debug($"Key {key} has been deleted");
                    return true;
                }
            }
            catch (IOException ex)
            {
                _logger.Error(ex);
            }

            return false;
        }

        public void Clear()
        {
            _indexes.Clear();
            Dispose();

            _deleteFile(IndexFilePath);
        }

        public void Dispose()
        {
            _logger.Info("Storage disposed");
        }

        private void InitializeStorageFolder()
        {
            if (Directory.Exists(StorageFolder))
            {
                _logger.Info("Data folder exists. Deleting...");
                Directory.Delete(StorageFolder);
            }

            _logger.Info("Creating empty file for storage");
            Directory.CreateDirectory(StorageFolder);
        }
    }
}