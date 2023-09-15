using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    public int xSize;
    public int zSize;
    public List<Transform> cubesTransform;

    public float smoothMoveDuration;
    public float ySpeed = 0.5f;
    public float amplitude;

    void Start()
    {
        GenerateCubes();
    }
    private void GenerateCubes()
    {
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < zSize; j++)
            {
                if (i == 0 || i == xSize - 1 || j == 0 || j == zSize - 1)
                {
                    var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.GetComponent<MeshRenderer>().material.color = Random.ColorHSV();
                    cube.transform.parent = transform;
                    cube.transform.position = new Vector3(-zSize + i, 0f, j);
                    cubesTransform.Add(cube.transform);
                }
            }
        }
    }

    private void Update()
    {
        for (int i = 0; i < cubesTransform.Count; i++)
        {
            var yPos = i % 2 == 0 ? Mathf.Sin((i + amplitude) * Time.time * ySpeed) : 
                                    Mathf.Cos((i + amplitude) * Time.time * ySpeed);
            yPos = yPos * 0.5f + 0.5f;
            var newPos = new Vector3(cubesTransform[i].position.x,
                yPos,
                cubesTransform[i].position.z);

            cubesTransform[i].position = Vector3.Lerp(cubesTransform[i].position,
                newPos,
                Time.deltaTime * smoothMoveDuration);
        }
    }
}