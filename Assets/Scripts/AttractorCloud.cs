using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp.Assets.Scripts
{
    public class AttractorCloud
    {
        private List<AttractorPoint> _points;

        public List<AttractorPoint> Points => _points;

        public AttractorCloud()
        {
            _points = new List<AttractorPoint>();
        }

        public AttractorCloud(List<AttractorPoint> points)
        {
            _points = points;
        }

        /// <summary>
        /// Add a collection of attractor points
        /// </summary>
        /// <param name="points"></param>
        public void AddPoints(List<AttractorPoint> points)
        {
            _points.AddRange(points);
        }

        /// <summary>
        /// Add a single attractor point
        /// </summary>
        /// <param name="point"></param>
        public void AddPoint(AttractorPoint point)
        {
            _points.Add(point);
        }

        /// <summary>
        /// Remove a single attractor point
        /// </summary>
        /// <param name="point"></param>
        public void RemovePoint(AttractorPoint point)
        {
            _points.Remove(point);
        }

        /// <summary>
        /// Draw gizmos for debugging
        /// </summary>
        public void DrawGizmos()
        {
            Gizmos.color = Color.red;
            foreach (AttractorPoint p in _points)
            {
                Gizmos.DrawSphere(p.Position, 0.05f);
            }
        }
    }
}
