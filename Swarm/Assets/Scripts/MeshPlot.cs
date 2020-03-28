using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class MeshPlot : MonoBehaviour
{
    private Mesh mesh;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
    }

    void Update()
    {
        Vector3[] vertices = mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].y = (vertices[i].x + vertices[i].z)* Time.time * 0.1f;
        }

        mesh.vertices = vertices;
    }
}
