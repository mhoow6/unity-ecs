using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using Unity.Collections;
using Unity.Burst;

namespace JobSystemTutorial
{
    public class Testing : MonoBehaviour
    {
        PerformanceIndicatorUnity benchmark;
        [SerializeField] bool useJobs;

        void Awake()
        {
            benchmark = new PerformanceIndicatorUnity();
        }

        void Update()
        {
            benchmark.Begin();


            if (useJobs)
            {
                // ������ Job�� ��� ���� NativeList
                NativeList<JobHandle> jobHandleList = new NativeList<JobHandle>(Allocator.Temp);
                // 10���� Job�� �õ��Ѵ�
                for (int i = 0; i < 10; i++)
                {
                    JobHandle jobHandle = ReallyToughTaskJob();
                    // ����Ʈ�� �߰�
                    jobHandleList.Add(jobHandle);
                }

                // job�� �����ٰ� jobsystem���� �˸�
                JobHandle.CompleteAll(jobHandleList); // jobHandle.Complete();
                // ���� ����Ʈ�� ���̻� ������ �����Ƿ� �����Ѵ�
                jobHandleList.Dispose();
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    ReallyToughTask();
                }
            }

            

            benchmark.End();
        }

        void ReallyToughTask()
        {
            // �ſ� ������ ���
            float value = 0f;
            int count = 50000;
            while (count != 0)
            {
                value = Mathf.Exp(Mathf.Sqrt(value));
                count--;
            }
        }
        JobHandle ReallyToughTaskJob()
        {
            ReallyToughJob job = new ReallyToughJob();

            // ���� �����尡 ������ �ش� job�� ������ �� �ֵ��� �Ѵ�. ��ȯ�����δ� job�� �ڵ鸵�� �� �ִ� JobHandle ����ü�� ��ȯ�Ѵ�.
            // Worker Thread�� �ڽ��� CPU �ھ� ���� ���� �ٸ���
            return job.Schedule();
        }
    }

    // ����Ʈ �����Ϸ��� �۵����� �ڵ尡 �����ϵ� �� ���� �� ����ȭ�ϰ� ������ش�.
    [BurstCompile]
    public struct ReallyToughJob : IJob
    {
        public void Execute()
        {
            throw new System.NotImplementedException();
        }
    }
}

