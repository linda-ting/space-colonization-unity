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
		public const int BranchSubdivisions = 4;

		[SerializeField]
		public MeshFilter _meshFilter;

		// Start is called before the first frame update
		void Start()
        {
            _timeLapsed = 0.0f;
            _attractors = new AttractorCloud();
            _treePlant = new TreePlant(new Branch(), _attractors);
		}

		// Update is called once per frame
		void Update()
        {
            _timeLapsed += Time.deltaTime;

            if (_timeLapsed > _updateTime)
            {
                _timeLapsed = 0.0f;
                _treePlant.Grow();
				DrawMesh();
            }
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
				int vert_idx = (int)b.Id * BranchSubdivisions;
				Quaternion q = Quaternion.FromToRotation(Vector3.up, b.Orientation);

				for (int j = 0; j < BranchSubdivisions; j++)
				{
					float theta = j * 2f * Mathf.PI / BranchSubdivisions;
					Vector3 unrotated_pos = new Vector3(Mathf.Cos(theta) * b.Diameter, 0, Mathf.Sin(theta) * b.Diameter);
					Vector3 end_pos = q * unrotated_pos + b.PositionEnd;
					vertices[vert_idx + j] = end_pos - transform.position;

					Debug.Log("adding end pt for branch " + b.Id + " div " + j + ": " + (end_pos - transform.position));

					// if this branch is root, add vertices for base of trunk
					vertices[numBranches * BranchSubdivisions + j] = b.Position + unrotated_pos - transform.position;
				}
			}

			// create faces
			for (int i = 0; i < numBranches; i++)
			{
				Branch b = branches[i];
				int face_idx = (int)b.Id * BranchSubdivisions * 6;
				int top_idx = (int)b.Id * BranchSubdivisions;
				int bot_idx = b.Parent == null ? numBranches * BranchSubdivisions : top_idx;

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
