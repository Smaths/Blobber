using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Environment
{
    public class RandomMeshRack : MonoBehaviour
    {
        [Title("Randomization", "Create variation in what is shown on game start.", TitleAlignments.Split)]
        [LabelText("Randomize Object")]
        [Tooltip("`true`: select a random game object from the list of models to show on game start.\n`false`: Use selected game object.")]
        [SerializeField] private bool _randomObjectFlag = true;
        [LabelText("Randomize Scale")]
        [SerializeField] private bool _randomScaleFlag = false;
        [ShowIfGroup("_randomScaleFlag")]
        [Indent]
        [MinValue(0)]
        [Tooltip("0 for no change. Affects the random range for the object's scale. Watch out for very large numbers.")]
        [SerializeField] private float _randomScaleAmount;

        [LabelText("Randomize Rotation")]
        [SerializeField] private bool _randomRotationFlag = true;

        [Title("Models")]
        [SerializeField] private GameObject _selectedObject;
        [SerializeField] private List<GameObject> _models;

        private void OnValidate()
        {
            FindChildGameObjects();
            SetSelectedObject();
        }

        // Start is called before the first frame update
        private void Start()
        {
            RandomizeAll();
        }

        [Button("Find Models")]
        private void FindChildGameObjects()
        {
            _models = new List<GameObject>();
            for (int i = 0; i < transform.childCount; i++)
            {
                _models.Add(transform.GetChild(i).gameObject);
            }

            SetSelectedObject();
        }

        private void SetSelectedObject()
        {
            if (_models.IsNullOrEmpty()) return;

            // Search for first model that is active in hierarchy.
            foreach (GameObject model in _models.Where(m => m.activeInHierarchy))
            {
                _selectedObject = model;
            }
        }

        #region Randomization
        public void RandomizeAll()
        {
            if (!_models.IsNullOrEmpty())
            {
                // Randomize model
                if (_randomObjectFlag) RandomizeModel();
                // Randomize scale
                if (_randomScaleFlag) RandomizeScale();
                // Randomize rotation
                if (_randomRotationFlag) RandomizeRotation();
            }
        }

        public void RandomizeModel()
        {
            // Disable all game objects
            foreach (GameObject model in _models)
                model.SetActive(false);

            // Enable random model
            int randomIndex = Random.Range(0, _models.Count - 1);
            _selectedObject = _models[randomIndex];
            _selectedObject.SetActive(true);
        }

        public void RandomizeScale()
        {
            Vector3 localScale = transform.localScale;
            float randomScaleAmount = Random.Range(localScale.x - _randomScaleAmount, localScale.x + _randomScaleAmount);
            transform.localScale = new Vector3(randomScaleAmount, randomScaleAmount, randomScaleAmount);
        }

        public void RandomizeRotation()
        {
            // Rotate randomly in 90° increments
            float[] rotationOptions = {0f, 90f, 180f, 270f};
            int randomRotation = Random.Range(0, rotationOptions.Length - 1);
            transform.rotation = Quaternion.Euler(0, rotationOptions[randomRotation], 0);
        }
        #endregion
    }
}
