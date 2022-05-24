using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
public struct FadeJob : IJobParallelFor {
    public NativeArray<float> FadeValues;
    [ReadOnly] float deltaTime;
    public void Execute(int index) {
        FadeValues[index] -= deltaTime;
    }
}
