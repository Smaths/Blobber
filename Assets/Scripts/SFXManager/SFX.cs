using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SFXManager
{
    [Serializable]
    public class SFX
    {
        [LabelText("SFX Type")]
        [LabelWidth(100)]
        [OnValueChanged("SFXChange")]
        [InlineButton("PlaySFX")]
        public SFXManager.SFXType sfxType = SFXManager.SFXType.SFX;

        [LabelText("$_sfxLabel")]
        [LabelWidth(100)]
        [ValueDropdown("SFXType")]
        [OnValueChanged("SFXChange")]
        [InlineButton("SelectSFX")]
        public SFXClip sfxToPlay;
        private string _sfxLabel = "SFX";

        [Space]
        [SerializeField]
        private bool _showSettings;
        [SerializeField]
        private bool _editSettings;

        [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
        [ShowIf("_showSettings")]
        [EnableIf("_editSettings")]
        [SerializeField]
        private SFXClip _sfxBase;

        [Title("Audio Source")]
        [ShowIf("_showSettings")]
        [EnableIf("_editSettings")]
        [SerializeField]
        private bool _waitToPlay = true;

        [ShowIf("_showSettings")]
        [EnableIf("_editSettings")]
        [SerializeField]
        private bool _useDefault = true;

        [DisableIf("_useDefault")]
        [ShowIf("_showSettings")]
        [EnableIf("_editSettings")]
        [SerializeField]
        private AudioSource _audioSource;

        private void SFXChange()
        {
            //keep the label up to date.
            _sfxLabel = sfxType + " SFX";
            // Keep the displayed "SFX Clip" up to date.
            _sfxBase = sfxToPlay;
        }

        private void SelectSFX()
        {
            UnityEditor.Selection.activeObject = sfxToPlay;
        }

        // Get list of SFX from manager, used in inspector
        private List<SFXClip> SFXType()
        {
            List<SFXClip> sfxList = sfxType switch
            {
                SFXManager.SFXType.UI => SFXManager.Instance.uiSFX,
                SFXManager.SFXType.Ambient => SFXManager.Instance.ambientSFX,
                SFXManager.SFXType.SFX => SFXManager.Instance.gameSFX,
                _ => throw new ArgumentOutOfRangeException()
            };

            return sfxList;
        }

        public void PlaySFX()
        {
            if (_useDefault || _audioSource == null)
                SFXManager.PlaySFX(sfxToPlay, _waitToPlay); // use default audio source
            else
                SFXManager.PlaySFX(sfxToPlay, _waitToPlay, _audioSource);
        }
    }
}