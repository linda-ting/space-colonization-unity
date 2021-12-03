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
        private Text _growthLengthText;

        [SerializeField]
        private Slider _killDistanceInput;
        [SerializeField]
        private Text _killDistanceText;

        [SerializeField]
        private Slider _perceptionDistanceInput;
        [SerializeField]
        private Text _perceptionDistanceText;

        [SerializeField]
        private Slider _perceptionRadiusInput;
        [SerializeField]
        private Text _perceptionRadiusText;

        [SerializeField]
        private Slider _randomGrowthInput;
        [SerializeField]
        private Text _randomGrowthText;

        [SerializeField]
        private Slider _trunkDiameterInput;
        [SerializeField]
        private Text _trunkDiameterText;

        [SerializeField]
        private Slider _diameterCoeffInput;
        [SerializeField]
        private Text _diameterCoeffText;

        [SerializeField]
        private Slider _treeAgeInput;
        [SerializeField]
        private Text _treeAgeText;

        [SerializeField]
        private Slider _branchingAngleInput;
        [SerializeField]
        private Text _branchingAngleText;

        [SerializeField]
        private Slider _numBranchesInput;
        [SerializeField]
        private Text _numBranchesText;

        [SerializeField]
        private Slider _branchingProbabilityInput;
        [SerializeField]
        private Text _branchingProbabilityText;

        [SerializeField]
        private Slider _branchingDistributionInput;
        [SerializeField]
        private Text _branchingDistributionText;

        [SerializeField]
        private Slider _branchingRandomnessInput;
        [SerializeField]
        private Text _branchingRandomnessText;

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
            Branch.BranchingProbability = _branchingProbabilityInput.value;
            Branch.BranchingDistribution = _branchingDistributionInput.value;
            Branch.BranchingRandomness = _branchingRandomnessInput.value;
            _treeGenerator.SetTreeAge((int)_treeAgeInput.value);

            ResetSliderLabels();
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
            Branch.BranchingProbability = 0.6f;
            Branch.BranchingDistribution = 0.25f;
            Branch.BranchingRandomness = 0.3f;

            _growthLengthInput.value = Branch.GrowthLength;
            _killDistanceInput.value = Branch.KillDistance;
            _perceptionDistanceInput.value = Branch.PerceptionLength;
            _perceptionRadiusInput.value = Branch.PerceptionRadius;
            _randomGrowthInput.value = Branch.RandomGrowthParam;
            _trunkDiameterInput.value = Branch.TrunkDiameter;
            _diameterCoeffInput.value = Branch.DiameterCoeff;
            _branchingAngleInput.value = Branch.BranchingAngle;
            _numBranchesInput.value = Branch.MaxBranching;
            _branchingProbabilityInput.value = Branch.BranchingProbability;
            _branchingDistributionInput.value = Branch.BranchingDistribution;
            _branchingRandomnessInput.value = Branch.BranchingRandomness;

            ResetSliderLabels();
        }

        private void ResetSliderLabels()
        {
            _growthLengthText.text = System.Math.Round(_growthLengthInput.value, 2).ToString();
            _killDistanceText.text = System.Math.Round(_killDistanceInput.value, 2).ToString();
            _perceptionDistanceText.text = System.Math.Round(_perceptionDistanceInput.value, 2).ToString();
            _perceptionRadiusText.text = System.Math.Round(_perceptionRadiusInput.value, 2).ToString();
            _randomGrowthText.text = System.Math.Round(_randomGrowthInput.value, 2).ToString();
            _trunkDiameterText.text = System.Math.Round(_trunkDiameterInput.value, 2).ToString();
            _diameterCoeffText.text = System.Math.Round(_diameterCoeffInput.value, 2).ToString();
            _branchingAngleText.text = _branchingAngleInput.value.ToString();
            _numBranchesText.text = _numBranchesInput.value.ToString();
            _branchingProbabilityText.text = System.Math.Round(_branchingProbabilityInput.value, 2).ToString();
            _branchingDistributionText.text = System.Math.Round(_branchingDistributionInput.value, 2).ToString();
            _branchingRandomnessText.text = System.Math.Round(_branchingRandomnessInput.value, 2).ToString();
            _treeAgeText.text = _treeAgeInput.value.ToString();
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