using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class MoveToDestinationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        
        Entities.ForEach((ref Translation translation, ref Rotation rotation, in Destination destination, in Speed speed) => {
            if (math.all(translation.Value == destination.Value)) 
                return;
            
            float3 toDestination = destination.Value - translation.Value;
            rotation.Value = quaternion.LookRotation(toDestination, new float3(0, 1, 0));

            // movement = direction * speed * deltatime.
            float3 movement = math.normalize(toDestination) * speed.Value * deltaTime;

            //if (the movement value of this frame) > (the amount we need to move).
            if (math.length(movement) > math.length(toDestination))
            {
                translation.Value = destination.Value;
            }
            else
            {
                translation.Value += movement;
            }

        }).ScheduleParallel();
    }
}
