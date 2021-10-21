using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp.Assets.Scripts
{
    public class AttractorPoint
    {
        private Vector3 _position;
        private float _nearestDist;
        private Branch _nearestBranch;
        private bool _isRemoved;
        private uint _id;

        private static uint _lastId = 0;

        public Vector3 Position => _position;
        public float NearestDist => _nearestDist;
        public Branch NearestBranch => _nearestBranch;
        public bool IsRemoved => _isRemoved;
        public uint Id => _id;

        public AttractorPoint() : this(Vector3.zero) { }

        public AttractorPoint(Vector3 position)
        {
            _position = position;
            _nearestDist = float.MaxValue;
            _nearestBranch = null;
            _isRemoved = false;
            _id = _lastId + 1;
            _lastId++;
        }

        /// <summary>
        /// Set distance of branch closest to this point
        /// </summary>
        /// <param name="dist"></param>
        public void SetNearestDist(float dist)
        {
            _nearestDist = dist;
        }

        /// <summary>
        /// Set pointer for branch closest to this point
        /// </summary>
        /// <param name="branch"></param>
        public void SetNearestBranch(Branch branch)
        {
            _nearestBranch = branch;
        }

        public void SetNearest(Branch branch, float dist)
        {
            _nearestBranch = branch;
            _nearestDist = dist;
        }

        /// <summary>
        /// Clears nearest distance and nearest branch to this point
        /// </summary>
        public void ClearNearestBranch()
        {
            _nearestBranch = null;
            _nearestDist = float.MaxValue;
        }

        public void AddAttractorToBranch()
        {
            if (_nearestBranch != null) _nearestBranch.AddAttractor(this);
        }

        /// <summary>
        /// Remove (kill) attractor point 
        /// </summary>
        public void Remove()
        {
            _isRemoved = true;
        }

        public void DrawGizmos()
        {
            if (_isRemoved) return;

            if (_nearestBranch == null)
            {
                Gizmos.color = Color.red;
            }
            else
            {
                Gizmos.color = Color.yellow;
            }

            Gizmos.DrawSphere(_position, 0.1f);
        }
    }
}
