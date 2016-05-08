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

        private readonly Action<string, byte[]> _fileCreate;
        private readonly Func<string, byte[]> _fileRead;
        private readonly Action<string> _fileDelete;
        private readonly Func<string, bool> _directoryExists;
        private readonly Func<string, DirectoryInfo> _directoryCreate;
        private readonly Action<string> _directoryDelete;

        public DiscAndMemoryStorage() : this(
            File.WriteAllBytes,
            File.ReadAllBytes,
            File.Delete,
            Directory.Exists,
            Directory.CreateDirectory,
            Directory.Delete)
        {
        }

        internal DiscAndMemoryStorage(
            Action<string, byte[]> fileCreate,
            Func<string, byte[]> fileRead,
            Action<string> fileDelete,
            Func<string, bool> directoryExists,
            Func<string, DirectoryInfo> directoryCreate,
            Action<string> directoryDelete)
        {
            _fileCreate = fileCreate;
            _fileRead = fileRead;
            _fileDelete = fileDelete;
            _directoryExists = directoryExists;
            _directoryCreate = directoryCreate;
            _directoryDelete = directoryDelete;

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
                _fileCreate($@"{StorageFolder}\{fileName}.{FileExtension}", obj);
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
                    byte[] bytes = _fileRead($@"{StorageFolder}\{fileName}.{FileExtension}");
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
                    _fileDelete($@"{StorageFolder}\{fileName}.{FileExtension}");
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

            _fileDelete(IndexFilePath);
        }

        public void Dispose()
        {
            _logger.Info("Storage disposed");
        }

        private void InitializeStorageFolder()
        {
            if (_directoryExists(StorageFolder))
            {
                _logger.Info("Data folder exists. Deleting...");
                _directoryDelete(StorageFolder);
            }

            _logger.Info("Creating empty file for storage");
            _directoryCreate(StorageFolder);
        }
    }
}