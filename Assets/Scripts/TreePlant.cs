﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp.Assets.Scripts
{
    public class TreePlant
    {
        private Branch _root;
        private uint _age;
        private AttractorCloud _attractorCloud;

        public Branch Root => _root;
        public uint Age => _age;
        public AttractorCloud Attractors => _attractorCloud;

        public TreePlant() : this(new Branch(), new AttractorCloud()) { }

        public TreePlant(Branch root) : this(root, new AttractorCloud()) { }

        public TreePlant(Branch root, AttractorCloud cloud)
        {
            _root = root;
            _age = 0;
            _attractorCloud = cloud;
        }

        /// <summary>
        /// Set attractor point cloud for tree to fit
        /// </summary>
        /// <param name="cloud"></param>
        public void SetAttractorCloud(AttractorCloud cloud)
        {
            _attractorCloud = cloud;
        }

        /// <summary>
        /// Grow tree
        /// </summary>
        public void Grow()
        {
            if (_attractorCloud.Points.Count == 0 || _age > 25) return;

            Debug.Log("growing tree! age: " + _age);

            // clear active attractors for branches
            _root.ClearAttractors();

            // clear nearest branches for attractor points
            _attractorCloud.ClearNearestBranches();

            // find new nearest branches for attractor points
            _root.FindAttractors(_attractorCloud);

            // add active attractors for branches
            _attractorCloud.AddAttractorsToBranches();

            // grow the tree
            _root.Grow(_attractorCloud);

            // kill flagged attractor points
            _attractorCloud.ClearRemovedPoints();

            // increment age
            _age++;
        }

        /// <summary>
        /// Draw gizmos for debugging
        /// </summary>
        public void DrawGizmos()
        {
            _root.DrawGizmos();
        }
    }
}