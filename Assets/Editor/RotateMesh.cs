//https://answers.unity.com/questions/561786/how-to-export-obj-from-editor-with-rescaled-mesh.html?_ga=2.22974716.637888141.1592259629-1152228443.1579129359

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class RotateMesh : EditorWindow {
    
    private string error = "";
    
    [MenuItem("Window/Rotate Mesh %#r")]
    public static void Initialize() 
    {
        EditorWindow.GetWindow(typeof(RotateMesh));
    }

    void OnGUI() 
    {
        Transform curr = UnityEditor.Selection.activeTransform;
        GUILayout.Label ("Creates a clone of the game object with a rotated mesh\n" + 
            "so that the rotation will be (0,0,0) and the scale will\nbe (1,1,1).");
        GUILayout.Space(20);

        if (GUILayout.Button ("Rotate Mesh")) 
        {
            error = "";
            RotateTheMesh();
        }
        
        GUILayout.Space(20);
        GUILayout.Label(error);
    }
    
    void RotateTheMesh() 
    {
        List<Transform> children = new List<Transform>();
        Transform current = UnityEditor.Selection.activeTransform;

        MeshFilter mf;
        if (current == null) 
        {
            error = "No appropriate object selected.";
            Debug.Log (error);    
            return;
        }
        
        if (current.localScale.x < 0.0 || current.localScale.y < 0.0f || current.localScale.z < 0.0f) 
        {
            error = "Cannot process game object with negative scale values.";
            Debug.Log (error);
            return;
        }
        
        mf = current.GetComponent<MeshFilter>();
        if (mf == null || mf.sharedMesh == null) 
        {
            error = "No mesh on the selected object";
            Debug.Log (error);
            return;
        }
        
        // Create the duplicate game object
        GameObject go = Instantiate (current.gameObject) as GameObject;
        mf = go.GetComponent<MeshFilter>();
        mf.sharedMesh = Instantiate (mf.sharedMesh) as Mesh;
        current = go.transform;
        
        // Disconnect any child objects and same them for later
        foreach (Transform child in current) 
        {
            if (child != current) 
            {
                children.Add (child);
                child.parent = null;
            }
        }
        
        // Rotate and scale the mesh
        Vector3[] vertices = mf.sharedMesh.vertices;
        for (int i = 0; i < vertices.Length; i++) 
        {
            vertices[i] = current.TransformPoint(vertices[i]) - current.position;
        }
        mf.sharedMesh.vertices = vertices;

        
        // Fix the normals
        Vector3[] normals = mf.sharedMesh.normals;
        if (normals != null) 
        {
            for (int i = 0; i < normals.Length; i++)
                normals[i] = current.rotation * normals[i];
        }
        mf.sharedMesh.normals = normals;
        mf.sharedMesh.RecalculateBounds();
        
        current.transform.rotation = Quaternion.identity;
        current.localScale = new Vector3(1,1,1);
    
        // Restore the children
        foreach (Transform child in children) 
        {
            child.parent = current;
        }
        
        // Set selection to new game object
        UnityEditor.Selection.activeObject = current.gameObject;

        //--- Do a rudamentary fixup of mesh, box, and sphere colliders----
        MeshCollider mc = current.GetComponent<MeshCollider>();
        if (mc != null) 
        {
            mc.sharedMesh = mf.sharedMesh;
        }
        
        BoxCollider bc = current.GetComponent<BoxCollider>();
        if (bc != null) 
        {
            DestroyImmediate(bc);
            current.gameObject.AddComponent<BoxCollider>();
        }
        SphereCollider sc = current.GetComponent<SphereCollider>();
        if (sc != null) 
        {
            DestroyImmediate(sc);
            current.gameObject.AddComponent<SphereCollider>();
        }
        
        if (current.GetComponent<Collider>()) 
        {
            error = "Be sure to verify size of collider.";
        }
    }
}