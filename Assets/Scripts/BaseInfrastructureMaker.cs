using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MapReader))]
abstract class BaseInfrastructureMaker : MonoBehaviour
{
    protected MapReader map;

    void Awake()
    {
        map = GetComponent<MapReader>();
    }
    protected Vector3 GetCentre(OsmWay way)
    {
        Vector3 total = Vector3.zero;

        foreach (var id in way.NodeIDs)
        {
            total += map.nodes[id];
        }

        return total / way.NodeIDs.Count;
    }

    protected void CreateObject(OsmWay way, Material mat, string objectName)
    {
        // Make sure we have some name to display
        objectName = string.IsNullOrEmpty(objectName) ? "OsmWay" : objectName;

        // Create an instance of the object and place it in the centre of its points
        GameObject go = new GameObject(objectName);
        Vector3 localOrigin = GetCentre(way);
        go.transform.position = localOrigin - map.bounds.Centre;

        // Add the mesh filter and renderer components to the object
        MeshFilter mf = go.AddComponent<MeshFilter>();
        MeshRenderer mr = go.AddComponent<MeshRenderer>();

        // Apply the material
        mr.material = mat;

        // Create the collections for the object's vertices, indices, UVs etc.
        List<Vector3> vectors = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> indices = new List<int>();

        // Call the child class' object creation code
        OnObjectCreated(way, localOrigin, vectors, normals, uvs, indices);

        // Apply the data to the mesh
        mf.mesh.vertices = vectors.ToArray();
        mf.mesh.triangles = indices.ToArray();
        mf.mesh.normals = normals.ToArray();
        mf.mesh.uv = uvs.ToArray();
    }
    protected abstract void OnObjectCreated(OsmWay way, Vector3 origin, List<Vector3> vectors, List<Vector3> normals, List<Vector2> uvs, List<int> indices);
}