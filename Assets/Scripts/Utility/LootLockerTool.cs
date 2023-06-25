using System;
using LootLocker.Requests;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Events;

// Lootlocker source: https://docs.lootlocker.com/players/authentication/guest-login
namespace Utility
{
    public class LootLockerTool : Singleton<LootLockerTool>
    {
        [Title("Settings")]
        [SerializeField] private string _leaderboardKey = "blobs_leaderboard";
        [SerializeField] private int _downloadCount = 11;
        [SerializeField, ReadOnly] private string _memberID;
        [SerializeField] private string _playerName;
        [Space]
        [Header("Data")]
        [SerializeField] private LootLockerLeaderboardMember[] _members;
        [Header("Events")]
        public UnityEvent OnPlayerNameSet;
        public UnityEvent OnSessionSetupCompleted;
        public UnityEvent OnTopScoresDownloadComplete;
        public UnityEvent OnNearbyScoresDownloadComplete;

        // Public properties
        public LootLockerLeaderboardMember[] Members => _members;

        public string PlayerName => _playerName;

        #region Lifecycle
        private void Start()
        {
            SetupLootLockerGuestSession();
        }
        #endregion

        #region Setup
        private void SetupLootLockerGuestSession()
        {
            LootLockerSDKManager.StartGuestSession(response =>
            {
                if (!response.success)
                {
                    Debug.Log("Error starting Lootlocker session");
                    return;
                }

                SetupDidComplete();
                Debug.Log("Successfully started LootLocker session");
            });
        }

        private void SetupDidComplete()
        {
            _members = GetTopScores();
        }
        #endregion

        public void SubmitScore(int score)
        {
            var memberID = _memberID.IsNullOrWhitespace() ? "Test" : _memberID;

            LootLockerSDKManager.SubmitScore(memberID, score, _leaderboardKey, response =>
            {
                if (response.statusCode == 200)
                {
                    Debug.Log($"Submit Score Successful – memberID: {_memberID} | score: {score} | leaderboardKey: {_leaderboardKey}");
                }
                else
                {
                    Debug.Log("Submit Score Failed: " + response.Error);
                }
            });
        }

        private LootLockerLeaderboardMember[] GetTopScores()
        {
            LootLockerSDKManager.GetScoreList(_leaderboardKey, _downloadCount, 0, response =>
            {
                if (response.statusCode == 200) {
                    Debug.Log($"Leaderboard Get Top Scores Successful – {response.items.Length} item(s) downloaded.");

                    foreach (LootLockerLeaderboardMember member in response.items)
                        Debug.Log(member.ToString());

                    _members = response.items;
                } else {
                    Debug.Log("Leaderboard Get Top Scores Failed: " + response.Error);
                }
            });

            return Array.Empty<LootLockerLeaderboardMember>();
        }

        private LootLockerLeaderboardMember[] GetScoresAroundMember()
        {
            LootLockerSDKManager.GetMemberRank(_leaderboardKey, _memberID, response =>
            {
                if (response.statusCode == 200)
                {
                    int rank = response.rank;
                    int count = _downloadCount;
                    int after = rank < 6 ? 0 : rank - 5;

                    LootLockerSDKManager.GetScoreList(_leaderboardKey, count, after, response =>
                    {
                        if (response.statusCode == 200)
                        {
                            Debug.Log($"Leaderboard Get Scores Around Member Successful – {response.items.Length} item(s) downloaded.");

                            foreach (LootLockerLeaderboardMember member in response.items)
                                Debug.Log(member.ToString());

                            _members = response.items;
                        }

                        Debug.Log("Leaderboard Get Scores Around Member Failed: " + response.Error);
                    });
                }
                else
                {
                    Debug.Log("Leaderboard Get Member Rank Failed: " + response.Error);
                }
            });

            return Array.Empty<LootLockerLeaderboardMember>();;
        }

        public void SetPlayerName(string playerName)
        {
            LootLockerSDKManager.SetPlayerName(playerName, (response) =>
            {
                if (response.success)
                {
                    Debug.Log($"Successfully set player name:{_playerName}({_memberID})");
                    _playerName = playerName;
                    OnPlayerNameSet?.Invoke();
                }
                else
                {
                    Debug.Log("Error setting player name");
                }
            });
        }
    }
}