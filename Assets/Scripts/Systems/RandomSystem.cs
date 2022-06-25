using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Jobs.LowLevel.Unsafe;

[UpdateInGroup(typeof(InitializationSystemGroup))] //This ensures that this system will be setup before any other that might use it.
public partial class RandomSystem : SystemBase
{
    public NativeArray<Unity.Mathematics.Random> RandomArray { get; private set; }

    protected override void OnCreate()
    {
        Random[] randomArray = new Unity.Mathematics.Random[JobsUtility.MaxJobThreadCount];
        System.Random seed = new System.Random();

        for (int i = 0; i < randomArray.Length; i++)
        {
            randomArray[i] = new Unity.Mathematics.Random((uint)seed.Next());
        }

        RandomArray = new NativeArray<Unity.Mathematics.Random>(randomArray, Allocator.Persistent);
    }

    protected override void OnUpdate() { }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        RandomArray.Dispose();
    }
}
