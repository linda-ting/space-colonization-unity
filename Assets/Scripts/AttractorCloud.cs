using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp.Assets.Scripts
{
    public class AttractorCloud
    {
        private List<AttractorPoint> _points;

        public List<AttractorPoint> Points => _points;

        public const int NumSampleAttractors = 1000;

        public AttractorCloud()
        {
            _points = new List<AttractorPoint>();
            GenerateAttractorsSphere(4.0f);
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
            Vector3 offset = radius * Vector3.up;
            for (int i = 0; i < NumSampleAttractors; i++)
            {
                float x = Random.value - 0.5f;
                float y = Random.value - 0.5f;
                float z = Random.value - 0.5f;
                Vector3 pos = new Vector3(x, y, z).normalized * radius + offset;
                AttractorPoint attractor = new AttractorPoint(pos);
                _points.Add(attractor);
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
