using System.Collections.Generic;
using PCB.Math;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace PCB.Icosahedron
{
    [ExecuteAlways]
    public class ProceduralIcosahedronSystemRoot : MonoBehaviour
    {
        public bool regenerate = true;
        
        public bool generated = false;

        public Transform childNodeHolder;
        
        public List<ProceduralIcosahedronNode> childNodes = new List<ProceduralIcosahedronNode>();

        [Range(0, 8)]
        public uint chunkSubdivisionCount = 7;

        private uint _lastChunkSubdivisionCount;
        
        [FormerlySerializedAs("radius")] public double baseRadius = 10.0;
        
        public double scale = 1.0;

        public bool showDebug = true;
        public double Radius => this.baseRadius / this.scale;

        public Material planetMaterial;
        
        private void Update()
        {
            if (this.regenerate)
            {
                this.regenerate = false;
                this.generated = false;

                foreach (ProceduralIcosahedronNode node in this.childNodes)
                {
                    node.Undivide(true);
                    GameObject.DestroyImmediate(node.gameObject);
                }

                this.childNodes.Clear();
            }
            
            if (this.generated)
            {
                if (this.chunkSubdivisionCount != this._lastChunkSubdivisionCount)
                {
                    this._lastChunkSubdivisionCount = this.chunkSubdivisionCount; 
                    foreach (ProceduralIcosahedronNode childNode in this.childNodes)
                    {
                        childNode.Regenerate();
                    }
                }
                
                return;
            }
            
            this.generated = true;

            this._lastChunkSubdivisionCount = this.chunkSubdivisionCount;
            
            float phi = (float)((1 + math.sqrt(5.0)) / 2.0);
                
            List<Vector3> verticesCartesian = new List<Vector3>();

            verticesCartesian.Add(new Vector3(0.0f, phi, -1.0f));
            verticesCartesian.Add(new Vector3(0.0f, phi, 1.0f));
            verticesCartesian.Add(new Vector3(phi, 1.0f, 0.0f));
            verticesCartesian.Add(new Vector3(1.0f, 0, -phi));
            verticesCartesian.Add(new Vector3(-1.0f, 0, -phi));
            verticesCartesian.Add(new Vector3(-phi, 1.0f, 0.0f));
            verticesCartesian.Add(new Vector3(1.0f, 0, phi));
            verticesCartesian.Add(new Vector3(phi, -1.0f, 0.0f));
            verticesCartesian.Add(new Vector3(0.0f, -phi, -1.0f));
            verticesCartesian.Add(new Vector3(-phi, -1.0f, 0.0f));
            verticesCartesian.Add(new Vector3(-1.0f, 0, phi));
            verticesCartesian.Add(new Vector3(0.0f, -phi, 1.0f));
                
            SphericalCoordinateDegrees topRotationDegrees = SphericalCoordinateDegrees.FromCartesian(new double3(verticesCartesian[0].x, verticesCartesian[0].y, verticesCartesian[0].z));
            
            Matrix4x4 yRotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(0.0f, (float) -topRotationDegrees.azimuth, 0.0f));

            Matrix4x4 xRotationMatrix =
                Matrix4x4.Rotate(Quaternion.Euler((float)-topRotationDegrees.polar, 0.0f, 0.0f));

            Matrix4x4 yCounterRotationMatrix =
                Matrix4x4.Rotate(Quaternion.Euler(0.0f, (float)topRotationDegrees.azimuth, 0.0f));
            
            for (int i = 0; i < verticesCartesian.Count; i++)
            {
                verticesCartesian[i] = yCounterRotationMatrix * xRotationMatrix * yRotationMatrix * new Vector4(verticesCartesian[i].x, verticesCartesian[i].y, verticesCartesian[i].z);
            }
                
            List<SphericalCoordinateRadians> vertices = new List<SphericalCoordinateRadians>();

            foreach (Vector3 vertex in verticesCartesian)
            {
                vertices.Add(SphericalCoordinateRadians.FromCartesian(new double3(vertex.x, vertex.y, vertex.z)));
            }
            
            double radial = this.baseRadius / this.scale;

            for (int i = 0; i < vertices.Count; i++)
            {
                SphericalCoordinateRadians vertex = vertices[i];
                vertex.radial = radial;
                vertices[i] = vertex;
            }

            vertices[0] = new SphericalCoordinateRadians(radial, 0.0, 0.0);
            vertices[11] = new SphericalCoordinateRadians(radial, math.PI_DBL, 0.0);
            
            List<(SphericalCoordinateRadians, SphericalCoordinateRadians, SphericalCoordinateRadians)> nodes 
                = new List<(SphericalCoordinateRadians, SphericalCoordinateRadians, SphericalCoordinateRadians)>();
            
            nodes.Add((vertices[0], vertices[2], vertices[1]));
            nodes.Add((vertices[0], vertices[3], vertices[2]));
            nodes.Add((vertices[0], vertices[4], vertices[3]));
            nodes.Add((vertices[0], vertices[5], vertices[4]));
            nodes.Add((vertices[0], vertices[1], vertices[5]));
            
            nodes.Add((vertices[6], vertices[1], vertices[2]));
            nodes.Add((vertices[2], vertices[7], vertices[6]));
            nodes.Add((vertices[7], vertices[2], vertices[3]));
            nodes.Add((vertices[3], vertices[8], vertices[7]));
            nodes.Add((vertices[8], vertices[3], vertices[4]));
            nodes.Add((vertices[4], vertices[9], vertices[8]));
            nodes.Add((vertices[9], vertices[4], vertices[5]));
            nodes.Add((vertices[5], vertices[10], vertices[9]));
            nodes.Add((vertices[10], vertices[5], vertices[1]));
            nodes.Add((vertices[1], vertices[6], vertices[10]));
            
            nodes.Add((vertices[11], vertices[6], vertices[7]));
            nodes.Add((vertices[11], vertices[7], vertices[8]));
            nodes.Add((vertices[11], vertices[8], vertices[9]));
            nodes.Add((vertices[11], vertices[9], vertices[10]));
            nodes.Add((vertices[11], vertices[10], vertices[6]));
            
            for (int i = 0; i < nodes.Count; i++)
            {
                Transform parent = this.childNodeHolder 
                    ? this.childNodeHolder 
                    : this.transform;
                
                GameObject nodeObject = new GameObject($"Test {i}");
                nodeObject.transform.parent = parent;

                SphericalCoordinateRadians childPositionSpherical =
                    SphericalCoordinateRadians.TriangleInterpolateCenter(nodes[i].Item1, nodes[i].Item2, nodes[i].Item3);

                double3 childPositionCartesian = childPositionSpherical.ToCartesian();
                
                nodeObject.transform.localPosition = new Vector3((float) childPositionCartesian.x, (float) childPositionCartesian.y, (float) childPositionCartesian.z);
                nodeObject.transform.rotation = Quaternion.identity;
                nodeObject.transform.localScale = Vector3.one;
                nodeObject.AddComponent<ProceduralIcosahedronNode>();

                ProceduralIcosahedronNode node = nodeObject.GetComponent<ProceduralIcosahedronNode>();

                this.childNodes.Add(node);

                node.index = $"{i}";
                node.root = this;
                node.Top = nodes[i].Item1.ToDegrees();
                node.BottomLeft = nodes[i].Item2.ToDegrees();
                node.BottomRight = nodes[i].Item3.ToDegrees();
            }

            this.childNodes[0].leftNeighbor = this.childNodes[1];
            this.childNodes[0].rightNeighbor = this.childNodes[4];
            this.childNodes[0].bottomNeighbor = this.childNodes[5];
            
            this.childNodes[1].leftNeighbor = this.childNodes[2];
            this.childNodes[1].rightNeighbor = this.childNodes[0];
            this.childNodes[1].bottomNeighbor = this.childNodes[7];
            
            this.childNodes[2].leftNeighbor = this.childNodes[3];
            this.childNodes[2].rightNeighbor = this.childNodes[1];
            this.childNodes[2].bottomNeighbor = this.childNodes[9];
            
            this.childNodes[3].leftNeighbor = this.childNodes[4];
            this.childNodes[3].rightNeighbor = this.childNodes[3];
            this.childNodes[3].bottomNeighbor = this.childNodes[11];
            
            this.childNodes[4].leftNeighbor = this.childNodes[0];
            this.childNodes[4].rightNeighbor = this.childNodes[3];
            this.childNodes[4].bottomNeighbor = this.childNodes[13];
            
            this.childNodes[5].leftNeighbor = this.childNodes[14];
            this.childNodes[5].rightNeighbor = this.childNodes[6];
            this.childNodes[5].bottomNeighbor = this.childNodes[0];
            
            this.childNodes[6].leftNeighbor = this.childNodes[7];
            this.childNodes[6].rightNeighbor = this.childNodes[5];
            this.childNodes[6].bottomNeighbor = this.childNodes[15];
            
            this.childNodes[7].leftNeighbor = this.childNodes[6];
            this.childNodes[7].rightNeighbor = this.childNodes[8];
            this.childNodes[7].bottomNeighbor = this.childNodes[1];
            
            this.childNodes[8].leftNeighbor = this.childNodes[9];
            this.childNodes[8].rightNeighbor = this.childNodes[7];
            this.childNodes[8].bottomNeighbor = this.childNodes[16];
            
            this.childNodes[9].leftNeighbor = this.childNodes[8];
            this.childNodes[9].rightNeighbor = this.childNodes[10];
            this.childNodes[9].bottomNeighbor = this.childNodes[2];
            
            this.childNodes[10].leftNeighbor = this.childNodes[11];
            this.childNodes[10].rightNeighbor = this.childNodes[9];
            this.childNodes[10].bottomNeighbor = this.childNodes[17];
            
            this.childNodes[11].leftNeighbor = this.childNodes[10];
            this.childNodes[11].rightNeighbor = this.childNodes[12];
            this.childNodes[11].bottomNeighbor = this.childNodes[3];
            
            this.childNodes[12].leftNeighbor = this.childNodes[13];
            this.childNodes[12].rightNeighbor = this.childNodes[11];
            this.childNodes[12].bottomNeighbor = this.childNodes[18];
            
            this.childNodes[13].leftNeighbor = this.childNodes[12];
            this.childNodes[13].rightNeighbor = this.childNodes[14];
            this.childNodes[13].bottomNeighbor = this.childNodes[4];
            
            this.childNodes[14].leftNeighbor = this.childNodes[5];
            this.childNodes[14].rightNeighbor = this.childNodes[13];
            this.childNodes[14].bottomNeighbor = this.childNodes[19];
            
            this.childNodes[15].leftNeighbor = this.childNodes[19];
            this.childNodes[15].rightNeighbor = this.childNodes[16];
            this.childNodes[15].bottomNeighbor = this.childNodes[6];
            
            this.childNodes[16].leftNeighbor = this.childNodes[15];
            this.childNodes[16].rightNeighbor = this.childNodes[17];
            this.childNodes[16].bottomNeighbor = this.childNodes[8];
            
            this.childNodes[17].leftNeighbor = this.childNodes[16];
            this.childNodes[17].rightNeighbor = this.childNodes[18];
            this.childNodes[17].bottomNeighbor = this.childNodes[10];
            
            this.childNodes[18].leftNeighbor = this.childNodes[17];
            this.childNodes[18].rightNeighbor = this.childNodes[19];
            this.childNodes[18].bottomNeighbor = this.childNodes[12];
            
            this.childNodes[19].leftNeighbor = this.childNodes[18];
            this.childNodes[19].rightNeighbor = this.childNodes[15];
            this.childNodes[19].bottomNeighbor = this.childNodes[14];
        }
    }
}