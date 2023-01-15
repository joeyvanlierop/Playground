using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Cel
{
    public class GrassInstancer : MonoBehaviour
    {
        public Mesh mesh;
        public Material material;

        private uint[] args = new uint[5];
        private ComputeBuffer argsBuffer;
        private ComputeBuffer drawDataBuffer;
        private List<DrawData> instances;
        private MaterialPropertyBlock mpb;
        private Terrain terrain;

        private void Awake()
        {
            terrain = GetComponent<Terrain>();

            instances = new List<DrawData>();

            argsBuffer = new ComputeBuffer(5, sizeof(uint), ComputeBufferType.IndirectArguments);
            // Meshes with sub-meshes needs more structure, this assumes a single sub-mesh
            args[0] = mesh.GetIndexCount(0);

            mpb = new MaterialPropertyBlock();

            SpawnExample();
        }

        private void LateUpdate()
        {
            // Only needs to be called if "instances" changed
            PushDrawData();
            mpb.SetBuffer("_DrawData", drawDataBuffer);

            args[1] = (uint)instances.Count;
            argsBuffer.SetData(args);

            Graphics.DrawMeshInstancedIndirect(
                mesh, 0, material,
                new Bounds(Vector3.zero, Vector3.one * 1000f),
                argsBuffer, 0,
                mpb
            );
        }

        private void OnDestroy()
        {
            argsBuffer?.Release();
            drawDataBuffer?.Release();
        }

        private void PushDrawData()
        {
            if (drawDataBuffer == null || drawDataBuffer.count < instances.Count)
            {
                drawDataBuffer?.Release();
                drawDataBuffer = new ComputeBuffer(instances.Count, Marshal.SizeOf<DrawData>());
            }

            drawDataBuffer.SetData(instances);
        }

        private void SpawnExample()
        {
            instances.Clear();
            for (var x = 0; x < terrain.terrainData.size.x; x++)
            {
                for (var z = 0; z < terrain.terrainData.size.z; z++)
                {
                    var meshPos = new Vector3(x, 0, z) + transform.position;
                    meshPos.y = terrain.SampleHeight(meshPos);
                    Debug.Log(meshPos);
                    instances.Add(new DrawData
                    {
                        // Pos = Random.insideUnitSphere * 100f,
                        Pos = meshPos,
                        Rot = Quaternion.identity,
                        Scale = Vector3.one
                    });
                }
            }
        }

        private struct DrawData
        {
            public Vector3 Pos;
            public Quaternion Rot;
            public Vector3 Scale;
        }
    }
}