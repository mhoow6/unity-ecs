using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms;

namespace JobSystemTutorial
{
    public class CubeManager : MonoBehaviour
    {
        [SerializeField] int CUBE_COUNT = 1000;
        readonly int CUBE_ROTATE_SPEED_BOOST = 30;

        [SerializeField] List<GameObject> cubes;
        #region Job에 대입하기 위해 선언
        NativeArray<float3> cubePositions;
        NativeArray<quaternion> cubeRotations;
        NativeArray<int> cubeSpeeds;
        #endregion

        PerformanceIndicatorUnity benchmark;
        [SerializeField] bool useJobs;

        void Awake()
        {
            cubes = new List<GameObject>();
            benchmark = new PerformanceIndicatorUnity();
        }

        void Start()
        {
            // 큐브의 속도 NativeArray 초기화
            cubeSpeeds = new NativeArray<int>(CUBE_COUNT, Allocator.Persistent);
            // CUBE_COUNT만큼 큐브 인스턴싱
            for (int i = 0; i < CUBE_COUNT; i++)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cubeSpeeds[i] = UnityEngine.Random.Range(1, 10);
                cube.transform.position = new Vector3(UnityEngine.Random.Range(5f, -5f), 0, UnityEngine.Random.Range(5f, -5f));
                cube.transform.SetParent(transform);
                Destroy(cube.GetComponent<Collider>());

                cubes.Add(cube);
            }
        }

        void Update()
        {
            benchmark.Begin();
            /******************************************************************************/

            if (useJobs)
            {
                // 1. 위치, 회전은 매번 바뀌므로 Update에서 NativeArray 초기화
                cubePositions = new NativeArray<float3>(CUBE_COUNT, Allocator.TempJob);
                cubeRotations = new NativeArray<quaternion>(CUBE_COUNT, Allocator.TempJob);
                for (int i = 0; i < CUBE_COUNT; i++)
                {
                    cubePositions[i] = cubes[i].transform.position;
                    cubeRotations[i] = cubes[i].transform.rotation;
                }

                // 2. 병렬 job 생성
                CubeMoveRotateParallelJob job = new CubeMoveRotateParallelJob()
                {
                    deltaTime = Time.deltaTime,
                    positionArray = cubePositions,
                    rotationArray = cubeRotations,
                    speedArray = cubeSpeeds
                };

                #region ParallelJob의 Schuedule() 매개변수 설명
                // innerloopBatchCount: WorkStealing(스레드 스케쥴링 알고리즘)이 세분화되는 정도.
                // 즉, 스레드 큐가 얼만큼 다른 스레드의 job을 훔칠 때 얼만큼 훔칠 것인지 정한다.
                // WorkStealing: 하나의 스레드 큐가 비어지면 다른 스레드의 task를 훔쳐올 수 있도록 하여 효율적인 동작을 유도하는 알고리즘
                #endregion

                // 3-1. job을 실행
                JobHandle jobHandle = job.Schedule(CUBE_COUNT, 100);
                // 3-2. job system에게 job이 완료되었다고 알림
                jobHandle.Complete();

                // 4. job에 의해 바뀐 큐브의 위치, 회전값 수정
                for (int i = 0; i < CUBE_COUNT; i++)
                {
                    cubes[i].transform.SetPositionAndRotation(
                        cubePositions[i],
                        cubeRotations[i]
                        );
                }

                // 5. 사용했던 NativeArray 해제
                cubePositions.Dispose();
                cubeRotations.Dispose();
            }
            else
            {
                for (int i = 0; i < CUBE_COUNT; i++)
                {
                    Vector3 afterPos = new Vector3(
                        cubes[i].transform.position.x,
                        cubes[i].transform.position.y + cubeSpeeds[i] * Time.deltaTime * 0.1f,
                        cubes[i].transform.position.z);
                    Vector3 afterRot = cubes[i].transform.rotation.eulerAngles + new Vector3(0, cubeSpeeds[i] * Time.deltaTime * CUBE_ROTATE_SPEED_BOOST, 0);

                    cubes[i].transform.SetPositionAndRotation(
                        afterPos,
                        Quaternion.Euler(afterRot)
                        );
                }
            }

            /******************************************************************************/
            benchmark.End();
        }

        void OnDestroy()
        {
            cubeSpeeds.Dispose();
        }
    }

    // 리스트의 인덱스에 맞게 Job을 병렬로 실행하기 위하여 IJobParallelFor 구현
    [BurstCompile]
    public struct CubeMoveRotateParallelJob : IJobParallelFor
    {
        #region ParallelJob 주의사항
        // job 쓰레드는 메인 쓰레드에서 동작하는 것이 아니므로 Transform을 직접적으로 건드릴 수 없다.
        // ※ MonoBehavior안에서 돌아가는 UnityEngine namespace의 클래스들은 다 못건든다고 보면된다.
        // 그래서 어떤 데이터를 수정할 것인지 우리가 명시해주어야 한다.
        #endregion

        public NativeArray<float3> positionArray; // cube transform.position
        public NativeArray<quaternion> rotationArray; // cube transform.rotation
        public NativeArray<int> speedArray; // cube speed

        // NativeField가 아닌 값들은 Job에서 참조값이 아닌 항상 복사값임을 유의하자.
        public float deltaTime; // Time.deltaTime

        public void Execute(int index)
        {
            // 위치
            positionArray[index] += new float3(0f, speedArray[index] * deltaTime, 0f);

            // 회전
            var desiredQuternion = math.mul(math.normalizesafe(rotationArray[index]), quaternion.AxisAngle(math.up(), deltaTime * speedArray[index]));
            rotationArray[index] = desiredQuternion;
        }
    }
}

