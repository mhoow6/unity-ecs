using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace ECSTutorial
{
    // ECS - Component
    // 오직 데이터만 들고 있는다.
    public struct CubeComponent : IComponentData
    {
        // 움직임의 속도를 지정함
        public float speed;
    }
}

