using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp.Assets.Scripts
{
    public class AttractorPoint
    {
        private Vector3 _position;
        private float _nearestDist;
        private bool _isRemoved;

        public Vector3 Position => _position;
        public float NearestDist => _nearestDist;
        public bool IsRemoved => _isRemoved;

        public AttractorPoint() : this(Vector3.zero) { }

        public AttractorPoint(Vector3 position)
        {
            _position = position;
            _nearestDist = float.MaxValue;
            _isRemoved = false;
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
        /// Remove (kill) attractor point 
        /// </summary>
        public void Remove()
        {
            _isRemoved = true;
        }
    }
}
