using Micro.Net.Abstractions;

namespace Micro.Net.Dispatch
{
    public class DispatcherFeature : Feature
    {
        public DispatcherFeature(int id, string name) : base(id, name)
        {
        }

        public static DispatcherFeature Replies => new DispatcherFeature(1, nameof(Replies));
        public static DispatcherFeature Outbox => new DispatcherFeature(2, nameof(Outbox));
        public static DispatcherFeature Sync => new DispatcherFeature(3, nameof(Sync));
        public static DispatcherFeature Async => new DispatcherFeature(4, nameof(Async));

    }
}