using Unity.Collections;
using Unity.Entities;

namespace Kosmos.Prototypes.Staging.Components
{
    public struct Stage : IBufferElementData
    {
        public NativeArray<StagePart> Parts;
    }
    
    public struct StagePart
    {
        public Entity Value;
    }
}