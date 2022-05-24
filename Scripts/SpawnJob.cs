
using Unity.Jobs;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine.Jobs;

public struct SpawnManyJob : IJobParallelForTransform {

    [ReadOnly] public Unity.Mathematics.Random rand;
    public NativeArray<float3> angles;
    public NativeArray<float> speeds, timeAlive;
    public NativeArray<int> lifeSpan;
    public NativeArray<Color> colors;

    //public NativeArray<CubeEntity> entities;

    public SpawnManyJob(NativeArray<float3> angles, NativeArray<float> speeds, NativeArray<float> timeAlive, NativeArray<int> lifeSpan, NativeArray<Color> colors) {
        //this.entities = entities;
        rand = new Unity.Mathematics.Random(23424);
        this.angles = angles;
        this.timeAlive = timeAlive;
        this.speeds = speeds;
        this.lifeSpan = lifeSpan;
        this.colors = colors;
    }

    public void Execute(int index, TransformAccess transform) {
        // Position
        transform.position = new Vector3(rand.NextInt(-150, 150), rand.NextInt(-150, 150), rand.NextInt(-150, 150));

        // Scale
        float scale = rand.NextFloat(0.5f, 5);
        transform.localScale = Vector3.one * scale;

        // Colors
        colors[index] = new Color(rand.NextFloat(0, 1.0f), rand.NextFloat(0, 1.0f), rand.NextFloat(0, 1.0f));
        // Speed
        speeds[index] = rand.NextFloat(0.5f, 5);
        // Time alive
        timeAlive[index] = 0;
        // Life-span
        lifeSpan[index] = rand.NextInt(4, 10);
        // Angle
        angles[index] = new float3(rand.NextInt(0, 2), rand.NextInt(0, 2), rand.NextInt(0, 2));
    }
}

public struct SpawnFew : IJob {

    Unity.Mathematics.Random rand;
    Color color;
    float speed, timeAlive, scale;
    int lifeSpan;
    float3 position, angle;

    public void Execute() {
        position = new Vector3(rand.NextInt(-150, 150), rand.NextInt(-150, 150), rand.NextInt(-150, 150));
        // Scale
        scale = rand.NextFloat(0.5f, 5);
        // Colors
        color = new Color(rand.NextFloat(0, 1.0f), rand.NextFloat(0, 1.0f), rand.NextFloat(0, 1.0f));
        // Speed
        speed = rand.NextFloat(0.5f, 5);
        // Time alive
        timeAlive = 0;
        // Life-span
        lifeSpan = rand.NextInt(4, 10);
        // Angle
        angle = new float3(rand.NextInt(0, 2), rand.NextInt(0, 2), rand.NextInt(0, 2));
    }
}
