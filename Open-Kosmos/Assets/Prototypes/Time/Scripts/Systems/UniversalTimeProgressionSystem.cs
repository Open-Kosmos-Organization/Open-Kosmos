using Kosmos.Prototypes.Time;
using Unity.Entities;

namespace Kosmos.Prototypes.VAB.Gizmos
{
    public partial class UniversalTimeProgressionSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .ForEach((
                    ref UniversalTime universalTime,
                    in UniversalTimeModifier currentModifier,
                    in UniversalTimePaused currentPaused,
                    in IsCurrentPlayerTimelineTag currentPlayerTimelineTag
                ) =>
                {
                    if (currentPaused.Value)
                    {
                        return;
                    }

                    universalTime.Value += currentModifier.Value * SystemAPI.Time.DeltaTime;
                })
                .Run();
        }
    }
}
