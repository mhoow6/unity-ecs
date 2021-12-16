using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ECSTutorial
{
    public class CubeManagerClassic : MonoBehaviour
    {
        [SerializeField] int CUBE_COUNT = 1000;
        [SerializeField] int CUBE_ROTATE_SPEED_BOOST = 30;

        [SerializeField, ShowOnly] List<GameObject> cubes = new List<GameObject>();
        [SerializeField, ShowOnly] int[] cubeSpeeds;

        void Start()
        {
            cubeSpeeds = new int[CUBE_COUNT];
            for (int i = 0; i < CUBE_COUNT; i++)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cubeSpeeds[i] = Random.Range(1, 10);
                cube.transform.position = new Vector3(Random.Range(5f, -5f), 0, Random.Range(5f, -5f));
                cube.transform.SetParent(transform);
                Destroy(cube.GetComponent<Collider>());

                cubes.Add(cube);
            }
        }

        
        void Update()
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
    }
}

