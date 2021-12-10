using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp.Assets.Scripts
{
    public class TreeGenerator : MonoBehaviour
    {
        public GameObject _treeObject;
        public TreePlant _treePlant;
        public AttractorCloud _attractors;
        public float _timeLapsed;
        public const float _updateTime = 0.5f;
		public const int BranchSubdivisions = 8;

		[SerializeField]
		public DepthSensor _depthSensor;

		private bool _isPaused;
		public bool IsPaused => _isPaused;

		[SerializeField]
		public MeshFilter _meshFilter;

		// Start is called before the first frame update
		void Start()
        {
			Reset();
        }

		// Update is called once per frame
		void Update()
        {
			if (_isPaused) return;

            _timeLapsed += Time.deltaTime;

            if (_timeLapsed > _updateTime)
            {
                _timeLapsed = 0.0f;
                _treePlant.Grow();
				DrawMesh();
            }
        }

		/// <summary>
        /// Unpause growth
        /// </summary>
		public void StartGrowing()
        {
			_isPaused = false;
        }

		/// <summary>
        /// Pause growth
        /// </summary>
		public void StopGrowing()
        {
			_isPaused = true;
        }

        public void Reset()
		{
			_isPaused = true;
			_timeLapsed = 0f;
			Branch.ResetIdNum();
			AttractorPoint.ResetIdNum();
			_attractors = new AttractorCloud();
			_treePlant = new TreePlant(new Branch(), _attractors);
			_meshFilter.mesh.Clear();
		}

        /// <summary>
        /// Updates point cloud using data parsed from input image
        /// </summary>
        /// <param name="filename"></param>
        public void ParsePointCloudFromImage(string filename)
        {
			float scale = 6f;
			int numVert = 500;
			int numRot = 8;

			_depthSensor.LoadInputImage(filename);
			//Vector3[] vertices = _depthSensor.GetSampledPointsCenteredScaled(scale, numVert);
			Vector3[] vertices = _depthSensor.GetSampledPointsCenteredScaledRotated(scale, numVert);

			Vector3[] outVertices = new Vector3[numRot * numVert];

			// rotate vertices 8 times to create a hull
			for (int i = 0; i < numRot; i++)
            {
				float angle = i * 360f / numRot;
				Matrix4x4 rot = Matrix4x4.Rotate(Quaternion.AngleAxis(angle, Vector3.up));

				for (int j = 0; j < numVert; j++)
                {
					Vector3 rotVert = rot * vertices[j];
					outVertices[i * numVert + j] = rotVert;
                }
            }

			_attractors = new AttractorCloud(outVertices);
			_treePlant.SetAttractorCloud(_attractors);
        }

		/// <summary>
        /// Set maximum age of tree
        /// </summary>
        /// <param name="age"></param>
		public void SetTreeAge(int age)
        {
			_treePlant.SetMaxAge(age);
        }

		/// <summary>
        /// Redraws mesh of tree
        /// </summary>
		public void DrawMesh()
		{
			List<Branch> branches = new List<Branch>();
			_treePlant.Root.GetSubtree(branches);
			int numBranches = branches.Count;

			if (numBranches == 0) return;

			Vector3[] vertices = new Vector3[(numBranches + 1) * BranchSubdivisions];
			int[] triangles = new int[numBranches * BranchSubdivisions * 6];
			List<int> leafBranches = new List<int>();

			// create vertices
			for (int i = 0; i < numBranches; i++)
			{
				Branch b = branches[i];
				b.ComputeDiameter();
				int vert_idx = (int)b.Id * BranchSubdivisions;
				Quaternion q = Quaternion.FromToRotation(Vector3.up, b.Orientation);

				// flag which branches need leaves
				if (b.Degree >= 4)
                {
					leafBranches.Add(i);
                }

				for (int j = 0; j < BranchSubdivisions; j++)
				{
					float theta = j * 2f * Mathf.PI / BranchSubdivisions;
					Vector3 unrotated_pos = new Vector3(Mathf.Cos(theta) * b.Diameter, 0, Mathf.Sin(theta) * b.Diameter);
					Vector3 end_pos = q * unrotated_pos + b.PositionEnd;
					vertices[vert_idx + j] = end_pos - transform.position;

					// TODO if this branch is root, add vertices for base of trunk
					//vertices[numBranches * BranchSubdivisions + j] = b.Position + unrotated_pos - transform.position;
				}
			}

			// create faces
			for (int i = 0; i < numBranches; i++)
			{
				Branch b = branches[i];
				int face_idx = (int)b.Id * BranchSubdivisions * 6;
				int top_idx = (int)b.Id * BranchSubdivisions;
				int bot_idx = b.Parent == null ? numBranches * BranchSubdivisions : (int)b.Parent.Id * BranchSubdivisions;

				for (int j = 0; j < BranchSubdivisions; j++)
                {
					triangles[face_idx + j * 6] = bot_idx + j;
					triangles[face_idx + j * 6 + 1] = top_idx + j;

					if (j == BranchSubdivisions - 1)
                    {
						// last subdivision, wrap back around to first
						triangles[face_idx + j * 6 + 2] = top_idx;
						triangles[face_idx + j * 6 + 3] = bot_idx + j;
						triangles[face_idx + j * 6 + 4] = top_idx;
						triangles[face_idx + j * 6 + 5] = bot_idx;
					} else
                    {
						// continue around cylinder
						triangles[face_idx + j * 6 + 2] = top_idx + j + 1;
						triangles[face_idx + j * 6 + 3] = bot_idx + j;
						triangles[face_idx + j * 6 + 4] = top_idx + j + 1;
						triangles[face_idx + j * 6 + 5] = bot_idx + j + 1;
					}
                }
			}

			// add leaves
			float side = 0.15f;
			int startIdx = vertices.Length;
			Vector3[] verticesWithLeaves = new Vector3[vertices.Length + 4 * leafBranches.Count];
			int[] trianglesWithLeaves = new int[triangles.Length + 6 * leafBranches.Count];
			Array.Copy(vertices, verticesWithLeaves, vertices.Length);
			Array.Copy(triangles, trianglesWithLeaves, triangles.Length);

			for (int i = 0; i < leafBranches.Count; i++)
            {
				Branch b = branches[leafBranches[i]];
				Quaternion q = Quaternion.FromToRotation(Vector3.forward, b.Orientation);
				//float angle = 0f;
				float angle = Mathf.PerlinNoise(b.Id, b.Id) > 0.5 ? Branch.BranchingAngle : Branch.BranchingAngle + 180;
				Quaternion r = Quaternion.AngleAxis(angle, b.Orientation);
				Vector3 t = b.PositionEnd - transform.position;

				Vector3 botLeft = r * q * new Vector3(0, 0, 0) + t;
				Vector3 botRight = r * q * new Vector3(0, side, 0) + t;
				Vector3 topLeft = r * q * new Vector3(side, 0, 0) + t;
				Vector3 topRight = r * q * new Vector3(side, side, 0) + t;

				verticesWithLeaves[vertices.Length + 4 * i] = botLeft;
				verticesWithLeaves[vertices.Length + 4 * i + 1] = botRight;
				verticesWithLeaves[vertices.Length + 4 * i + 2] = topLeft;
				verticesWithLeaves[vertices.Length + 4 * i + 3] = topRight;

				trianglesWithLeaves[triangles.Length + 6 * i] = startIdx + 4 * i;
				trianglesWithLeaves[triangles.Length + 6 * i + 1] = startIdx + 4 * i + 1;
				trianglesWithLeaves[triangles.Length + 6 * i + 2] = startIdx + 4 * i + 2;

				trianglesWithLeaves[triangles.Length + 6 * i + 3] = startIdx + 4 * i + 1;
				trianglesWithLeaves[triangles.Length + 6 * i + 4] = startIdx + 4 * i + 3;
				trianglesWithLeaves[triangles.Length + 6 * i + 5] = startIdx + 4 * i + 2;
			}

			Mesh mesh = _meshFilter.mesh;
			mesh.Clear();
			mesh.vertices = verticesWithLeaves;
			mesh.triangles = trianglesWithLeaves;
			mesh.RecalculateNormals();
		}


		/// <summary>
		/// Draw gizmos for debugging
		/// </summary>
		void OnDrawGizmos()
        {
            if (_attractors == null || _treePlant == null) return;
            _attractors.DrawGizmos();
            _treePlant.DrawGizmos();
        }
    }
}
