using System.Collections.Generic;
using JetBrains.Annotations;
using PCB.Math;
using Unity.Mathematics;
using UnityEngine;

namespace PCB.Icosahedron
{
    [ExecuteAlways]
    public class ProceduralIcosahedronNode : MonoBehaviour
    {
        public bool subdivide = false;
        public bool undivide = false;

        public bool subdivided = false;
        
        public ProceduralIcosahedronNode leftNeighbor;
        public ProceduralIcosahedronNode rightNeighbor;
        public ProceduralIcosahedronNode bottomNeighbor;

        public string index;
        
        public uint levelOfDetail;
        
        public ProceduralIcosahedronSystemRoot root;

        [CanBeNull] public ProceduralIcosahedronNode parent;

        [CanBeNull] public Mesh chunkMesh;

        private SphericalCoordinateDegrees _top;
        private SphericalCoordinateDegrees _bottomLeft;
        private SphericalCoordinateDegrees _bottomRight;
        
        private double3 _topCartesian;
        private double3 _bottomLeftCartesian;
        private double3 _bottomRightCartesian;

        public SphericalCoordinateDegrees Top
        {
            get => this._top;
            set
            {
                this._top = value;
                this._topCartesian = value.ToCartesian();
                this.RecalculateCenter();
            }
        }
        
        public SphericalCoordinateDegrees BottomLeft
        {
            get => this._bottomLeft;
            set
            {
                this._bottomLeft = value;
                this._bottomLeftCartesian = value.ToCartesian();
                this.RecalculateCenter();
            }
        }

        public SphericalCoordinateDegrees BottomRight
        {
            get => this._bottomRight;
            set
            {
                this._bottomRight = value;
                this._bottomRightCartesian = value.ToCartesian();
                this.RecalculateCenter();
            }
        }
        
        public SphericalCoordinateDegrees Center { get; private set; }
        
        public double3 TopCartesian
        {
            get => this._topCartesian;
            set
            {
                this._topCartesian = value;
                this.Top = SphericalCoordinateDegrees.FromCartesian(value);
            }
        }

        public double3 BottomLeftCartesian
        {
            get => this._bottomLeftCartesian;
            set
            {
                this._bottomLeftCartesian = value;
                this._bottomLeft = SphericalCoordinateDegrees.FromCartesian(value);
                this.RecalculateCenter();
            }
        }

        public double3 BottomRightCartesian
        {
            get => this._bottomRightCartesian;
            set
            {
                this._bottomRightCartesian = value;
                this._bottomRight = SphericalCoordinateDegrees.FromCartesian(value);
                this.RecalculateCenter();
            }
        }

        public double3 CenterCartesian { get; private set; }

        public bool showDebug = true;
        public bool regenerateImmediately;
        public bool regenerateNow;
        
        public List<ProceduralIcosahedronNode> childNodes = new List<ProceduralIcosahedronNode>();

        public void Update()
        {
            if (this.subdivide && this.undivide)
            {
                this.subdivide = false;
                this.undivide = false;
            }

            if (this.subdivide)
            {
                this.subdivide = false;
                this.Subdivide();
            }

            if (this.undivide)
            {
                this.undivide = false;
                this.Undivide();
            }
            
            if (this.regenerateNow)
            {
                this.Regenerate();
            }
            
            if (!this.subdivided)
            {
                if (!this.chunkMesh)
                {
                    this.AddChunkMesh();
                }

                if (!this.root.showDebug || !this.showDebug)
                {
                    return;
                }
                
                double3 leftCenterCartesian =
                    SphericalCoordinateDegrees
                        .Interpolate(this._top, this._bottomLeft, 0.5)
                        .ToCartesian();
                double3 rightCenterCartesian = SphericalCoordinateDegrees
                    .Interpolate(this._top, this._bottomRight, 0.5)
                    .ToCartesian();
                double3 bottomCenterCartesian = SphericalCoordinateDegrees
                    .Interpolate(this._bottomLeft, this._bottomRight, 0.5)
                    .ToCartesian();
                
                
                Vector3 topPoint = new Vector3(
                    (float)_topCartesian.x, 
                    (float)_topCartesian.y, 
                    (float)_topCartesian.z);
                Vector3 bottomLeftPoint = new Vector3(
                    (float)_bottomLeftCartesian.x, 
                    (float)_bottomLeftCartesian.y,
                    (float)_bottomLeftCartesian.z);
                Vector3 bottomRightPoint = new Vector3(
                    (float)_bottomRightCartesian.x, 
                    (float)_bottomRightCartesian.y,
                    (float)_bottomRightCartesian.z);
                Vector3 leftCenterPoint = new Vector3(
                    (float)leftCenterCartesian.x, 
                    (float)leftCenterCartesian.y,
                    (float)leftCenterCartesian.z);
                Vector3 rightCenterPoint = new Vector3(
                    (float)rightCenterCartesian.x, 
                    (float)rightCenterCartesian.y,
                    (float)rightCenterCartesian.z);
                Vector3 bottomCenterPoint = new Vector3(
                    (float)bottomCenterCartesian.x, 
                    (float)bottomCenterCartesian.y,
                    (float)bottomCenterCartesian.z);

                if (this.leftNeighbor.subdivided)
                {
                    Debug.DrawLine(this.root.transform.position + topPoint, this.root.transform.position + leftCenterPoint, Color.red, 0.0f);
                    Debug.DrawLine(this.root.transform.position + leftCenterPoint, this.root.transform.position + bottomLeftPoint, Color.red, 0.0f);
                    Debug.DrawLine(this.root.transform.position + bottomRightPoint, this.root.transform.position + leftCenterPoint, Color.red, 0.0f);
                }
                else
                {
                    Debug.DrawLine(this.root.transform.position + topPoint, this.root.transform.position + bottomLeftPoint, Color.red, 0.0f);
                }

                if (this.rightNeighbor.subdivided)
                {
                    Debug.DrawLine(this.root.transform.position + topPoint, this.root.transform.position + rightCenterPoint, Color.red, 0.0f);
                    Debug.DrawLine(this.root.transform.position + rightCenterPoint, this.root.transform.position + bottomRightPoint, Color.red, 0.0f);
                    Debug.DrawLine(this.root.transform.position + rightCenterPoint, this.root.transform.position + bottomLeftPoint, Color.red, 0.0f);
                }
                else
                {
                    Debug.DrawLine(this.root.transform.position + bottomRightPoint, this.root.transform.position + topPoint, Color.red, 0.0f);
                }

                if (this.bottomNeighbor.subdivided)
                {
                    Debug.DrawLine(this.root.transform.position + bottomLeftPoint, this.root.transform.position + bottomCenterPoint, Color.red,
                        0.0f);
                    Debug.DrawLine(this.root.transform.position + bottomCenterPoint, this.root.transform.position + bottomRightPoint, Color.red,
                        0.0f);
                    Debug.DrawLine(this.root.transform.position + topPoint, this.root.transform.position + bottomCenterPoint, Color.red,
                        0.0f);
                }
                else
                {
                    Debug.DrawLine(this.root.transform.position + bottomLeftPoint, this.root.transform.position + bottomRightPoint, Color.red,
                        0.0f);
                }
            }
        }

        public void Regenerate()
        {
            Debug.Log("regenerating//");
            
            if (this.subdivided)
            {
                foreach (ProceduralIcosahedronNode child in this.childNodes)
                {
                    child.Regenerate();
                }
            }
            else
            {
                this.RemoveChunkMesh();
                this.AddChunkMesh();
            }
        }
        
        public void Subdivide()
        {
            if (this.subdivided)
            {
                return;
            }
            
            if (this.leftNeighbor.levelOfDetail < this.levelOfDetail)
            {
                return;
            }

            if (this.rightNeighbor.levelOfDetail < this.levelOfDetail)
            {
                return;
            }

            if (this.bottomNeighbor.levelOfDetail < this.levelOfDetail)
            {
                return;
            }

            this.subdivided = true;
            
            this.RemoveChunkMesh();

            SphericalCoordinateDegrees center = SphericalCoordinateDegrees.TriangleInterpolateCenter(
                this._top,
                this._bottomRight,
                this._bottomLeft);

            double3 centerCartesian = center.ToCartesian();

            Vector3 centerPoint =
                new Vector3((float)centerCartesian.x, (float)centerCartesian.y, (float)centerCartesian.z);
            
            SphericalCoordinateDegrees leftCenter = SphericalCoordinateDegrees.Interpolate(this._top, this._bottomLeft, 0.5);
            SphericalCoordinateDegrees rightCenter = SphericalCoordinateDegrees.Interpolate(this._top, this._bottomRight, 0.5);
            SphericalCoordinateDegrees bottomCenter =
                SphericalCoordinateDegrees.Interpolate(this._bottomLeft, this._bottomRight, 0.5);

            for (int i = 0; i < 4; i++)
            {
                string newIndex = $"{this.index}.{i}";
                
                GameObject newObject = new GameObject($"Test {newIndex}");
                newObject.transform.parent = this.gameObject.transform;
                newObject.transform.localPosition = new Vector3();
                newObject.transform.rotation = Quaternion.identity;
                newObject.transform.localScale = Vector3.one;
                newObject.AddComponent<ProceduralIcosahedronNode>();
                
                ProceduralIcosahedronNode node = newObject.GetComponent<ProceduralIcosahedronNode>();

                this.childNodes.Add(node);
                node.index = newIndex;
                node.parent = this;
                node.root = this.root;
                node.levelOfDetail = this.levelOfDetail + 1;
            }

            this.childNodes[0]._top = this._top;
            this.childNodes[0]._bottomLeft = leftCenter;
            this.childNodes[0]._bottomRight = rightCenter;
            
            this.childNodes[1]._top = rightCenter;
            this.childNodes[1]._bottomLeft = bottomCenter;
            this.childNodes[1]._bottomRight = this._bottomRight;
            
            this.childNodes[2]._top = bottomCenter;
            this.childNodes[2]._bottomLeft = rightCenter;
            this.childNodes[2]._bottomRight = leftCenter;
            
            this.childNodes[3]._top = leftCenter;
            this.childNodes[3]._bottomLeft = this._bottomLeft;
            this.childNodes[3]._bottomRight = bottomCenter;

            this.childNodes[0].leftNeighbor = this.leftNeighbor;
            this.childNodes[0].rightNeighbor = this.rightNeighbor;
            this.childNodes[0].bottomNeighbor = this.childNodes[2];
            
            this.childNodes[1].leftNeighbor = this.childNodes[2];
            this.childNodes[1].rightNeighbor = this.rightNeighbor;
            this.childNodes[1].bottomNeighbor = this.bottomNeighbor;
            
            this.childNodes[2].leftNeighbor = this.childNodes[1];
            this.childNodes[2].rightNeighbor = this.childNodes[3];
            this.childNodes[2].bottomNeighbor = this.childNodes[0];
            
            this.childNodes[3].leftNeighbor = this.leftNeighbor;
            this.childNodes[3].rightNeighbor = this.childNodes[2];
            this.childNodes[3].bottomNeighbor = this.bottomNeighbor;

            for (int i = 0; i < this.childNodes.Count; i++)
            {
                SphericalCoordinateDegrees centerSpherical = SphericalCoordinateDegrees.TriangleInterpolateCenter(
                    this.childNodes[i]._top,
                    this.childNodes[i]._bottomLeft,
                    this.childNodes[i]._bottomRight);

                double3 nodeCenterCartesian = centerSpherical.ToCartesian();
                
                Vector3 nodeCenterPoint = new Vector3((float) nodeCenterCartesian.x, (float) nodeCenterCartesian.y, (float) nodeCenterCartesian.z);

                Vector3 offset = nodeCenterPoint - centerPoint;

                this.childNodes[i].transform.localPosition = offset;
            }
        }

        public void Undivide(bool force = false)
        {
            if (!this.subdivided)
            {
                return;
            }
            
            if (this.leftNeighbor.levelOfDetail > this.levelOfDetail)
            {
                return;
            }
            
            if (this.rightNeighbor.levelOfDetail > this.levelOfDetail)
            {
                return;
            }
            if (this.bottomNeighbor.levelOfDetail > this.levelOfDetail)
            {
                return;
            }

            this.subdivided = false;
            
            foreach (ProceduralIcosahedronNode childNode in this.childNodes)
            {
                childNode.Undivide();
                GameObject.DestroyImmediate(childNode.gameObject);
            }
            //
            this.childNodes.Clear();
            
            this.AddChunkMesh();
        }
        
        private void AddChunkMesh()
        {
            this.gameObject.AddComponent<MeshFilter>();
            this.gameObject.AddComponent<MeshRenderer>();

            this.chunkMesh = this.GenerateChunkMesh();
            
            MeshFilter meshFilter = this.gameObject.GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = this.gameObject.GetComponent<MeshRenderer>();

            meshFilter.sharedMesh = this.chunkMesh;
            meshRenderer.sharedMaterial = this.root.planetMaterial;
        }

        private Mesh GenerateChunkMesh()
        {
            Mesh mesh = new Mesh();
            
            if (this.root.chunkSubdivisionCount > 8)
            {
                Debug.LogError("Cannot have a chunk subdivision count greater than 8");
                return mesh;
            }
            
            ulong majorTriangleCount = 0b1u << (int) this.root.chunkSubdivisionCount;
            ulong majorVerticeCount = majorTriangleCount + 1;

            ulong totalTriangleCount = majorTriangleCount * majorTriangleCount;

            ulong totalIndiceCount = totalTriangleCount * 3;
            
            ulong verticeCount = SumSmallerEqualN(majorVerticeCount);

            List<Vector3> vertices = new List<Vector3>((int)verticeCount);
            
            vertices.Add(SphericalToVector3(this._top));

            double majorVertexInterpolationStep = 1.0 / (double)majorTriangleCount;
            
            for (int i = 1; i < (int) majorTriangleCount; i++)
            {
                double currentMajorInterpolation = majorVertexInterpolationStep * i;
                
                if (currentMajorInterpolation >= 1.0)
                {
                    Debug.LogError($"Major interpolation for row {i} is greater than 1.0, is: {currentMajorInterpolation}");
                }
                
                SphericalCoordinateDegrees beginning = SphericalCoordinateDegrees.Interpolate(this._top, this._bottomRight, currentMajorInterpolation);
                SphericalCoordinateDegrees end =
                    SphericalCoordinateDegrees.Interpolate(this._top, this._bottomLeft, currentMajorInterpolation);

                vertices.Add(SphericalToVector3(beginning));

                double minorVertexInterpolationStep = 1.0 / (double)i;
                
                for (int j = 1; j < i; j++)
                {
                    double currentMinorInterpolation = minorVertexInterpolationStep * j;
                    
                    if (currentMinorInterpolation >= 1.0)
                    {
                        Debug.LogError($"Minor interpolation for row {i}, {j} is greater than 1.0, is: {currentMinorInterpolation}");
                    }
                    
                    SphericalCoordinateDegrees vertex = SphericalCoordinateDegrees.Interpolate(beginning, end, currentMinorInterpolation);
                    
                    vertices.Add(SphericalToVector3(vertex));
                }
                
                vertices.Add(SphericalToVector3(end));
            }
            
            vertices.Add(SphericalToVector3(this._bottomRight));
            
            for (int i = 1; i < (int) majorTriangleCount; i++)
            {
                double currentMinorInterpolation = majorVertexInterpolationStep * i;

                if (currentMinorInterpolation >= 1.0)
                {
                    Debug.LogError($"Bottom Minor interpolation for {i} is greater than 1.0, is: {currentMinorInterpolation}");
                }
                
                SphericalCoordinateDegrees vertex = SphericalCoordinateDegrees.Interpolate(this._bottomRight, this._bottomLeft, currentMinorInterpolation);
                    
                vertices.Add(SphericalToVector3(vertex));
            }
            
            vertices.Add(SphericalToVector3(this._bottomLeft));

            List<Vector3> normals = new List<Vector3>(vertices.Count);

            foreach (Vector3 vertex in vertices)
            {
                normals.Add(vertex.normalized);
            }
            
            mesh.SetVertices(vertices);
            mesh.SetNormals(normals);

            List<int> indices = new List<int>((int) totalIndiceCount);

            for (ulong i = 0; i < majorTriangleCount; i++)
            {
                ulong firstTopIndex = SumSmallerEqualN((ulong)i);
                ulong firstBottomIndex = SumSmallerEqualN((ulong)i + 1u);

                ulong rowTriangles = 1 + ((ulong)i * 2);

                for (ulong j = 0; j < rowTriangles; j++)
                {
                    if (j % 2 == 0)
                    {
                        ulong offset = j / 2;
                        
                        indices.Add((int)(firstTopIndex + offset));
                        indices.Add((int)(firstBottomIndex + offset));
                        indices.Add((int)(firstBottomIndex + offset + 1));
                    }
                    else
                    {
                        ulong topOffset = ((j - 1) / 2) + 1;
                        ulong bottomOffset = (j - 1) / 2;
                        
                        indices.Add((int)(firstBottomIndex + topOffset));
                        indices.Add((int)(firstTopIndex + bottomOffset + 1));
                        indices.Add((int)(firstTopIndex + bottomOffset));
                    }
                }
            }
            
            mesh.SetTriangles(indices, 0);
            
            mesh.RecalculateBounds();
            mesh.RecalculateTangents();
            
            return mesh;
        }

        private void RemoveChunkMesh()
        {
            GameObject.DestroyImmediate(this.gameObject.GetComponent<MeshRenderer>());
            GameObject.DestroyImmediate(this.gameObject.GetComponent<MeshFilter>());

            if (this.chunkMesh)
            {
                this.chunkMesh.Clear();
                this.chunkMesh = null;
            }
        }

        private void RecalculateCenter()
        {
            this.Center = SphericalCoordinateDegrees.TriangleInterpolateCenter(
                this._top,
                this._bottomLeft,
                this._bottomRight);
        }

        private static ulong SumSmallerN(ulong n)
        {
            if (n is 0 or 1)
            {
                return 0;
            }
            
            // n * (n-1) / 2
            return (n * (n - 1)) >> 1;
        }
        
        private static ulong SumSmallerEqualN(ulong n)
        {
            return n + SumSmallerN(n);
        }
        
        private static Vector3 SphericalToVector3(SphericalCoordinateDegrees spherical)
        {
            double3 cartesian = spherical.ToCartesian();

            return new Vector3((float) cartesian.x, (float) cartesian.y, (float) cartesian.z);
        }
        
        private double3 InterpolateTriangleSideMiddlePoint(double3 a, double3 b)
        {
            return math.normalize(math.lerp(a, b, 0.5)) * this.root.Radius;
        }
    }
}