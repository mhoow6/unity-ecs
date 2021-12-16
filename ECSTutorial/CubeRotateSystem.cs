using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


namespace ECSTutorial
{
    public class CubeRotateSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((ref Rotation rot, ref CubeComponent comp) =>
            {
                // ���� ���ʹϾ𿡼�, yaw�࿡ ���� Time.deltaTime * comp.speed��ŭ ȸ����, ���ʹϾ� ��
                var desired = math.mul(math.normalizesafe(rot.Value), quaternion.AxisAngle(math.up(), Time.DeltaTime * comp.speed));

                rot = new Rotation { Value = desired };
            });
        }
    }
}

