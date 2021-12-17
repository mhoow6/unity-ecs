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
                // 복수의 Job을 담기 위한 NativeList
                NativeList<JobHandle> jobHandleList = new NativeList<JobHandle>(Allocator.Temp);
                // 10개의 Job을 시도한다
                for (int i = 0; i < 10; i++)
                {
                    JobHandle jobHandle = ReallyToughTaskJob();
                    // 리스트에 추가
                    jobHandleList.Add(jobHandle);
                }

                // job이 끝났다고 jobsystem에게 알림
                JobHandle.CompleteAll(jobHandleList); // jobHandle.Complete();
                // 이제 리스트는 더이상 사용되지 않으므로 해제한다
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
            // 매우 복잡한 계산
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

            // 여유 쓰레드가 있을때 해당 job을 실행할 수 있도록 한다. 반환값으로는 job을 핸들링할 수 있는 JobHandle 구조체를 반환한다.
            // Worker Thread는 자신의 CPU 코어 수에 따라 다르다
            return job.Schedule();
        }
    }

    // 버스트 컴파일러를 작동시켜 코드가 컴파일될 때 더욱 더 최적화하게 만들어준다.
    [BurstCompile]
    public struct ReallyToughJob : IJob
    {
        public void Execute()
        {
            throw new System.NotImplementedException();
        }
    }
}

