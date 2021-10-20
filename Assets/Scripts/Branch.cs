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
        private bool _isDormant;

        // public accessors for private properties
        public Vector3 Position => _position;
        public Vector3 PositionEnd => _position + _orientation * _length;
        public Vector3 Orientation => _orientation;
        public uint Degree => _degree;
        public Branch Parent => _parent;
        public List<Branch> Children => _children;
        public BranchType Type => _type;
        public float Length => _length;
        public bool IsDormant => _isDormant;

        // constants
        public const float InternodeLength = 1.0f;
        public const float RollAngle = 0.523f;
        public const float BranchingAngle = 0.523f;
        public const float GrowthLength = 0.75f;
        public const float DiameterCoeff = 0.6f;
        public const float KillDistance = 0.75f;
        public const float PerceptionLength = 0.75f;
        public const float PerceptionRadius = 0.15f;

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
            _isDormant = false;
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
        }

        /// <summary>
        /// Set the degree of this branch
        /// </summary>
        /// <param name="degree"></param>
        public void SetDegree(uint degree)
        {
            _degree = degree;
        }

        /// <summary
        /// Calculates random orientation in positive Y direction
        /// </summary>
        /// <returns></returns>
        private static Vector3 GetRandomOrientation()
        {
            return new Vector3(Random.value - 0.5f, Random.value, Random.value - 0.5f).normalized;
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

                // add lateral bud
                Vector3 budPos = PositionEnd;
                Vector3 budOri = _orientation;
                Branch bud = new Branch(budPos, budOri, BranchType.lateral_bud);
                AddChild(bud);
                bud.ColonizeSpace(cloud);
            }
            else if (_type == BranchType.lateral_bud)
            {
                // grow lateral bud into apical bud
                _type = BranchType.apical_bud;
                ColonizeSpace(cloud);
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
                bud.ColonizeSpace(cloud);
            }
        }

        /// <summary>
        /// Colonizes space!
        /// </summary>
        /// <param name="c"></param>
        public void ColonizeSpace(AttractorCloud cloud)
        {
            List<AttractorPoint> currAttractors = new List<AttractorPoint>();
            float minDist = float.MaxValue;

            // remove attractor points within kill distance & find attractors for this branch
            foreach (AttractorPoint point in cloud.Points.ToArray())
            {
                float dist = Vector3.Distance(PositionEnd, point.Position);
                float cone_dist = Vector3.Dot(point.Position - PositionEnd, Orientation);
                float cone_radius = cone_dist * PerceptionLength / PerceptionRadius;
                float orth_dist = Vector3.Magnitude(point.Position - PositionEnd - cone_dist * Orientation);

                if (dist < KillDistance)
                {
                    cloud.RemovePoint(point);
                }
                else
                {
                    if (cone_dist < 0 || cone_dist > PerceptionLength) continue;

                    if (orth_dist <= cone_radius && orth_dist < minDist)
                    {
                        minDist = orth_dist;
                        currAttractors.Add(point);
                    }
                }
            }

            Debug.Log("num attractors: " + currAttractors.Count);

            // grow branch towards attractors
            Vector3 orientation = Vector3.zero;
            Vector3 centroid = Vector3.zero;
            if (currAttractors.Count > 0)
            {
                // average growth direction
                foreach (AttractorPoint point in currAttractors)
                {
                    orientation += Vector3.Normalize(point.Position - PositionEnd);
                    centroid += point.Position;
                }
                orientation /= currAttractors.Count;
                _orientation = orientation;

                centroid /= currAttractors.Count;
                _length = Vector3.Distance(centroid, PositionEnd);

                Debug.Log("setting new orientation: " + _orientation);
                Debug.Log("setting new length: " + _length);
            } else
            {
                // make dormant
                _isDormant = true;
            }
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
