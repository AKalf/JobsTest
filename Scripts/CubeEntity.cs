using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public struct CubeEntity {
    public int Index;
    public float Speed, TimeAlive;
    public int LifeSpan;
    public float4 Color;
    public float3 AngleRotation;

    public CubeEntity(int index, float speed, int lifeSpan, float4 color, float3 angleRotation) {
        this.Index = index;
        this.Speed = speed;
        this.TimeAlive = 0;
        this.LifeSpan = lifeSpan;
        this.Color = color;
        this.AngleRotation = angleRotation;
    }

    public void SetSpeed(float s) {
        Speed = s;
    }
}
