using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
using EzySlice;
using UnityEngine.Assertions;
using Plane = EzySlice.Plane;

public class VoronoiPartition : MonoBehaviour
{
    private Vector3[] m_Points;
    private Vector3 m_RegionCenter;
    private float m_RegionRadius;
    private Tetrahedron m_SuperTetrahedron;

    public VoronoiPartition(int numberOfPartitions, Vector3 regionCenter, float regionRadius)
    {
        m_Points = new Vector3[numberOfPartitions];
        m_RegionRadius = regionRadius;
        m_RegionCenter = regionCenter;
    }

    private void Start()
    {
        m_RegionRadius = 10;
        Partition();
    }

    [ContextMenu("Partition")]
    public void Partition()
    {
        m_RegionRadius = 10;
        m_Points = new Vector3[25];
        GeneratePoints();
        BuildSuperTetrahedron();
    }

    private void GeneratePoints()
    {
        for (int i = 0; i < m_Points.Length; i++)
        {
            m_Points[i] = (Random.insideUnitSphere + m_RegionCenter) * m_RegionRadius;
        }
    }

    private void BuildSuperTetrahedron()
    {
        Debug.Log(m_RegionCenter);
        
        var edgeLength = Mathf.Sqrt(24) * m_RegionRadius;
        var bottomTriangleHeight = MathUtils.CalculateOtherSideHavingHypotenuse(edgeLength, edgeLength / 2);
        var superTetrahedronHeight = Mathf.Sqrt(6) * edgeLength / 3;

        var bottomTriangleVertex1 =
            new Vector3(0, -m_RegionRadius, 3 * m_RegionRadius) + m_RegionCenter;
        var bottomTriangleVertex2 =
            new Vector3(edgeLength / 2, -m_RegionRadius, bottomTriangleVertex1.z - bottomTriangleHeight) +
            m_RegionCenter;
        var bottomTriangleVertex3 = new Vector3(-bottomTriangleVertex2.x, -m_RegionRadius, bottomTriangleVertex2.z) +
                                    m_RegionCenter;
        
        var upperVertex = new Vector3(m_RegionCenter.x, superTetrahedronHeight - m_RegionRadius, m_RegionCenter.y) +
                          m_RegionCenter;
        
        Debug.Log((bottomTriangleVertex1 + bottomTriangleVertex2 + bottomTriangleVertex3) / 3);

        m_SuperTetrahedron =
            new Tetrahedron(bottomTriangleVertex1, bottomTriangleVertex2, bottomTriangleVertex3, upperVertex);
        var differenceCentroidAndSphereCenter = m_SuperTetrahedron.centroid - m_RegionCenter;

        m_SuperTetrahedron.a -= differenceCentroidAndSphereCenter;
        m_SuperTetrahedron.b -= differenceCentroidAndSphereCenter;
        m_SuperTetrahedron.c -= differenceCentroidAndSphereCenter;
        m_SuperTetrahedron.d -= differenceCentroidAndSphereCenter;
        
        
        Assert.AreEqual(m_SuperTetrahedron.centroid, m_RegionCenter);
    }


    private void OnDrawGizmos()
    {
        if (m_Points.Length == 0)
            return;

        Gizmos.DrawWireSphere(m_RegionCenter, m_RegionRadius);
        foreach (var point in m_Points)
        {
            Gizmos.DrawSphere(point, 0.1f);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(m_SuperTetrahedron.a, 0.1f);
        Gizmos.DrawSphere(m_SuperTetrahedron.b, 0.1f);
        Gizmos.DrawSphere(m_SuperTetrahedron.c, 0.1f);
        Gizmos.DrawSphere(m_SuperTetrahedron.d, 0.1f);
        Gizmos.DrawSphere(m_SuperTetrahedron.centroid, 0.1f);

        Gizmos.DrawLine(m_SuperTetrahedron.a, m_SuperTetrahedron.b);
        Gizmos.DrawLine(m_SuperTetrahedron.a, m_SuperTetrahedron.d);
        Gizmos.DrawLine(m_SuperTetrahedron.a, m_SuperTetrahedron.c);
        Gizmos.DrawLine(m_SuperTetrahedron.d, m_SuperTetrahedron.b);
        Gizmos.DrawLine(m_SuperTetrahedron.d, m_SuperTetrahedron.c);
        Gizmos.DrawLine(m_SuperTetrahedron.b, m_SuperTetrahedron.c);
    }
}

struct Tetrahedron
{
    public Vector3 a, b, c, d;

    public Tetrahedron(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        this.a = a;
        this.b = b;
        this.c = c;
        this.d = d;
    }

    public Vector3 centroid => (a + b + c + d) / 4;
}