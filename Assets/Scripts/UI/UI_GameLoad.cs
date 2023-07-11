using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Utility;

namespace UI
{
    public class UI_GameLoad : MonoBehaviour
    {
        [SerializeField] private TMP_Text _responseLabel;

        #region Lifecycle
        private void Start()
        {
            _responseLabel.text = string.Empty;
        }

        private void OnEnable()
        {
            if (LootLockerTool.instanceExists)
            {
                LootLockerTool.Instance.OnSetupComplete.AddListener(OnLeaderboardSetup);
            }
        }

        private void OnDisable()
        {
            if (LootLockerTool.instanceExists)
            {
                LootLockerTool.Instance.OnSetupComplete.RemoveListener(OnLeaderboardSetup);
            }
        }
        #endregion

        private void OnLeaderboardSetup(bool isSuccess, string response)
        {
            _responseLabel.text = response;
            _responseLabel.color = isSuccess ? ColorConstants.Yellow : ColorConstants.Red;
            _responseLabel.transform.DOPunchScale(Vector3.one * 0.2f, 1.0f, 3)
                .OnComplete(() => StartCoroutine(ChangeSceneToStartAfterDelay(2f)));
        }

        private IEnumerator ChangeSceneToStartAfterDelay(float time)
        {
            yield return new WaitForSeconds(time);
            SceneFader.Instance.FadeToStart();
        }
    }
}