namespace Micro.Net.Abstractions.Components
{
    public abstract class ComponentKind
    {
        public abstract class Transport : ComponentKind
        {
            public abstract class Dispatch : ComponentKind {}
            public abstract class Receive : ComponentKind {}
        }
        public abstract class Storage : ComponentKind {}
        public abstract class Cache : ComponentKind {}
        public abstract class Discovery : ComponentKind {}
    }
}