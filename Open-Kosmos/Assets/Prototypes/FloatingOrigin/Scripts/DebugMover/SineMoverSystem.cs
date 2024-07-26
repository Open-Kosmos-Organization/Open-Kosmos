using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Kosmos.Prototypes.FloatingOrigin.DebugMover
{
    public partial class SineMoverSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((ref LocalTransform transform, in SineMover sineMover) =>
            {
                var multipliedTime = SystemAPI.Time.ElapsedTime * sineMover.Speed;
                transform.Position.y += sineMover.Amplitude * (float)math.sin(multipliedTime);
            }).Schedule();
        }
    }
}