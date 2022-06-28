using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class AssignNewDestinationSystem : SystemBase
{
    private RandomSystem _randomSystem;

    protected override void OnCreate()
    {
        _randomSystem = World.GetExistingSystem<RandomSystem>();
    }

    protected override void OnUpdate()
    {
        var randomArray = _randomSystem.RandomArray;
        
        Entities
            .WithNativeDisableParallelForRestriction(randomArray)
            .ForEach((int nativeThreadIndex, ref Destination destination, in Translation translation) => {
                
                float distance = math.abs(math.length(destination.Value - translation.Value));
                if (distance < 0.1f)
                {
                    Random random = randomArray[nativeThreadIndex];
                    destination.Value += random.NextFloat3(-50, 50);

                    randomArray[nativeThreadIndex] = random;
                }

        }).ScheduleParallel();
    }
}
