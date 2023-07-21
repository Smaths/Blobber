using TMPro;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class UI_VersionNumber : MonoBehaviour
    {
        [SerializeField] private string _prependText;
        [SerializeField] private string _postfixText;
        private TMP_Text _label;

        private void OnValidate()
        {
            _label ??= GetComponent<TMP_Text>();
            _label.text = _prependText + Application.version +_postfixText;
        }

        // Start is called before the first frame update
        private void Start()
        {
            _label.text = _prependText + Application.version +_postfixText;
        }
    }
}
