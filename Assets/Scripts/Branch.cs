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
        public static float InternodeLength = 1.0f;
        public static float RollAngle = 0.523f;
        public static float BranchingAngle = 0.523f;

        public static float GrowthLength = 0.4f;
        public static float KillDistance = 0.7f;
        public static float PerceptionLength = 1.8f;
        public static float PerceptionRadius = 1.5f;
        public static float RandomGrowthParam = 0.1f;

        public static float TrunkDiameter = 1.0f;
        public static float DiameterCoeff = 0.8f;

        public Branch()
            : this(Vector3.zero, Vector3.up, BranchType.metamer, GrowthLength, 0, null) { }

        public Branch(Vector3 position, Vector3 orientation, BranchType type)
            : this(position, orientation, type, GrowthLength, 0, null) { }

        public Branch(Vector3 position, Vector3 orientation, BranchType type, float length)
            : this(position, orientation, type, length, 0, null) { }

        public Branch(Vector3 position, Vector3 orientation, BranchType type, float length, uint degree, Branch parent)
        {
            _position = position;
            _orientation = orientation;
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

        public void GetSubtree(List<Branch> allBranches)
        {
            if (!allBranches.Contains(this)) allBranches.Add(this);

            foreach (Branch b in _children)
            {
                b.GetSubtree(allBranches);
            }
        }

        /// <summary>
        /// Grow this branch (recursive & rule-based)
        /// </summary>
        public void Grow(AttractorCloud cloud)
        {
            ColonizeSpace(cloud);

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

                // add lateral bud
                Vector3 budPos = PositionEnd;
                Vector3 budOri = _orientation;
                Branch bud = new Branch(budPos, budOri, BranchType.lateral_bud);
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

                // add apical bud
                Vector3 budPos = PositionEnd;
                Vector3 budOri = _orientation;
                Branch bud = new Branch(budPos, budOri, BranchType.apical_bud);
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

            // iterate through all attractor points
            foreach (AttractorPoint point in cloud.Points)
            {
                float dist = Vector3.Distance(PositionEnd, point.Position);

                // only consider this point if within conical perception volume
                if (dist > PerceptionLength) continue;

                float cone_dist = Vector3.Dot(point.Position - PositionEnd, Orientation);
                if (cone_dist < 0 || cone_dist > PerceptionLength) continue;

                float cone_radius = cone_dist * PerceptionRadius / PerceptionLength;
                float orth_dist = Vector3.Magnitude(point.Position - PositionEnd - cone_dist * Orientation);
                if (orth_dist > cone_radius) continue;

                // if this is the closest branch to this point, set pointer
                if (dist < point.NearestDist) point.SetNearest(this, dist);
            }
        }

        /// <summary>
        /// Colonizes space!
        /// </summary>
        /// <param name="c"></param>
        public void ColonizeSpace(AttractorCloud cloud)
        {
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

                if (_parent != null)
                {
                    // TODO add constraints on branching angle relative to parent branch (if any)
                }

                centroid /= _attractors.Count;
                _length = Mathf.Min(Vector3.Distance(centroid, PositionEnd), GrowthLength);
            } else
            {
                _isDormant = true;
            }

            // make sure children buds remain attached
            foreach (Branch b in _children)
            {
                b.SetPosition(PositionEnd);
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
