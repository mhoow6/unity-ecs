using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace ECSTutorial
{
    // ECS - Component
    // ���� �����͸� ��� �ִ´�.
    public struct CubeComponent : IComponentData
    {
        // �������� �ӵ��� ������
        public float speed;
    }
}

