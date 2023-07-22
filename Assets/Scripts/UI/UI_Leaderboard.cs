using System;
using LootLocker.Requests;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utility;

namespace UI
{
    public class UI_Leaderboard : MonoBehaviour
    {
        private enum LeaderBoardPage
        {
            Top,
            Nearby
        }

        [SerializeField] private LeaderBoardPage _currentPage = LeaderBoardPage.Nearby;
        [Required, ChildGameObjectsOnly] [SerializeField] private UI_List _topList;
        [Required, ChildGameObjectsOnly] [SerializeField] private UI_List _nearbyList;
        [Required, ChildGameObjectsOnly] [SerializeField] private Button _nearbyButton;
        [Required, ChildGameObjectsOnly] [SerializeField] private Button _topButton;
        [Required, ChildGameObjectsOnly] [SerializeField] private TMP_Text _kidFriendlyLabel;

        private LootLockerLeaderboardMember[] _topMembers;
        private LootLockerLeaderboardMember[] _nearbyMembers;

        private void OnEnable()
        {
            SetupUI();

            SwitchPage(_currentPage);

            if (LootLockerTool.instanceExists)
            {
                LootLockerTool.Instance.OnLeaderboardDataUpdated.AddListener(OnLeaderboardDataUpdated);
            }
        }

        private void OnDisable()
        {
            if (LootLockerTool.instanceExists)
            {
                LootLockerTool.Instance.OnLeaderboardDataUpdated.RemoveListener(OnLeaderboardDataUpdated);
            }
        }

        private void SetupUI()
        {
            // Get players' scores.
            _topMembers = LootLockerTool.Instance.TopMembers;
            _nearbyMembers = LootLockerTool.Instance.NearbyMembers;
            _topList.Initialize(_topMembers);
            _nearbyList.Initialize(_nearbyMembers);

            // Player info
            if (_nearbyMembers.IsNullOrEmpty() == false)
            {
                _nearbyButton.gameObject.SetActive(true);

                _topList.SetSelectedRow(LootLockerTool.Instance.Rank);
                _nearbyList.SetSelectedRow(LootLockerTool.Instance.Rank);
            }
            else
            {
                _currentPage = LeaderBoardPage.Top;
                _nearbyButton.gameObject.SetActive(false);
            }

            // Kid-friendly Table
            if (PlayerPrefs.GetInt(PrefKeys.IsInKidMode) == 1)
            {
                _kidFriendlyLabel.text = "kid-friendly leaderboard <b>IS</b> enabled.";
            }
            else
            {
                _kidFriendlyLabel.text = "kid-friendly leaderboard is <b>NOT</b> enabled.";
            }

            switch (_currentPage)
            {
                case LeaderBoardPage.Top:
                    EventSystem.current.SetSelectedGameObject(_topButton.gameObject);
                    break;
                case LeaderBoardPage.Nearby:
                    EventSystem.current.SetSelectedGameObject(_nearbyButton.gameObject);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void ShowTopLeaderboard()
        {
            SwitchPage(LeaderBoardPage.Top);
        }

        public void ShowNearbyLeaderboard()
        {
            SwitchPage(LeaderBoardPage.Nearby);
        }

        private void SwitchPage(LeaderBoardPage newPage)
        {
            _currentPage = newPage;

            ShowPage(_currentPage);
        }

        private void ShowPage(LeaderBoardPage page)
        {
            switch (page)
            {
                case LeaderBoardPage.Top:
                    _topList.gameObject.SetActive(true);
                    _nearbyList.gameObject.SetActive(false);
                    break;
                case LeaderBoardPage.Nearby:
                    _topList.gameObject.SetActive(false);
                    _nearbyList.gameObject.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(page), page, null);
            }
        }

        public void ResetData()
        {
            SetupUI();
        }

        private void OnLeaderboardDataUpdated()
        {
            ResetData();
        }
    }
}