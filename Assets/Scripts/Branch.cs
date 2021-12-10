using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp.Assets.Scripts
{
    public enum BranchType
    {
        metamer,
        internode,
        apical_bud,
        lateral_bud
    };

    public class Branch
    {
        // private properties
        private Vector3 _position;
        private Vector3 _orientation;
        private Vector3 _right;
        private Vector3 _forward;
        private uint _degree;
        private Branch _parent;
        private List<Branch> _children;
        private BranchType _type;
        private float _length;
        private float _diameter;
        private List<AttractorPoint> _attractors;
        private bool _isDormant;
        private uint _id;

        private static uint _lastId = 0;

        // public accessors for private properties
        public Vector3 Position => _position;
        public Vector3 PositionEnd => _position + _orientation * _length;
        public Vector3 Orientation => _orientation;
        public Vector3 Right => _right;
        public Vector3 Forward => _forward;
        public uint Degree => _degree;
        public Branch Parent => _parent;
        public List<Branch> Children => _children;
        public BranchType Type => _type;
        public float Length => _length;
        public float Diameter => _diameter;
        public List<AttractorPoint> Attractors => _attractors;
        public bool IsDormant => _isDormant;
        public uint Id => _id;

        // constants
        public static float RollAngle = 30f;
        public static float BranchingAngle = 30f;
        public static float BranchingProbability = 0.6f;
        public static float BranchingDistribution = 0.25f;
        public static float BranchingRandomness = 0.3f;
        public static int MaxBranching = 4;

        public static float GrowthLength = 0.4f;
        public static float KillDistance = 1.0f;
        public static float PerceptionLength = 1.8f;
        public static float PerceptionRadius = 1.8f;
        public static float RandomGrowthParam = 0.1f;

        public static float TrunkDiameter = 1.0f;
        public static float DiameterCoeff = 0.82f;

        public Branch()
            : this(Vector3.zero, Vector3.up, Vector3.right, BranchType.metamer, GrowthLength, 0, null) { }

        public Branch(Vector3 position, Vector3 orientation, Vector3 right, BranchType type)
            : this(position, orientation, right, type, GrowthLength, 0, null) { }

        public Branch(Vector3 position, Vector3 orientation, Vector3 right, BranchType type, float length)
            : this(position, orientation, right, type, length, 0, null) { }

        public Branch(Vector3 position, Vector3 orientation, Vector3 right, BranchType type, float length, uint degree, Branch parent)
        {
            _position = position;
            _orientation = orientation;
            _right = right;
            _forward = Vector3.Cross(_orientation, _right).normalized;
            _degree = degree;
            _parent = parent;
            _children = new List<Branch>();
            _type = type;
            _length = length;
            _attractors = new List<AttractorPoint>();
            _isDormant = false;
            _id = _lastId;
            _lastId++;

            ComputeDiameter();
        }

        /// <summary>
        /// Add a new child to this branch
        /// </summary>
        /// <param name="branch"></param>
        public void AddChild(Branch child)
        {
            child.SetParent(this);
            child.SetDegree(_degree + 1);
            _children.Add(child);
        }

        /// <summary>
        /// Set the parent of this branch
        /// </summary>
        /// <param name="branch"></param>
        public void SetParent(Branch parent)
        {
            _parent = parent;
            ComputeDiameter();
        }

        /// <summary>
        /// Set the degree of this branch
        /// </summary>
        /// <param name="degree"></param>
        public void SetDegree(uint degree)
        {
            _degree = degree;
        }

        /// <summary>
        /// Set starting position of this branch
        /// </summary>
        /// <param name="position"></param>
        public void SetPosition(Vector3 position)
        {
            _position = position;
        }

        /// <summary>
        /// Reset last id number of class
        /// </summary>
        public static void ResetIdNum()
        {
            _lastId = 0;
        }

        /// <summary>
        /// Add an attractor point to current list of attractors
        /// </summary>
        /// <param name="point"></param>
        public void AddAttractor(AttractorPoint point)
        {
            _attractors.Add(point);
        }

        /// <summary>
        /// Clear all active attractor points
        /// </summary>
        public void ClearAttractors()
        {
            foreach (Branch c in _children)
            {
                c.ClearAttractors();
            }

            // clear active attractors
            _attractors.Clear();
        }

        /// <summary
        /// Calculates random orientation
        /// </summary>
        /// <returns></returns>
        private static Vector3 GetRandomOrientation()
        {
            return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        }

        /// <summary>
        /// Set random growth orientation for this branch
        /// </summary>
        private void SetRandomOrientation()
        {
            _orientation = GetRandomOrientation();
            _length = GrowthLength;
        }

        /// <summary>
        /// Add subtree rooted at this branch to list
        /// </summary>
        /// <param name="allBranches"></param>
        public void GetSubtree(List<Branch> allBranches)
        {
            if (!allBranches.Contains(this)) allBranches.Add(this);

            foreach (Branch b in _children)
            {
                b.GetSubtree(allBranches);
            }
        }

        private bool WillBranch()
        {
            float func = 1f - 0.9f * Mathf.Pow(2, -BranchingDistribution * _degree);
            float rand = Random.Range(0f, 1f);
            float prob = (1 - BranchingRandomness) * func + BranchingRandomness * rand;
            return prob > (1f - BranchingProbability);
        }

        /// <summary>
        /// Grow this branch (recursive & rule-based)
        /// </summary>
        public void Grow(AttractorCloud cloud)
        {
            // grow children
            foreach (Branch b in _children)
            {
                b.Grow(cloud);
            }

            // do not grow this branch if dormant
            if (_isDormant) return;

            if (_type == BranchType.metamer)
            {
                // grow metamer into internode
                _type = BranchType.internode;

                if (_children.Count > 0 && !WillBranch() || _children.Count >= MaxBranching) return;

                // add lateral bud
                Vector3 budPos = PositionEnd;
                Vector3 budOri = _orientation;
                Vector3 budRight = _right;
                Branch bud = new Branch(budPos, budOri, budRight, BranchType.lateral_bud);
                AddChild(bud);
            }
            else if (_type == BranchType.lateral_bud)
            {
                // grow lateral bud into apical bud
                _type = BranchType.apical_bud;
            }
            else if (_type == BranchType.apical_bud)
            {
                // grow apical bud into metamer
                _type = BranchType.metamer;

                if (_children.Count > 0 && !WillBranch() || _children.Count >= MaxBranching) return;

                // add apical bud
                Vector3 budPos = PositionEnd;
                Vector3 budOri = _orientation;
                Vector3 budRight = _right;
                Branch bud = new Branch(budPos, budOri, budRight, BranchType.apical_bud);
                AddChild(bud);
            }
        }

        /// <summary>
        /// Find the closest branch for each attractor point
        /// </summary>
        /// <param name="cloud"></param>
        public void FindAttractors(AttractorCloud cloud)
        {
            // find attractors for children
            foreach (Branch b in _children)
            {
                b.FindAttractors(cloud);
            }

            // transform perception space according to branching angle
            float angle = Random.value > 0.5 ? BranchingAngle : -BranchingAngle;
            Quaternion rot = Quaternion.AngleAxis(angle, _forward);
            Vector3 ori = rot * _orientation;

            int cnt = FindAttractorsHelper(cloud, angle, rot, ori, PerceptionLength);
            if (cnt > 0) return;

            // adjust search length if no attractors found
            float xDist = Mathf.Min(Mathf.Abs(PositionEnd[0] - cloud.BoundingBox.min[0]),
                                    Mathf.Abs(PositionEnd[0] - cloud.BoundingBox.max[0]));
            float zDist = Mathf.Min(Mathf.Abs(PositionEnd[2] - cloud.BoundingBox.min[2]),
                                    Mathf.Abs(PositionEnd[2] - cloud.BoundingBox.max[2]));
            float newPerceptionLength = Mathf.Min(xDist, zDist);
            if (newPerceptionLength <= PerceptionLength) return;
            //Debug.Log("new perception length " + newPerceptionLength);
            FindAttractorsHelper(cloud, angle, rot, ori, newPerceptionLength);
        }

        private int FindAttractorsHelper(AttractorCloud cloud, float angle, Quaternion rot, Vector3 ori, float searchLength)
        {
            int cnt = 0;

            foreach (AttractorPoint point in cloud.Points)
            {
                float dist = Vector3.Distance(PositionEnd, point.Position);

                // only consider this point if within conical perception volume
                if (dist > PerceptionLength) continue;

                float cone_dist = Vector3.Dot(point.Position - PositionEnd, ori);
                if (cone_dist < 0 || cone_dist > PerceptionLength) continue;

                float cone_radius = cone_dist * PerceptionRadius / PerceptionLength;
                float orth_dist = Vector3.Magnitude(point.Position - PositionEnd - cone_dist * ori);
                if (orth_dist > cone_radius) continue;

                // if this is the closest branch to this point, set pointer
                if (dist < point.NearestDist)
                {
                    point.SetNearest(this, dist);
                    cnt++;
                }
            }

            return cnt;
        }

        /// <summary>
        /// Colonizes space!
        /// </summary>
        /// <param name="c"></param>
        public void ColonizeSpace(AttractorCloud cloud)
        {
            foreach (Branch b in _children) {
                b.ColonizeSpace(cloud);
            }

            // do not change orientation if not a bud
            if (_type == BranchType.metamer || _type == BranchType.internode) return;

            // remove attractor points within kill distance
            foreach (AttractorPoint point in cloud.Points)
            {
                float dist = Vector3.Distance(PositionEnd, point.Position);
                if (dist < KillDistance) cloud.RemovePoint(point);
            }

            // grow branch towards attractors
            Vector3 orientation = Vector3.zero;
            Vector3 centroid = Vector3.zero;
            if (_attractors.Count > 0)
            {
                // average growth direction
                foreach (AttractorPoint point in _attractors)
                {
                    orientation += Vector3.Normalize(point.Position - PositionEnd);
                    centroid += point.Position;
                }
                orientation /= _attractors.Count;
                orientation += GetRandomOrientation() * RandomGrowthParam;
                _orientation = orientation.normalized;

                // ensure that branches do not go down
                _orientation[1] = Mathf.Max(-0.1f, _orientation[1]);
                _orientation = orientation.normalized;

                centroid /= _attractors.Count;
                _length = Mathf.Min(Vector3.Distance(centroid, PositionEnd), GrowthLength);
            } else
            {
                _isDormant = true;
            }
        }

        public void ComputeDiameter()
        {
            _diameter = _parent == null ? TrunkDiameter : DiameterCoeff * _parent.Diameter;
        }

        /// <summary>
        /// Draw gizmos for debugging
        /// </summary>
        public void DrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(_position, PositionEnd);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_position, 0.05f);
            Gizmos.DrawSphere(PositionEnd, 0.05f);

            foreach (Branch b in _children)
            {
                b.DrawGizmos();
            }
        }
    }
}
