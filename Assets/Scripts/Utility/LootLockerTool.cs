using Exclude;
using LootLocker.Requests;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Events;

// Lootlocker source: https://docs.lootlocker.com/players/authentication/guest-login
// Script execution order modified.
// TODO: Remove the modified Script execution order when GameLoad scene is complete and working.

namespace Utility
{
    public class LootLockerTool : Singleton<LootLockerTool>
    {
        [Title("Leaderboard", "Responsible for leaderboard functionality.")]
        [SerializeField, DisplayAsString] private bool _isInitialized;
        [Title("Settings", horizontalLine: false)]
        [Space]
        [MinValue(1)] [MaxValue(100)]
        [Tooltip("Number of leaderboard members to download for display in the leaderboard UI. Current needed is 11 members to make UI look nice.")]
        [SerializeField] private int _downloadCount = 11;
        [Tooltip("Lootlocker key, required to correctly connect to the leaderboard data.")]
        [SerializeField] private string _leaderboardKey = GameExcludeConstants.LootLockerKey;
        [LabelText("Leaderboard Member Data")]
        [SerializeField] private LootLockerLeaderboardMember[] _topMembers;
        [SerializeField] private LootLockerLeaderboardMember[] _nearbyMembers;

        [Title("Player Info")]
        [SerializeField] [ReadOnly] private string _playerName;
        [SerializeField] [ReadOnly] private string _memberID;
        [SerializeField] [ReadOnly] private int _playerID;
        [SerializeField] [ReadOnly] private string _playerPublicUID;
        [SerializeField] [ReadOnly] private int _rank;
        [SerializeField] [ReadOnly] private int _highScore;
        [SerializeField] private bool _hasPlayerInfo;

        [PropertyOrder(100)] [Space]
        [SerializeField] private bool _showDebug;

        // Flags
        private bool _hasAttemptedTopLeaderboardData;
        private bool _hasAttemptedNearbyLeaderboardData;
        private bool _hasAttemptedPlayerData;

        #region Public Properties
        public LootLockerLeaderboardMember[] TopMembers => _topMembers;
        public LootLockerLeaderboardMember[] NearbyMembers => _nearbyMembers;
        public string PlayerName => _playerName;
        public bool IsInitialized => _isInitialized;
        public int Rank => _rank;
        public int HighScore => _highScore;

        public bool HasPlayerInfo => _hasPlayerInfo;
        #endregion

        [TitleGroup("Unity Events")]
        [FoldoutGroup("Unity Events/Events", false)] public UnityEvent<string> OnPlayerNameUpdated;
        [FoldoutGroup("Unity Events/Events")] public UnityEvent<bool, string> OnSetupComplete; // isSuccess, response

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
                if (response.success)
                {
#if UNITY_EDITOR
                    if (_showDebug)
                        Debug.Log($"<color=#58AE91>––LootLocker––Successfully started LootLocker session with player ID: {response.player_id}</color>");
#endif

                    _memberID = response.player_id.ToString();
                    _playerID = response.player_id;


                    _isInitialized = true;

                    GetPlayerData();
                    GetTopScores();
                }
                else
                {
                    Debug.LogWarning($"<color=ECAB34>––LootLocker––Error starting Lootlocker session | error: {response.Error}</color>");
                }
            });
        }
        #endregion

        #region Scores
        public void GetTopScores()
        {
            LootLockerSDKManager.GetScoreList(_leaderboardKey, _downloadCount, 0, response =>
            {
                if (response.statusCode == 200)
                {
                    _topMembers = response.items;

#if UNITY_EDITOR
                    if (_showDebug)
                    {
                        Debug.Log($"<color=#58AE91>––LootLocker––Get Top Scores Successful – {response.items.Length} leaderboard member(s) downloaded.</color>");
                        // LogMemberData(response.items);
                    }
#endif
                }
                else
                {
                    Debug.LogWarning($"<color=ECAB34>––LootLocker––Get Top Scores Failed: {response.Error}</color>");
                }

                _hasAttemptedTopLeaderboardData = true;
                CheckForDataCompletelyDownloaded();
            });
        }

        private void GetScoresAroundMember()
        {
            int rank = _rank;
            int count = _downloadCount;
            int after = rank < 6 ? 0 : rank - 5;

            LootLockerSDKManager.GetScoreList(_leaderboardKey, count, after, scoreResponse =>
            {
                if (scoreResponse.statusCode == 200)
                {
                    _nearbyMembers = scoreResponse.items;
#if UNITY_EDITOR
                    if (_showDebug)
                    {
                        Debug.Log($"<color=#58AE91>––LootLocker––Leaderboard Get Scores Around Member Successful – {scoreResponse.items.Length} leaderboard member(s) downloaded.</color>");
                        // LogMemberData(scoreResponse.items);
                    }
#endif
                }
                else
                {
                    Debug.LogWarning("<color=ECAB34>––LootLocker––Leaderboard Get Scores Around Member Failed: {scoreResponse.Error}</color>");
                }

                _hasAttemptedNearbyLeaderboardData = true;
                CheckForDataCompletelyDownloaded();
            });
        }

        public void SubmitPlayerScore(int score)
        {

            if (score <= 0)
            {
                Debug.Log($"––LootLocker––Score is 0 and was not submitted to leaderboard.");
                return;
            }

            string memberID = _memberID.IsNullOrWhitespace() ? "Test" : _memberID;

            LootLockerSDKManager.SubmitScore(memberID, score, _leaderboardKey, response =>
            {
                if (response.statusCode == 200)
                {
#if UNITY_EDITOR
                    Debug.Log($"<color=#58AE91>––LootLocker––Submit Score Successful–memberID: {memberID} | player name: {_playerName} | score: {score} | leaderboard: {_leaderboardKey}</color>");
#endif
                }
                else
                {
                    Debug.LogWarning($"<color=#ECAB34>––LootLocker––Submit Score Failed: {response.Error}</color>");
                }
            });
        }
        #endregion

        #region Player Data
        private void GetPlayerData()
        {
            LootLockerSDKManager.GetMemberRank(_leaderboardKey, _memberID, response =>
            {
                if (response.statusCode == 200)
                {
                    _rank = response.rank;
                    _highScore = response.score;
                    _memberID = response.member_id;
                    _playerName = response.player.name;
                    _playerID = response.player.id;
                    _playerPublicUID = response.player.public_uid;
                    _hasPlayerInfo = true;
                    OnPlayerNameUpdated?.Invoke(_playerName);
#if UNITY_EDITOR
                    Debug.Log("<color=#58AE91>––LootLocker––Successfully retrieved player data</color>");
#endif

                    GetScoresAroundMember();
                }
                else
                {
                    Debug.Log("<color=ECAB34>––LootLocker––Error getting player name</color>");
                }

                _hasAttemptedPlayerData = true;
                CheckForDataCompletelyDownloaded();
            });
        }

        public void UpdatePlayerName(string playerName)
        {
            LootLockerSDKManager.SetPlayerName(playerName, response =>
            {
                if (response.success)
                {
                    _playerName = playerName;
#if UNITY_EDITOR
                    Debug.Log($"<color=#58AE91>––LootLocker––Successfully set player name: {_playerName} (ID: {_playerID})</color>");
#endif
                    GetTopScores();
                    GetScoresAroundMember();

                    OnPlayerNameUpdated?.Invoke(playerName);
                }
                else
                {
                    Debug.Log("<color=ECAB34>––LootLocker––Error setting player name</color>");
                }

                _hasAttemptedPlayerData = true;
            });
        }
        #endregion

        #region Callbacks
        private void CheckForDataCompletelyDownloaded()
        {
            if (!_hasAttemptedTopLeaderboardData || !_hasAttemptedNearbyLeaderboardData || !_hasAttemptedPlayerData) return;

            // All data loading is done
            string outputText = string.IsNullOrEmpty(_playerName) ? "Yay! All done loading." : $"Hey there {_playerName}!";
            OnSetupComplete?.Invoke(true, outputText);
        }

        private void LogMemberData(LootLockerLeaderboardMember[] members)
        {
            foreach (LootLockerLeaderboardMember member in members) Debug.Log(GetFormattedLeaderboardString(member));
        }

        private string GetFormattedLeaderboardString(LootLockerLeaderboardMember member)
        {
            return $"<color=grey>\tMemberID: {member.member_id} | Score: {member.score} –– Player Name: {member.player.name} | Player ID: {member.player.id} | Player UID: {member.player.public_uid}</color>";
        }
        #endregion
    }
}