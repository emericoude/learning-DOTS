using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Rendering;

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
            ColorGroup = GetComponentDataFromEntity<URPMaterialPropertyBaseColor>(),
            PersonGroup = GetComponentDataFromEntity<PersonTag>(true), //pass true if it's readonly
            Seed = System.DateTimeOffset.Now.Millisecond //creating a seed based on the computer's time

        }.Schedule(_stepPhysics.Simulation, Dependency);
    }

    struct OnCollision : ITriggerEventsJob
    {
        //This gets us access to all of the Entities with the URPMaterialPropertyBaseColor and the PersonTag components.
        public ComponentDataFromEntity<URPMaterialPropertyBaseColor> ColorGroup;
        [ReadOnly] public ComponentDataFromEntity<PersonTag> PersonGroup; //Use the [ReadOnly] tag when it makes sense.

        public float Seed;
        
        public void Execute(TriggerEvent triggerEvent)
        {
            bool isEntityAPerson = PersonGroup.HasComponent(triggerEvent.EntityA);
            bool isEntityBPerson = PersonGroup.HasComponent(triggerEvent.EntityB);

            if (isEntityAPerson && isEntityBPerson)
            {
                //creating a new random seed from multiplying the two indexes of the collided bodies and the seed value.
                //We add 1 to the seed value to prevent scenarios where we multiply by 0.
                Random randomSeed = new Random((uint)(triggerEvent.BodyIndexA * triggerEvent.BodyIndexB + (Seed + 1)));

                randomSeed = ChangeMaterialColor(randomSeed, triggerEvent.EntityA);
                ChangeMaterialColor(randomSeed, triggerEvent.EntityB);
            }
        }

        //Simple method to change the color of an entity
        private Random ChangeMaterialColor(Random seed, Entity entity)
        {
            if (ColorGroup.HasComponent(entity))
            {
                var colorComponent = ColorGroup[entity]; //get
                colorComponent.Value = seed.NextFloat4(0, 1);

                ColorGroup[entity] = colorComponent; //set

                //THIS IS INVALID: ColorGroup[entity].Value = seed.NextFloat4(0, 1);
            }

            return seed; //this is to prevent re-using the same exact sub-seed.
        }
    }
}
