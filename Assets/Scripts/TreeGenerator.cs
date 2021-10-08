using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp.Assets.Scripts
{
    public class TreeGenerator : MonoBehaviour
    {
        public GameObject _treeObject;
        public TreePlant _treePlant;
        public AttractorCloud _attractors;
        public float _timeLapsed;
        public const float _updateTime = 1.0f;

        // Start is called before the first frame update
        void Start()
        {
            _timeLapsed = 0.0f;

            // TODO create test attractor points
            _attractors = new AttractorCloud();
            _treePlant = new TreePlant(_attractors);
        }

        // Update is called once per frame
        void Update()
        {
            _timeLapsed += Time.deltaTime;

            if (_timeLapsed > _updateTime)
            {
                Debug.Log("growing tree!");
                _timeLapsed = 0.0f;
                _treePlant.Grow();
            }
        }

        /// <summary>
        /// Draw gizmos for debugging
        /// </summary>
        void OnDrawGizmos()
        {
            _attractors.DrawGizmos();
            _treePlant.DrawGizmos();
        }
    }
}
