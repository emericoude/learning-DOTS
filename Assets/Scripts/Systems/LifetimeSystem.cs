using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class LifetimeSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _endSimulationECBSystem;

    protected override void OnCreate()
    {
        _endSimulationECBSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        var ecb = _endSimulationECBSystem.CreateCommandBuffer().AsParallelWriter();
        
        Entities.ForEach((Entity entity, int entityInQueryIndex, ref Lifetime lifetime) => {
            lifetime.Value -= deltaTime;

            if (lifetime.Value <= 0)
            {
                ecb.DestroyEntity(entityInQueryIndex, entity);
            }
        }).ScheduleParallel();

        _endSimulationECBSystem.AddJobHandleForProducer(Dependency);
    }
}
