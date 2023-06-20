using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

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
        if (!_models.IsNullOrEmpty())
        {
            // Randomize model
            if (_randomObjectFlag)
            {
                // Disable all game objects
                foreach (var model in _models)
                {
                    model.gameObject.SetActive(false);
                }

                // Enable random model
                int randomIndex = Random.Range(0, _models.Count - 1);
                _selectedObject = _models[randomIndex];
            }

            if (_selectedObject)
            {
                _selectedObject.SetActive(true);   
            }
            else
            {
                print($"{gameObject.name} - No selected object, nothing will show!");
            }

            // Randomize Scale
            if (_randomScaleFlag)
            {
                Vector3 localScale = transform.localScale;
                float randomScaleAmount = Random.Range(localScale.x - _randomScaleAmount, localScale.x + _randomScaleAmount);
                transform.localScale = new Vector3(randomScaleAmount, randomScaleAmount, randomScaleAmount);
            }


            // Randomize rotation
            if (_randomRotationFlag)
            {
                // Rotate randomly in 90° increments
                float[] rotationOptions = { 0f, 90f, 180f, 270f };
                int randomRotation = Random.Range(0, rotationOptions.Length - 1);
                transform.rotation = Quaternion.Euler(0, rotationOptions[randomRotation], 0);
            }
        }
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

        foreach (GameObject model in _models.Where(model => model.activeInHierarchy))
        {
            _selectedObject = model;
        }
    }
}
