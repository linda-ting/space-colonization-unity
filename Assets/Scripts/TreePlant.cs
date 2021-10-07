using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp.Assets.Scripts
{
    public class TreePlant
    {
        private Branch _root;
        private uint _age;

        public Branch Root => _root;
        public uint Age => _age;

        public TreePlant() : this(new Branch()) { }

        public TreePlant(Branch root)
        {
            _root = root;
            _age = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="age"></param>
        public void SetAge(uint age)
        {
            _age = age;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Grow()
        {
            _root.Grow();
            _age++;
        }

        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cloud"></param>
        public void ColonizeSpace(AttractorCloud cloud)
        {
            // TODO
        }*/

        /// <summary>
        /// Print tree out to a txt file
        /// </summary>
        /// <param name="filename"></param>
        public void Print(string filename)
        {
            // TODO
        }
    }
}