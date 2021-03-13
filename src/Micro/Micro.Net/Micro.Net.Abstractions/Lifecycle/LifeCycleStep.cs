using Micro.Net.Abstractions.Components;

namespace Micro.Net.Abstractions.Lifecycle
{
    public abstract class LifeCycleStep
    {
        public abstract class PlatformInitialize : LifeCycleStep { }
        public abstract class PlatformShutdown : LifeCycleStep { }
        public abstract class ContainerInitialize : LifeCycleStep { }
        public abstract class ComponentsInitialize : LifeCycleStep { }
        public abstract class ComponentsShutdown : LifeCycleStep { }
        public abstract class ComponentsInitialize<TComponent> : LifeCycleStep where TComponent : ComponentKind { }
        public abstract class ComponentsShutdown<TComponent> : LifeCycleStep where TComponent : ComponentKind { }
        public abstract class ComponentInitialize<TComponent> : LifeCycleStep where TComponent : ComponentKind { }
        public abstract class ComponentShutdown<TComponent> : LifeCycleStep where TComponent : ComponentKind { }
    }
}