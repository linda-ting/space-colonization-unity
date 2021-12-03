using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AssemblyCSharp.Assets.Scripts
{
    public class UserInterfaceController : MonoBehaviour
    {
        [SerializeField]
        private TreeGenerator _treeGenerator;

        [SerializeField]
        private Slider _growthLengthInput;

        [SerializeField]
        private Slider _killDistanceInput;

        [SerializeField]
        private Slider _perceptionDistanceInput;

        [SerializeField]
        private Slider _perceptionRadiusInput;

        [SerializeField]
        private Slider _randomGrowthInput;

        [SerializeField]
        private Slider _trunkDiameterInput;

        [SerializeField]
        private Slider _diameterCoeffInput;

        [SerializeField]
        private Slider _treeAgeInput;

        [SerializeField]
        private Slider _branchingAngleInput;

        [SerializeField]
        private Slider _numBranchesInput;

        private bool _isPaused = true;


        public void UpdateParamsCallback()
        {
            Debug.Log("updating params!");

            Branch.GrowthLength = _growthLengthInput.value;
            Branch.KillDistance = _killDistanceInput.value;
            Branch.PerceptionLength = _perceptionDistanceInput.value;
            Branch.PerceptionRadius = _perceptionRadiusInput.value;
            Branch.RandomGrowthParam = _randomGrowthInput.value;
            Branch.TrunkDiameter = _trunkDiameterInput.value;
            Branch.DiameterCoeff = _diameterCoeffInput.value;
            Branch.BranchingAngle = _branchingAngleInput.value;
            Branch.MaxBranching = (int)_numBranchesInput.value;

            _treeGenerator.SetTreeAge((int)_treeAgeInput.value);
        }

        public void ResetParamsCallback()
        {
            Debug.Log("resetting params!");
            Branch.GrowthLength = 0.4f;
            Branch.KillDistance = 0.7f;
            Branch.PerceptionLength = 1.8f;
            Branch.PerceptionRadius = 1.5f;
            Branch.RandomGrowthParam = 0.1f;
            Branch.TrunkDiameter = 1.0f;
            Branch.DiameterCoeff = 0.8f;
            Branch.BranchingAngle = 30;
            Branch.MaxBranching = 4;
        }

        public void ToggleGrowth()
        {
            _isPaused = !_isPaused;

            if (_isPaused)
            {
                _treeGenerator.StopGrowing();
            }
            else
            {
                _treeGenerator.StartGrowing();
            }
        }

        public void StartCallback()
        {
            // TODO
        }

        public void PauseCallback()
        {
            // TODO
        }
    }
}