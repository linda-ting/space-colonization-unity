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

        public TreePlant() : this(new Branch()) { }

        public TreePlant(Branch root) : this(root, new AttractorCloud()) { }

        public TreePlant(AttractorCloud cloud) : this(new Branch(), cloud) { }

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
            //ColonizeSpace();
            _root.Grow();
            _age++;
        }

        /// <summary>
        /// Colonizes space!
        /// </summary>
        /// <param name="cloud"></param>
        public void ColonizeSpace()
        {
            _root.ColonizeSpace(_attractorCloud);
        }

        /// <summary>
        /// Print tree out to a txt file
        /// </summary>
        /// <param name="filename"></param>
        public void Print(string filename)
        {
            // TODO
        }

        public void DrawGizmos()
        {
            _root.DrawGizmos();
        }
    }
}