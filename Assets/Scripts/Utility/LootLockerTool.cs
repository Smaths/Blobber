using LootLocker.Requests;
using Sirenix.OdinInspector;
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
        [SerializeField] private string _leaderboardKey = "production_blobber_leaderboard";
        [LabelText("Leaderboard Member Data")]
        [SerializeField] private LootLockerLeaderboardMember[] _topMembers;
        [SerializeField] private LootLockerLeaderboardMember[] _nearbyMembers;

        [Title("Player Info")]
        [SerializeField] [ReadOnly] private string _playerName;
        [SerializeField] [ReadOnly] private string _memberID;
        [SerializeField] [ReadOnly] private int _playerID;
        [SerializeField] [ReadOnly] private string _playerPublicUID;
        [SerializeField] [ReadOnly] private int _rank;
        [SerializeField] [ReadOnly] private int _previousHighScore;
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
        [FoldoutGroup("Unity Events/Events")] public UnityEvent<bool, string> OnSetupComplete; // isSuccess, response
        [FoldoutGroup("Unity Events/Events", false)] public UnityEvent OnPlayerDataUpdated;
        [FoldoutGroup("Unity Events/Events", false)] public UnityEvent OnLeaderboardDataUpdated;

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
                    OnGuestSessionSetupCompleted(response);
                }
                else
                {
                    Debug.LogWarning($"<color=ECAB34>––LootLocker––Error starting Lootlocker session | error: {response.Error}</color>");
                }
            });
        }

        private void OnGuestSessionSetupCompleted( LootLockerGuestSessionResponse response)
        {

#if UNITY_EDITOR
            if (_showDebug) Debug.Log($"<color=#58AE91>––LootLocker––Successfully started LootLocker session with player ID: {response.player_id}</color>");
#endif
            _memberID = response.player_id.ToString();
            _playerID = response.player_id;

            _isInitialized = true;

            GetPlayerData();
            GetTopScores();
            GetNearbyScores();
        }
        #endregion

        #region Scores
        #region Top Scores
        public void GetTopScores()
        {
            LootLockerSDKManager.GetScoreList(_leaderboardKey, _downloadCount, 0, response =>
            {
                if (response.statusCode == 200)
                {
                    OnGetTopScoresCompleted(response);
                }
                else
                {
                    Debug.LogWarning($"<color=ECAB34>––LootLocker––Get Top Scores Failed: {response.Error}</color>");
                }

                _hasAttemptedTopLeaderboardData = true;
                CheckForDataCompletelyDownloaded();
            });
        }

        private void OnGetTopScoresCompleted(LootLockerGetScoreListResponse response)
        {
#if UNITY_EDITOR
            if (_showDebug) Debug.Log($"<color=#58AE91>––LootLocker––Get Top Scores Successful – {response.items.Length} leaderboard member(s) downloaded.</color>");
#endif
            _topMembers = response.items;
        }
        #endregion

        #region Nearby Scores
        private void GetNearbyScores()
        {
            int rank = _rank;
            int count = _downloadCount;
            int after = rank < 6 ? 0 : rank - 5;

            LootLockerSDKManager.GetScoreList(_leaderboardKey, count, after, response =>
            {
                if (response.statusCode == 200)
                {
                    OnGetNearbyScoresCompleted(response);
                }
                else
                {
                    Debug.LogWarning("<color=ECAB34>––LootLocker––Leaderboard Get Scores Around Member Failed: {scoreResponse.Error}</color>");
                }

                _hasAttemptedNearbyLeaderboardData = true;
                CheckForDataCompletelyDownloaded();
            });
        }

        private void OnGetNearbyScoresCompleted(LootLockerGetScoreListResponse response)
        {
#if UNITY_EDITOR
            if (_showDebug) Debug.Log($"<color=#58AE91>––LootLocker––Leaderboard Get Scores Around Member Successful – {response.items.Length} leaderboard member(s) downloaded.</color>");
#endif
            _nearbyMembers = response.items;
        }
        #endregion

        #region Submit Score
        public void SubmitPlayerScore(int score)
        {
            if (score <= 0)
            {
                Debug.Log($"––LootLocker––Score is 0 and was not submitted to leaderboard.");
                return;
            }

            LootLockerSDKManager.SubmitScore(_memberID, score, _leaderboardKey, response =>
            {
                if (response.statusCode == 200)
                {
                    OnScoreSubmitCompleted(score, response);
                }
                else
                {
                    Debug.LogWarning($"<color=#ECAB34>––LootLocker––Submit Score Failed: {response.Error}</color>");
                }
            });
        }

        private void OnScoreSubmitCompleted(int sentScore, LootLockerSubmitScoreResponse response)
        {
#if UNITY_EDITOR
            Debug.Log($"<color=#58AE91>––LootLocker––Submit Score Successful–memberID: {_memberID} | player name: {_playerName} | score: (sent {sentScore}) (received: {_highScore}) | leaderboard: {_leaderboardKey}</color>");
#endif
            _rank = response.rank;
            _highScore = response.score;

            // Get updated data after submitting
            GetTopScores();
            GetNearbyScores();
        }
        #endregion
        #endregion

        #region Player Data
        private void GetPlayerData()
        {
            LootLockerSDKManager.GetMemberRank(_leaderboardKey, _memberID, response =>
            {
                if (response.statusCode == 200)
                {
                    OnGetPlayerDataCompleted(response);
                }
                else
                {
                    Debug.Log("<color=ECAB34>––LootLocker––Error getting player name</color>");
                }

                _hasAttemptedPlayerData = true;
                CheckForDataCompletelyDownloaded();
            });
        }

        private void OnGetPlayerDataCompleted(LootLockerGetMemberRankResponse response)
        {
#if UNITY_EDITOR
            Debug.Log("<color=#58AE91>––LootLocker––Successfully retrieved player data</color>");
#endif
            _rank = response.rank;
            _highScore = _previousHighScore = response.score;
            _memberID = response.member_id;
            _playerName = response.player.name;
            _playerID = response.player.id;
            _playerPublicUID = response.player.public_uid;
            _hasPlayerInfo = true;

            OnPlayerDataUpdated?.Invoke();

            GetNearbyScores();
        }

        public void UpdatePlayerName(string playerName)
        {
            LootLockerSDKManager.SetPlayerName(playerName, response =>
            {
                if (response.success)
                {
                    OnUpdatePlayerNameCompleted(response);
                }
                else
                {
                    Debug.Log("<color=ECAB34>––LootLocker––Error setting player name</color>");
                }
            });
        }

        private void OnUpdatePlayerNameCompleted(PlayerNameResponse response)
        {
#if UNITY_EDITOR
            Debug.Log($"<color=#58AE91>––LootLocker––Successfully set player name: {_playerName} (ID: {_playerID})</color>");
#endif
            _playerName = response.name;
            _hasAttemptedPlayerData = true;

            OnPlayerDataUpdated?.Invoke();

            GetTopScores();
            GetNearbyScores();
        }
        #endregion

        #region Callbacks
        private void CheckForDataCompletelyDownloaded()
        {
            if (!_hasAttemptedTopLeaderboardData) return;
            if (!_hasAttemptedNearbyLeaderboardData) return;
            if (!_hasAttemptedPlayerData) return;

            // All data loading is done
            string outputText = string.IsNullOrEmpty(_playerName) ? "Yay! All done loading." : $"Hey there {_playerName}!";
            OnSetupComplete?.Invoke(true, outputText);
            OnLeaderboardDataUpdated?.Invoke();
        }

        private string GetFormattedLeaderboardString(LootLockerLeaderboardMember member)
        {
            return $"<color=grey>\tMemberID: {member.member_id} | Score: {member.score} –– Player Name: {member.player.name} | Player ID: {member.player.id} | Player UID: {member.player.public_uid}</color>";
        }
        #endregion

        // Logging
        private void LogMemberData(LootLockerLeaderboardMember[] members)
        {
            foreach (LootLockerLeaderboardMember member in members) Debug.Log(GetFormattedLeaderboardString(member));
        }
    }
}