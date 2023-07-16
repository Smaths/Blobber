using System;
using LootLocker.Requests;
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
        [SerializeField] private UI_List _topList;
        [SerializeField] private UI_List _nearbyList;
        [SerializeField] private Button _nearbyButton;
        [SerializeField] private Button _topButton;

        [Header("Player")]
        [SerializeField] private GameObject _playerContainer;
        [SerializeField] private TMP_Text _playerLabel;
        [SerializeField] private TMP_Text _rankLabel;
        [SerializeField] private TMP_Text _scoreLabel;

        private LootLockerLeaderboardMember[] _topMembers;
        private LootLockerLeaderboardMember[] _nearbyMembers;

        private void OnEnable()
        {
            // Get players' scores.
            _topMembers = LootLockerTool.Instance.TopMembers;
            _nearbyMembers = LootLockerTool.Instance.NearbyMembers;
            _topList.Initialize(_topMembers);
            _nearbyList.Initialize(_nearbyMembers);

            // Player info
            if (LootLockerTool.Instance.HasPlayerInfo)
            {
                _nearbyButton.gameObject.SetActive(true);
                _playerContainer.gameObject.SetActive(true);
                _playerLabel.text = LootLockerTool.Instance.PlayerName;
                _rankLabel.text = NumberFormatter.FormatNumberWithCommas(LootLockerTool.Instance.Rank);
                _scoreLabel.text = NumberFormatter.FormatNumberWithCommas(LootLockerTool.Instance.HighScore);

                _topList.SetSelectedRow(LootLockerTool.Instance.Rank);
                _nearbyList.SetSelectedRow(LootLockerTool.Instance.Rank);
            }
            else
            {
                _currentPage = LeaderBoardPage.Top;
                _nearbyButton.gameObject.SetActive(false);
                _playerContainer.gameObject.SetActive(false);
            }

            switch (_currentPage)
            {
                case LeaderBoardPage.Top:
                    EventSystem.current.SetSelectedGameObject(_topButton.gameObject);;
                    break;
                case LeaderBoardPage.Nearby:
                    EventSystem.current.SetSelectedGameObject(_nearbyButton.gameObject);;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            SwitchPage(_currentPage);
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
    }
}