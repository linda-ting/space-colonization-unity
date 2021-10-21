using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp.Assets.Scripts
{
    public class AttractorCloud
    {
        private List<AttractorPoint> _points;

        public List<AttractorPoint> Points => _points;

        public const int NumSampleAttractors = 300;

        public AttractorCloud()
        {
            _points = new List<AttractorPoint>();
            GenerateAttractorsSphere(3.0f);
        }

        public AttractorCloud(List<AttractorPoint> points)
        {
            _points = points;
        }

        /// <summary>
        /// Generates randomly sampled attractor points within a specific sphere radius
        /// </summary>
        /// <param name="radius"></param>
        private void GenerateAttractorsSphere(float radius)
        {
            Vector3 offset = 1.3f * radius * Vector3.up;
            for (int i = 0; i < NumSampleAttractors; i++)
            {
                Vector3 dir = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
                float r = Random.Range(0, radius);
                Vector3 pos = r * dir + offset;
                AttractorPoint attractor = new AttractorPoint(pos);
                _points.Add(attractor);
            }
        }

        /// <summary>
        /// Clears saved nearest distance and nearest branch for all attractor points
        /// </summary>
        public void ClearNearestBranches()
        {
            foreach (AttractorPoint p in _points)
            {
                p.ClearNearestBranch();
            }
        }

        /// <summary>
        /// Adds attractor points to their respective nearest branches
        /// </summary>
        public void AddAttractorsToBranches()
        {
            foreach (AttractorPoint p in _points)
            {
                if (p.IsRemoved) continue;
                p.AddAttractorToBranch();
            }
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
            point.Remove();
        }

        public void ClearRemovedPoints()
        {
            for (int i = _points.Count - 1; i >= 0; i--)
            {
                AttractorPoint p = _points[i];
                if (p.IsRemoved) _points.Remove(p);
            }
        }

        /// <summary>
        /// Draw gizmos for debugging
        /// </summary>
        public void DrawGizmos()
        {
            foreach (AttractorPoint p in _points)
            {
                p.DrawGizmos();
            }
        }
    }
}
