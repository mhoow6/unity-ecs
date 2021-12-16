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
    // ť�긦 �ʱ�ȭ�ϴ� �뵵�� Ŭ�����̴�. ECS�� �ƹ� �������
    public class CubeManagerECS : MonoBehaviour
    {
        [SerializeField] Mesh mesh;
        [SerializeField] Material mat;
        [SerializeField] int CUBE_COUNT = 0;

        void Start()
        {
            // ���� World���� Entity�� �����ϴ� Manager �ҷ�����
            var em = World.DefaultGameObjectInjectionWorld.EntityManager;

            // Archetype�� �����Ͽ� Component�� ���� �� �ִ�.
            // Translation: Transform�� position�� �����. float3�� �ʵ�� ������ �ִ� ����ü, float3 ���ο��� x,y,z�� �ִ�.
            // Rotation: Transform�� rotation
            // RenderMesh: �Ž��� �������� �� ���̴� ����ü
            // RenderBounds: RenderMesh�� �������ϱ� ���� ���̴� ����ü (2019.3.x ���� ���� �̰� ������ ������ �� ��)
            // LocalToWorld: �������� ���Ӽ��迡 �Ž��� ��� ������ �� ����ϴµ� ���̴� ����ü
            var arch = em.CreateArchetype(
                typeof(CubeComponent),
                typeof(Translation),
                typeof(Rotation),
                typeof(RenderMesh),
                typeof(LocalToWorld),
                typeof(RenderBounds)
                );

            // CreateEntity ���ڷ� NativeArray<Entity>�� ������ �� �ִ�, �ش� ������ CreateEntity�� ���� ������� ��ƼƼ�� �����ϴ� �뵵�̴�.
            NativeArray<Entity> entityArray = new NativeArray<Entity>(CUBE_COUNT, Allocator.Temp);

            // Entity�� �ܼ� �ε����̴�. ���� �����ʹ� Component�� ��������Ƿ� �̷������� �ڵ带 �ۼ��ؼ� ��ƼƼ�� ������ش�.
            em.CreateEntity(arch, entityArray);

            // entityArray�� ���ҵ��� �ʱ�ȭ���ش�.
            foreach (var entity in entityArray)
            {
                // Entity�����ڸ� ���ؼ� �ش� Entity�� Data�� �ٸ��� �ٲ۴�.
                em.SetComponentData(entity, new CubeComponent() { speed = Random.Range(1, 10)});

                // rendermesh�� shared component��
                // �������� ���� �Ž�, �ؽ�ó ����
                em.SetSharedComponentData(entity, new RenderMesh() {
                    mesh = mesh,
                    material = mat
                });

                // ��ġ ����
                em.SetComponentData(entity, new Translation()
                {
                    Value = new Unity.Mathematics.float3(Random.Range(5f, -5f), 0, Random.Range(5f, -5f))
                });
            }

            // entityArray�� �� �̻� ������ �����Ƿ� �����Ѵ�.
            entityArray.Dispose();
        }
    }
}
