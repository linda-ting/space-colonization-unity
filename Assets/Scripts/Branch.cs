using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp.Assets.Scripts;

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

        // public accessors for private properties
        public Vector3 Position => _position;
        public Vector3 Orientation => _orientation;
        public uint Degree => _degree;
        public Branch Parent => _parent;
        public List<Branch> Children => _children;
        public BranchType Type => _type;
        public float Length => _length;

        public Branch()
            : this(Vector3.zero, Vector3.up, BranchType.metamer, 0, null) { }

        public Branch(Vector3 position, Vector3 orientation, BranchType type)
            : this(position, orientation, type, 0, null) { }

        public Branch(Vector3 position, Vector3 orientation, BranchType type, uint degree, Branch parent)
        {
            _position = position;
            _orientation = orientation;
            _degree = degree;
            _parent = parent;
            _type = type;
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

        /// <summary>
        /// Find new growth orientation for this branch
        /// </summary>
        /// <returns></returns>
        public Vector3 FindNewOrientation()
        {
            // TODO
            return Vector3.up;
        }

        /// <summary>
        /// Grow this branch (recursive & rule-based)
        /// </summary>
        public void Grow()
        {
            // grow children
            for (int i = 0; i < Children.Count; i++)
            {
                _children[i].Grow();
            }

            if (_type == BranchType.metamer)
            {
                // grow metamer into internode
                _type = BranchType.internode;

                // add lateral bud
                // TODO set bud position and orientation
                Vector3 budPos = _position;
                Vector3 budOri = _orientation;
                Branch bud = new Branch(budPos, budOri, BranchType.lateral_bud);
                AddChild(bud);

            }
            else if (_type == BranchType.lateral_bud)
            {
                // grow lateral bud into apical bud
                // TODO set bud position and orientation
                Vector3 budPos = _position;
                Vector3 budOri = _orientation;
                _position = budPos;
                _orientation = budOri;
                _type = BranchType.apical_bud;

            }
            else if (_type == BranchType.apical_bud)
            {
                // grow apical bud into metamer
                _type = BranchType.metamer;

                // add apical bud
                // TODO set bud position and orientation
                Vector3 budPos = _position;
                Vector3 budOri = _orientation;
                Branch bud = new Branch(budPos, budOri, BranchType.apical_bud);
                AddChild(bud);
            }
        }

        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        public void ColonizeSpace(AttractorCloud c)
        {
            // TODO
        }*/

        /// <summary>
        /// Print branch out to a txt file
        /// </summary>
        /// <param name="file"></param>
        public void Print(string file)
        {
            // TODO
        }
    }
}
