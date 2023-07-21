using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utility;

namespace UI
{
    public class UI_ChangePlayerName : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _playerNameInput;
        [SerializeField] private int _maxCharacterCount = 12;
        [SerializeField] private Button _submitButton;

        public UnityEvent<string> OnSubmit;

        #region Lifecycle
        private void OnEnable()
        {
            _playerNameInput.onValidateInput += OnValidateInput;
            _playerNameInput.onValueChanged.AddListener(OnValueChanged);
            _submitButton.onClick.AddListener(OnSubmit_Tapped);

            _playerNameInput.text = string.Empty;
            _submitButton.interactable = false;

            EventSystem.current.SetSelectedGameObject(_submitButton.gameObject);
        }

        private void OnDisable()
        {
            _playerNameInput.onValidateInput -= OnValidateInput;
            _playerNameInput.onValueChanged.RemoveListener(OnValueChanged);
            _submitButton.onClick.RemoveListener(OnSubmit_Tapped);
        }
        #endregion

        // Text input validation
        private char OnValidateInput(string text, int charIndex, char addedChar)
        {
            // Prevent names longer than max setting
            if (charIndex + 1 >= _maxCharacterCount) return '\0';

            // Only accept: Letters, Digits, Punctuation.
            if (char.IsLetterOrDigit(addedChar) || char.IsPunctuation(addedChar) || char.IsSeparator(addedChar))
            {
                return addedChar;
            }

            return '\0';
        }

        private void OnValueChanged(string text)
        {
            // Limit length
            _submitButton.interactable = text.Length > 0;
        }

        // Button event handler
        private void OnSubmit_Tapped()
        {
            string playerName = _playerNameInput.text;
            if (playerName.IsNullOrWhitespace()) return;

            LootLockerTool.Instance.UpdatePlayerName(playerName);

            OnSubmit?.Invoke(playerName);

            gameObject.SetActive(false);
        }
    }
}
