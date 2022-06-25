using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent] //This generates a Monobehaviour version of this which we can add as a component. When we hit play, that component will be replaced by this data instead.
public struct Destination : IComponentData
{
    public float3 Value;
}
