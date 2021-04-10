using System;

namespace Micro.Net.Transport.FileSystem
{
    public class MessageProcessConfiguration
    {
        public Type ResponseType { get; set; }
        public string RequestDir { get; set; }
        public string ResponseDir { get; set; }
        public string RequestFilter { get; set; }
        public string RequestSerializer { get; set; }
        public string ResponseSerializer { get; set; }
        public bool KeepSkips { get; set; }
        public string SkipDirectory { get; set; }
        public bool KeepProcessed { get; set; }
        public string ProcessedDirectory { get; set; }
    }
}