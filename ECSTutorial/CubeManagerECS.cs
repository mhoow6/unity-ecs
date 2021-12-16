using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

#region Classic
/*public class Rotator : MonoBehaviour
{
    public float speed;

    void Update()
    {
        Vector3 after = transform.rotation.eulerAngles + new Vector3(0, speed * Time.deltaTime, 0);
        transform.SetPositionAndRotation(
            transform.position,
            Quaternion.Euler(after)
            );
    }
}*/
#endregion

namespace ECSTutorial
{
    // 큐브를 초기화하는 용도의 클래스이다. ECS와 아무 상관없음
    public class CubeManagerECS : MonoBehaviour
    {
        [SerializeField] Mesh mesh;
        [SerializeField] Material mat;
        [SerializeField] int CUBE_COUNT = 0;

        void Start()
        {
            // 현재 World에서 Entity를 관리하는 Manager 불러오기
            var em = World.DefaultGameObjectInjectionWorld.EntityManager;

            // Archetype을 정의하여 Component을 묶을 수 있다.
            // Translation: Transform의 position과 비슷함. float3를 필드로 가지고 있는 구조체, float3 내부에는 x,y,z가 있다.
            // Rotation: Transform의 rotation
            // RenderMesh: 매쉬를 렌더링할 때 쓰이는 구조체
            // RenderBounds: RenderMesh를 렌더링하기 위해 쓰이는 구조체 (2019.3.x 버젼 이후 이게 없으면 렌더링 안 됨)
            // LocalToWorld: 렌더러가 게임세계에 매쉬를 어떻게 보여줄 지 계산하는데 쓰이는 구조체
            var arch = em.CreateArchetype(
                typeof(CubeComponent),
                typeof(Translation),
                typeof(Rotation),
                typeof(RenderMesh),
                typeof(LocalToWorld),
                typeof(RenderBounds)
                );

            // CreateEntity 인자로 NativeArray<Entity>를 전달할 수 있다, 해당 변수는 CreateEntity로 인해 만들어진 엔티티를 보관하는 용도이다.
            NativeArray<Entity> entityArray = new NativeArray<Entity>(CUBE_COUNT, Allocator.Temp);

            // Entity는 단순 인덱스이다. 실제 데이터는 Component가 들고있으므로 이런식으로 코드를 작성해서 엔티티를 만들어준다.
            em.CreateEntity(arch, entityArray);

            // entityArray의 원소들을 초기화해준다.
            foreach (var entity in entityArray)
            {
                // Entity관리자를 통해서 해당 Entity의 Data를 다르게 바꾼다.
                em.SetComponentData(entity, new CubeComponent() { speed = Random.Range(1, 10)});

                // rendermesh는 shared component임
                // 렌더링에 쓰일 매쉬, 텍스처 지정
                em.SetSharedComponentData(entity, new RenderMesh() {
                    mesh = mesh,
                    material = mat
                });

                // 위치 랜덤
                em.SetComponentData(entity, new Translation()
                {
                    Value = new Unity.Mathematics.float3(Random.Range(5f, -5f), 0, Random.Range(5f, -5f))
                });
            }

            // entityArray는 더 이상 사용되지 않으므로 해제한다.
            entityArray.Dispose();
        }
    }
}
