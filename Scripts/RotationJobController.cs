
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
    NativeArray<Color> colors;
    NativeArray<float3> angles;
    TransformAccessArray transforms;

    RotationJob rotationJob;
    JobHandle rotationJobHandler;
    SpawnManyJob spawnJob;
    JobHandle spawnJobHandler;

    Material[] cubeMatReferences = null;
    float startTime;
    Color c = new Color();
    void Start() {
        transforms = new TransformAccessArray(NumberOfCubes);
        speeds = new NativeArray<float>(NumberOfCubes, Allocator.Persistent);
        timeAlive = new NativeArray<float>(NumberOfCubes, Allocator.Persistent);
        lifeSpan = new NativeArray<int>(NumberOfCubes, Allocator.Persistent);
        angles = new NativeArray<float3>(NumberOfCubes, Allocator.Persistent);
        colors = new NativeArray<Color>(NumberOfCubes, Allocator.Persistent);
        cubeMatReferences = new Material[NumberOfCubes];



        spawnJob = new SpawnManyJob(angles, speeds, timeAlive, lifeSpan, colors);
        rotationJob = new RotationJob {
            speeds = this.speeds,
            timeAlive = this.timeAlive,
            lifeSpans = this.lifeSpan,
            angles = this.angles,
            deltaTime = Time.deltaTime,
        };


        spawnJobHandler = spawnJob.Schedule(transforms);
        spawnJobHandler.Complete();
        angles = spawnJob.angles;
        speeds = spawnJob.speeds;
        timeAlive = spawnJob.timeAlive;
        lifeSpan = spawnJob.lifeSpan;
        colors = spawnJob.colors;
        for (int i = 0; i < NumberOfCubes; i++) {
            Transform trans = ((GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(TheCubePrefab)).transform;
            cubeMatReferences[i] = trans.GetComponent<Renderer>().material;
            transforms.Add(trans);
            cubeMatReferences[i].color = colors[i];
        }

    }
    void Update() {
        startTime = Time.realtimeSinceStartup;
        rotationJob.deltaTime = Time.deltaTime;
        rotationJobHandler = rotationJob.Schedule(transforms, spawnJobHandler);
        rotationJobHandler.Complete();
        this.timeAlive = rotationJob.timeAlive;

        WhoToFadeJob checkLifeStatus = new WhoToFadeJob {
            lifeSpans = this.lifeSpan,
            timeAlive = this.timeAlive
        };
        JobHandle handler = checkLifeStatus.Schedule(NumberOfCubes, 124);
        NativeArray<int> indexes =
        for (int i = 0; i < NumberOfCubes; i++) {
            if (timeAlive[i] < lifeSpan[i])
                continue;
            else {

                c = cubeMatReferences[i].color;
                c.a -= rotationJob.deltaTime;
                cubeMatReferences[i].color = c;

                if (cubeMatReferences[i].color.a <= 0) {
                    //SpawnFew spawnFewJob = new SpawnFew();
                    //spawnFewJob.Schedule();

                    transforms[i].position = new Vector3(UnityEngine.Random.Range(-150, 150), UnityEngine.Random.Range(-150, 150), UnityEngine.Random.Range(-150, 150));
                    transforms[i].localScale = Vector3.one * UnityEngine.Random.Range(0.5f, 5);
                    cubeMatReferences[i].color = new Color(UnityEngine.Random.Range(0, 1.0f), UnityEngine.Random.Range(0, 1.0f), UnityEngine.Random.Range(0, 1.0f));
                    colors[i] = cubeMatReferences[i].color;
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
        colors.Dispose();
        timeAlive.Dispose();
        lifeSpan.Dispose();
    }
}
