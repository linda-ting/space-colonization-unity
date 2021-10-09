using System.Collections;
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

        public void SetAttractorCloud(AttractorCloud cloud)
        {
            _attractorCloud = cloud;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Grow()
        {
            Debug.Log("growing tree! age: " + _age);
            _root.Grow(_attractorCloud);
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