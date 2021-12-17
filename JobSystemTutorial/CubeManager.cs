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
        #region Job�� �����ϱ� ���� ����
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
            // ť���� �ӵ� NativeArray �ʱ�ȭ
            cubeSpeeds = new NativeArray<int>(CUBE_COUNT, Allocator.Persistent);
            // CUBE_COUNT��ŭ ť�� �ν��Ͻ�
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
                // 1. ��ġ, ȸ���� �Ź� �ٲ�Ƿ� Update���� NativeArray �ʱ�ȭ
                cubePositions = new NativeArray<float3>(CUBE_COUNT, Allocator.TempJob);
                cubeRotations = new NativeArray<quaternion>(CUBE_COUNT, Allocator.TempJob);
                for (int i = 0; i < CUBE_COUNT; i++)
                {
                    cubePositions[i] = cubes[i].transform.position;
                    cubeRotations[i] = cubes[i].transform.rotation;
                }

                // 2. ���� job ����
                CubeMoveRotateParallelJob job = new CubeMoveRotateParallelJob()
                {
                    deltaTime = Time.deltaTime,
                    positionArray = cubePositions,
                    rotationArray = cubeRotations,
                    speedArray = cubeSpeeds
                };

                #region ParallelJob�� Schuedule() �Ű����� ����
                // innerloopBatchCount: WorkStealing(������ �����층 �˰���)�� ����ȭ�Ǵ� ����.
                // ��, ������ ť�� ��ŭ �ٸ� �������� job�� ��ĥ �� ��ŭ ��ĥ ������ ���Ѵ�.
                // WorkStealing: �ϳ��� ������ ť�� ������� �ٸ� �������� task�� ���Ŀ� �� �ֵ��� �Ͽ� ȿ������ ������ �����ϴ� �˰���
                #endregion

                // 3-1. job�� ����
                JobHandle jobHandle = job.Schedule(CUBE_COUNT, 100);
                // 3-2. job system���� job�� �Ϸ�Ǿ��ٰ� �˸�
                jobHandle.Complete();

                // 4. job�� ���� �ٲ� ť���� ��ġ, ȸ���� ����
                for (int i = 0; i < CUBE_COUNT; i++)
                {
                    cubes[i].transform.SetPositionAndRotation(
                        cubePositions[i],
                        cubeRotations[i]
                        );
                }

                // 5. ����ߴ� NativeArray ����
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

    // ����Ʈ�� �ε����� �°� Job�� ���ķ� �����ϱ� ���Ͽ� IJobParallelFor ����
    [BurstCompile]
    public struct CubeMoveRotateParallelJob : IJobParallelFor
    {
        #region ParallelJob ���ǻ���
        // job ������� ���� �����忡�� �����ϴ� ���� �ƴϹǷ� Transform�� ���������� �ǵ帱 �� ����.
        // �� MonoBehavior�ȿ��� ���ư��� UnityEngine namespace�� Ŭ�������� �� ���ǵ�ٰ� ����ȴ�.
        // �׷��� � �����͸� ������ ������ �츮�� ������־�� �Ѵ�.
        #endregion

        public NativeArray<float3> positionArray; // cube transform.position
        public NativeArray<quaternion> rotationArray; // cube transform.rotation
        public NativeArray<int> speedArray; // cube speed

        // NativeField�� �ƴ� ������ Job���� �������� �ƴ� �׻� ���簪���� ��������.
        public float deltaTime; // Time.deltaTime

        public void Execute(int index)
        {
            // ��ġ
            positionArray[index] += new float3(0f, speedArray[index] * deltaTime, 0f);

            // ȸ��
            var desiredQuternion = math.mul(math.normalizesafe(rotationArray[index]), quaternion.AxisAngle(math.up(), deltaTime * speedArray[index]));
            rotationArray[index] = desiredQuternion;
        }
    }
}

