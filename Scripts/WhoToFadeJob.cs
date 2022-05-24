using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
public struct WhoToFadeJob : IJobParallelFor {
    [ReadOnly] public NativeArray<float> timeAlive;
    [ReadOnly] public NativeArray<int> lifeSpans;
    public NativeArray<int> results;
    public void Execute(int index) {
        if (timeAlive[index] < lifeSpans[index])
            results[index] = index;
        else
            results[index] = -1;
    }


}
