using UnityEngine;
using Unity.Collections;
using UnityEngine.Jobs;
using Unity.Burst;
using Unity.Mathematics;

[BurstCompile]
public struct RotationJob : IJobParallelForTransform {

    [ReadOnly] public NativeArray<float3> angles;
    [ReadOnly] public NativeArray<float> speeds;
    [ReadOnly] public NativeArray<int> lifeSpans;
    public NativeArray<float> timeAlive;
    [ReadOnly] public float deltaTime;

    public void Execute(int index, TransformAccess transform) {
        timeAlive[index] += deltaTime;
        transform.rotation = math.mul(transform.rotation, quaternion.AxisAngle(angles[index], speeds[index] * deltaTime));
    }
}
