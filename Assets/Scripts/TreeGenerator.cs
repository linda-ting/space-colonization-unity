﻿using System.Collections;
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
            _timeLapsed = 0.0f;
            _attractors = new AttractorCloud();
            _treePlant = new TreePlant(new Branch(), _attractors);
			_isPaused = true;

			// TODO add user input to upload image
			/*
			if (_depthSensor != null)
			{
				ParsePointCloudFromImage("Assets/Images/tree_bottom_2.jpeg");
			}
			*/
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

		/// <summary>
        /// Updates point cloud using data parsed from input image
        /// </summary>
        /// <param name="filename"></param>
		public void ParsePointCloudFromImage(string filename)
        {
			_depthSensor.LoadInputImage(filename);
			//Vector3[] vertices = _depthSensor.GetSampledPointsCenteredScaled(6, 500);
			Vector3[] vertices = _depthSensor.GetSampledPointsCenteredScaledRotated(6, 500);
			_attractors = new AttractorCloud(vertices);
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

			Vector3[] vertices = new Vector3[(numBranches + 1) * BranchSubdivisions];
			int[] triangles = new int[numBranches * BranchSubdivisions * 6];

			// create vertices
			for (int i = 0; i < numBranches; i++)
			{
				Branch b = branches[i];
				b.ComputeDiameter();
				int vert_idx = (int)b.Id * BranchSubdivisions;
				Quaternion q = Quaternion.FromToRotation(Vector3.up, b.Orientation);

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

			Mesh mesh = _meshFilter.mesh;
			mesh.Clear();
			mesh.vertices = vertices;
			mesh.triangles = triangles;
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
