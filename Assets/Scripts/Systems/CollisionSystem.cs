using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Physics.Systems;

public partial class CollisionSystem : SystemBase
{
    private StepPhysicsWorld _stepPhysics;

    protected override void OnCreate()
    {
        _stepPhysics = World.GetOrCreateSystem<StepPhysicsWorld>();
    }

    protected override void OnUpdate()
    {
        Dependency = new OnCollision
        {

        }.Schedule(_stepPhysics.Simulation, Dependency);
    }

    struct OnCollision : ITriggerEventsJob
    {
        public void Execute(TriggerEvent triggerEvent)
        {

        }
    }
}
