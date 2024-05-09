using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateTrackingPlane : MonoBehaviour
{    
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public GameObject CreatePlane(Camera rCamera)
    {        
        Vector3[] vertices = new Vector3[4];
        vertices[0] = rCamera.ScreenToWorldPoint(new Vector3(0, rCamera.pixelHeight-1, rCamera.nearClipPlane + 0.3f));
        vertices[1] = rCamera.ScreenToWorldPoint(new Vector3(rCamera.pixelWidth - 1, rCamera.pixelHeight - 1, rCamera.nearClipPlane + 0.3f));
        vertices[2] = rCamera.ScreenToWorldPoint(new Vector3(rCamera.pixelWidth - 1, 0, rCamera.nearClipPlane + 0.3f));
        vertices[3] = rCamera.ScreenToWorldPoint(new Vector3(0, 0, rCamera.nearClipPlane + 0.3f));
        foreach (var vert in vertices)
        {
            Debug.Log(vert);
        }
        int[] triangles = new int[] { 3, 0, 1, 3, 1, 2 };
        Vector2[] uvs = new Vector2[]
        {
            new Vector2(0,0),
            new Vector2(0,1),
            new Vector2(1,1),
            new Vector2(1,0)
           //FindObjectOfType<TrackingPlane>().CamToUv(FindObjectOfType<TrackingPlane>()._corners[0]),
           //FindObjectOfType<TrackingPlane>().CamToUv(FindObjectOfType<TrackingPlane>()._corners[2]),
           //FindObjectOfType<TrackingPlane>().CamToUv(FindObjectOfType<TrackingPlane>()._corners[3]),
           //FindObjectOfType<TrackingPlane>().CamToUv(FindObjectOfType<TrackingPlane>()._corners[1])
        };
        Vector3[] normals = new Vector3[]
        {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward
        };

        var mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.normals = normals;

        var plane = new GameObject("Screen plane");
        plane.transform.position = Vector3.zero;
        plane.transform.parent = rCamera.transform;
        plane.layer = 1;
        var meshFilter = plane.AddComponent<MeshFilter>();
        var meshRenderer = plane.AddComponent<MeshRenderer>();
        meshFilter.mesh = mesh;
        return plane;
    }
}
