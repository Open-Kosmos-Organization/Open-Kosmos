using Unity.Entities;

namespace Kosmos.Prototypes.Time.Components
{
    public struct UniversalTime : IComponentData
    {
        public double Value;
    }
    
    public struct UniversalTimeModifier : IComponentData
    {
        public double Value;
    }
    
    public struct UniversalTimePaused : IComponentData
    {
        public bool Value;
    }
    
    public struct IsCurrentPlayerTimelineTag : IComponentData
    {
    }
}
