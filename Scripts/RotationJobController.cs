
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;

public class RotationJobController : MonoBehaviour {
    [SerializeField] int NumberOfCubes = 2000;
    [SerializeField] UnityEngine.UI.Text framesText = null;
    [SerializeField] GameObject TheCubePrefab = null;
    NativeArray<float> speeds, timeAlive;
    NativeArray<int> lifeSpan;
    NativeArray<float3> angles;
    NativeArray<float> alphaColor;
    TransformAccessArray transforms;
    RotationJob rotationJob;
    JobHandle handler;
    Material[] cubeMatReferences = null;
    float startTime;
    Color c = new Color();
    void Start() {
        transforms = new TransformAccessArray(NumberOfCubes);
        speeds = new NativeArray<float>(NumberOfCubes, Allocator.TempJob);
        timeAlive = new NativeArray<float>(NumberOfCubes, Allocator.TempJob);
        lifeSpan = new NativeArray<int>(NumberOfCubes, Allocator.TempJob);
        angles = new NativeArray<float3>(NumberOfCubes, Allocator.TempJob);
        alphaColor = new NativeArray<float>(NumberOfCubes, Allocator.TempJob);
        cubeMatReferences = new Material[NumberOfCubes];
        for (int i = 0; i < NumberOfCubes; i++) {
            Transform trans = Instantiate(TheCubePrefab).transform;
            // Position
            trans.position = new Vector3(UnityEngine.Random.Range(-150, 150), UnityEngine.Random.Range(-150, 150), UnityEngine.Random.Range(-150, 150));
            // Scale
            float scale = UnityEngine.Random.Range(0.5f, 5);
            trans.localScale = Vector3.one * scale;
            // Colors
            alphaColor[i] = 1;
            cubeMatReferences[i] = trans.GetComponent<MeshRenderer>().material;
            cubeMatReferences[i].color = new Color(UnityEngine.Random.Range(0, 1.0f), UnityEngine.Random.Range(0, 1.0f), UnityEngine.Random.Range(0, 1.0f));
            // Speed
            speeds[i] = UnityEngine.Random.Range(0.5f, 5);
            // Time alive
            timeAlive[i] = 0;
            // Life-span
            lifeSpan[i] = UnityEngine.Random.Range(4, 10);
            // Angle
            angles[i] = new float3(UnityEngine.Random.Range(0, 2), UnityEngine.Random.Range(0, 2), UnityEngine.Random.Range(0, 2));

            transforms.Add(trans);
        }
        rotationJob = new RotationJob {
            speeds = this.speeds,
            timeAlive = this.timeAlive,
            lifeSpans = this.lifeSpan,
            angles = this.angles,
            deltaTime = Time.deltaTime,
        };
    }
    void Update() {
        startTime = Time.realtimeSinceStartup;
        rotationJob.deltaTime = Time.deltaTime;
        handler = rotationJob.Schedule(transforms);
        handler.Complete();
        this.timeAlive = rotationJob.timeAlive;
        for (int i = 0; i < NumberOfCubes; i++) {
            if (timeAlive[i] < lifeSpan[i])
                continue;
            else {

                alphaColor[i] -= rotationJob.deltaTime;
                c = cubeMatReferences[i].color;
                c.a = alphaColor[i];
                cubeMatReferences[i].color = c;

                if (alphaColor[i] <= 0) {
                    transforms[i].position = new Vector3(UnityEngine.Random.Range(-150, 150), UnityEngine.Random.Range(-150, 150), UnityEngine.Random.Range(-150, 150));
                    transforms[i].localScale = Vector3.one * UnityEngine.Random.Range(0.5f, 5);
                    cubeMatReferences[i].color = new Color(UnityEngine.Random.Range(0, 1.0f), UnityEngine.Random.Range(0, 1.0f), UnityEngine.Random.Range(0, 1.0f));
                    alphaColor[i] = 1;
                    // Speed
                    speeds[i] = UnityEngine.Random.Range(0.5f, 5);
                    rotationJob.speeds[i] = speeds[i];
                    // Time alive
                    timeAlive[i] = 0;
                    rotationJob.timeAlive[i] = 0;
                    // Life-span
                    lifeSpan[i] = UnityEngine.Random.Range(4, 10);
                    rotationJob.lifeSpans[i] = lifeSpan[i];
                    // Angle
                    angles[i] = new float3(UnityEngine.Random.Range(0, 2), UnityEngine.Random.Range(0, 2), UnityEngine.Random.Range(0, 2));
                    rotationJob.angles[i] = angles[i];
                }
            }
        }
        framesText.text = (Time.realtimeSinceStartup - startTime) * 1000 + " ms";
    }

    private void OnDestroy() {
        speeds.Dispose();
        angles.Dispose();
        transforms.Dispose();
        alphaColor.Dispose();
        timeAlive.Dispose();
        lifeSpan.Dispose();
    }
}
