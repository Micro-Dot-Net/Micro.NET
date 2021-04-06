using System;
using System.ComponentModel;
using System.IO;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Micro.Net.Abstractions;
using Micro.Net.Abstractions.Sagas;
using Micro.Net.Abstractions.Storage;
using Micro.Net.Core.Extensions;
using Micro.Net.Core.Storage;
using SmartFormat;

namespace Micro.Net.Storage.FileSystem
{
    public class FileSystemSagaPersistenceProvider<TData> : ISagaPersistenceProvider<TData> where TData : class, ISagaData
    {
        private readonly FileSystemSagaPersistenceConfiguration _config;
        private readonly ISerializerCollection _serializerCollection;

        internal FileSystemSagaPersistenceProvider(FileSystemSagaPersistenceConfiguration config, ISerializerCollection serializerCollection)
        {
            _config = config;
            _serializerCollection = serializerCollection;
        }

        [ThreadStatic]
        private FileStream _fs = null;

        public async Task<TData> Get(SagaKey key)
        {
            string file = Smart.Format(_config.NamePattern, new {Key = key, SagaData = typeof(TData).Name});

            file = Path.Combine(_config.StoragePath, file);

            ISerializer serializer = string.IsNullOrWhiteSpace(_config.Serializer)
                ? _serializerCollection.Get(_config.Serializer)
                : _serializerCollection.Default;

            _fs = FileExtensions.WaitForFile(file, FileMode.Create, FileAccess.ReadWrite, FileShare.None);

            _fs.Seek(0, SeekOrigin.Begin);

            TData data;

            using (StreamReader sr = new StreamReader(_fs))
            {
                data = serializer.Materialize<TData>(await sr.ReadToEndAsync());
            }

            if (Transaction.Current != null)
            {
                Transaction.Current.TransactionCompleted += (sender, args) => { _fs?.Dispose(); };
            }
            else
            {
                _fs?.Dispose();
                _fs = null;
            }

            return data;
        }

        public async Task Save(TData obj)
        {
            string file = Smart.Format(_config.NamePattern, new { Key = obj.Key, SagaData = typeof(TData).Name });

            file = Path.Combine(_config.StoragePath, file);

            ISerializer serializer = string.IsNullOrWhiteSpace(_config.Serializer)
                ? _serializerCollection.Get(_config.Serializer)
                : _serializerCollection.Default;

            _fs ??= FileExtensions.WaitForFile(file, FileMode.Create, FileAccess.ReadWrite, FileShare.None);

            _fs.Seek(0, SeekOrigin.Begin);

            string data = serializer.Serialize(obj);

            _fs.SetLength(data.Length);

            using (StreamWriter sw = new StreamWriter(_fs, Encoding.UTF8, 512, false))
            {
                await sw.WriteAsync(data);
            }

            if (Transaction.Current != null)
            {
                Transaction.Current.TransactionCompleted += (sender, args) => { _fs?.Dispose(); };
            }
            else
            {
                _fs?.Dispose();
                _fs = null;
            }
        }

        public async Task Complete(SagaKey key)
        {
            string file = Smart.Format(_config.NamePattern, new { Key = key, SagaData = typeof(TData).Name });

            string filePath = Path.Combine(_config.StoragePath, file);

            ISerializer serializer = string.IsNullOrWhiteSpace(_config.Serializer)
                ? _serializerCollection.Get(_config.Serializer)
                : _serializerCollection.Default;

            _fs ??= FileExtensions.WaitForFile(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None);

            _fs.Seek(0, SeekOrigin.Begin);

            using (FileStream cfs = new FileStream(Path.Combine(_config.ProcessedPath, file), FileMode.CreateNew,
                FileAccess.ReadWrite, FileShare.None))
            {
                using(StreamWriter sw = new StreamWriter(cfs))
                {
                    using (StreamReader sr = new StreamReader(_fs))
                    {
                        await sw.WriteAsync(await sr.ReadToEndAsync());
                    }
                }

                _fs.Close();
                File.Delete(_fs.Name);
            }

            if (Transaction.Current != null)
            {
                Transaction.Current.TransactionCompleted += (sender, args) => { _fs?.Dispose(); };
            }
            else
            {
                _fs?.Dispose();
                _fs = null;
            }
        }
    }
}
