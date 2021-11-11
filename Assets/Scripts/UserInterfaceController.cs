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
        private InputField _growthLengthInput;

        [SerializeField]
        private InputField _killDistanceInput;

        [SerializeField]
        private InputField _perceptionDistanceInput;

        [SerializeField]
        private InputField _perceptionRadiusInput;

        [SerializeField]
        private InputField _randomGrowthInput;

        [SerializeField]
        private InputField _trunkDiameterInput;

        [SerializeField]
        private InputField _diameterCoeffInput;

        [SerializeField]
        private InputField _treeAgeInput;

        [SerializeField]
        private InputField _branchingAngleInput;

        [SerializeField]
        private InputField _numBranchesInput;


        public void UpdateParamsCallback()
        {
            Debug.Log("updating params!");

            if (!string.IsNullOrEmpty(_growthLengthInput.text))
            {
                Branch.GrowthLength = float.Parse(_growthLengthInput.text.Trim());
            }

            if (!string.IsNullOrEmpty(_killDistanceInput.text))
            {
                Branch.KillDistance = float.Parse(_killDistanceInput.text.Trim());
            }

            if (!string.IsNullOrEmpty(_perceptionDistanceInput.text))
            {
                Branch.PerceptionLength = float.Parse(_perceptionDistanceInput.text.Trim());
            }

            if (!string.IsNullOrEmpty(_perceptionRadiusInput.text))
            {
                Branch.PerceptionRadius = float.Parse(_perceptionRadiusInput.text.Trim());
            }

            if (!string.IsNullOrEmpty(_randomGrowthInput.text))
            {
                Branch.RandomGrowthParam = float.Parse(_randomGrowthInput.text.Trim());
            }

            if (!string.IsNullOrEmpty(_trunkDiameterInput.text))
            {
                Branch.TrunkDiameter = float.Parse(_trunkDiameterInput.text.Trim());
            }

            if (!string.IsNullOrEmpty(_diameterCoeffInput.text))
            {
                Branch.DiameterCoeff = float.Parse(_diameterCoeffInput.text.Trim());
            }

            if (!string.IsNullOrEmpty(_treeAgeInput.text))
            {
                int age = int.Parse(_treeAgeInput.text.Trim());
                _treeGenerator.SetTreeAge(age);
            }

            if (!string.IsNullOrEmpty(_branchingAngleInput.text))
            {
                int angle = int.Parse(_branchingAngleInput.text.Trim());
                Branch.BranchingAngle = angle;
            }

            if (!string.IsNullOrEmpty(_numBranchesInput.text))
            {
                int num = int.Parse(_numBranchesInput.text.Trim());
                Branch.MaxBranching = num;
            }
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