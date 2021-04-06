namespace Micro.Net.Storage.FileSystem
{
    public class FileSystemSagaPersistenceConfiguration
    {
        public string StoragePath { get; set; }
        public string NamePattern { get; set; }
        //public string LogPath { get; set; }
        public bool KeepProcessed { get; set; }
        public string ProcessedPath { get; set; }
        //public bool KeepCorrupted { get; set; }
        //public string CorruptedPath { get; set; }
        public string Serializer { get; set; }
    }
}