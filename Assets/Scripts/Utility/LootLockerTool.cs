using LootLocker.Requests;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

// Lootlocker source: https://docs.lootlocker.com/players/authentication/guest-login
// Script execution order modified.
namespace Utility
{
    public class LootLockerTool : Singleton<LootLockerTool>
    {
        #region Private Fields
        [Title("Leaderboard", "Responsible for leaderboard functionality.")]
        [SerializeField, DisplayAsString] private bool _isInitialized;

        [Title("Player Info")]
        [DisplayAsString]
        [SerializeField] private bool _hasPlayerInfo;
        [SerializeField] [ReadOnly] private string _playerPublicUID;
        [SerializeField] [ReadOnly] private string _playerName;
        [SerializeField] [ReadOnly] private string _memberID;
        [SerializeField] [ReadOnly] private string _playerIdentifier;
        private int _playerID;

        [Title("Leaderboard Settings", horizontalLine: false)]
        [Space]
        [MinValue(1)] [MaxValue(100)]
        [Tooltip("Number of leaderboard members to download for display in the leaderboard UI. Current needed is 11 members to make UI look nice.")]
        [SerializeField] private int _downloadCount = 11;
        [Tooltip("Lootlocker key, required to correctly connect to the leaderboard data.")]
        [SerializeField] private string _leaderboardKey = "production_blobber_leaderboard";
        [LabelText("Leaderboard Member Data")]
        private LootLockerLeaderboardMember[] _topMembers;
        private LootLockerLeaderboardMember[] _nearbyMembers;

        [Title("Leaderboard Info")]
        [SerializeField] [ReadOnly] private int _rank;
        [Indent] [SerializeField] [ReadOnly] private int _previousRank;
        [SerializeField] [ReadOnly] private int _highScore;
        [Indent] [SerializeField] [ReadOnly] private int _previousHighScore;

        [PropertyOrder(100)] [Space]
        [SerializeField] private bool _showDebug;

        // Flags
        private bool _hasAttemptedTopLeaderboardData;
        private bool _hasAttemptedNearbyLeaderboardData;
        private bool _hasAttemptedPlayerData;
        #endregion

        #region Public Properties
        public LootLockerLeaderboardMember[] TopMembers => _topMembers;
        public LootLockerLeaderboardMember[] NearbyMembers => _nearbyMembers;
        public string PlayerName => _playerName;
        public bool IsInitialized => _isInitialized;
        public int PreviousRank => _previousRank;
        public string PrettyPreviousRank => "# " + NumberFormatter.FormatNumberWithCommas(_previousRank);
        public int Rank => _rank;
        public string PrettyRank => "# " + NumberFormatter.FormatNumberWithCommas(_rank);
        public int PreviousHighScore => _previousHighScore;
        public string PrettyPreviousHighScore => NumberFormatter.FormatNumberWithCommas(_previousHighScore);
        public int HighScore => _highScore;
        public string PrettyHighScore => NumberFormatter.FormatNumberWithCommas(_highScore);
        public bool HasPlayerInfo => _hasPlayerInfo;
        #endregion

        #region Events
        [TitleGroup("Unity Events")]
        [FoldoutGroup("Unity Events/Events")] public UnityEvent<bool, string> OnSetupComplete; // isSuccess, response
        [FoldoutGroup("Unity Events/Events", false)] public UnityEvent OnPlayerDataUpdated;
        [FoldoutGroup("Unity Events/Events", false)] public UnityEvent OnLeaderboardDataUpdated;
        #endregion

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
                    Debug.LogWarning($"<color=ECAB34>LootLocker––Error starting Lootlocker session | error: {response.Error}</color>");
                }
            });
        }

        private void OnGuestSessionSetupCompleted(LootLockerGuestSessionResponse response)
        {

#if UNITY_EDITOR
            if (_showDebug) Debug.Log($"<color=#58AE91>LootLocker––Successfully started LootLocker session with player ID: {response.player_id}</color>");
#endif
            _playerPublicUID = response.public_uid;
            _playerID = response.player_id;
            _memberID = response.player_id.ToString();
            _playerIdentifier = response.player_identifier;

            _isInitialized = true;

            GetPlayerData();
        }
        #endregion

        #region Leaderboard Data
        private void GetLeaderboardData()
        {
            if (_rank > 0)
                GetLeaderboardScoresNearby();
            else
                _hasAttemptedNearbyLeaderboardData = true;

            GetLeaderboardScoresTop();
        }
        #endregion

        #region Top Leaderboard Scores
        private void GetLeaderboardScoresTop()
        {
            LootLockerSDKManager.GetScoreList(_leaderboardKey, _downloadCount, 0, response =>
            {
                if (response.statusCode == 200)
                {
                    OnGetTopScores_Completed(response);
                }
                else
                {
                    Debug.LogWarning($"<color=ECAB34>LootLocker––Get Top Scores Failed: {response.Error}</color>");
                }

                _hasAttemptedTopLeaderboardData = true;
                CheckForDataCompletelyDownloaded();
            });
        }

        private void OnGetTopScores_Completed(LootLockerGetScoreListResponse response)
        {
#if UNITY_EDITOR
            if (_showDebug) Debug.Log($"<color=#58AE91>LootLocker––Get top scores success – {response.items.Length} leaderboard member(s) downloaded.</color>");
#endif
            _topMembers = response.items;
        }
        #endregion

        #region Nearby Leaderboard Scores
        private void GetLeaderboardScoresNearby()
        {
            int rank = _rank;
            int count = _downloadCount;
            int after = rank < 6 ? 0 : rank - 5;

            LootLockerSDKManager.GetScoreList(_leaderboardKey, count, after, response =>
            {
                if (response.statusCode == 200)
                {
                    OnGetNearbyScores_Completed(response);
                }
                else
                {
                    Debug.LogWarning("<color=ECAB34>LootLocker––Leaderboard Get Scores Around Member Failed: {scoreResponse.Error}</color>");
                }

                _hasAttemptedNearbyLeaderboardData = true;
                CheckForDataCompletelyDownloaded();
            });
        }

        private void OnGetNearbyScores_Completed(LootLockerGetScoreListResponse response)
        {
#if UNITY_EDITOR
            if (_showDebug) Debug.Log($"<color=#58AE91>LootLocker––Get nearby scores success – {response.items.Length} leaderboard member(s) downloaded.</color>");
#endif
            _nearbyMembers = response.items;
        }
        #endregion

        #region Submit Score
        public void SubmitPlayerScore(int score)
        {
            if (score <= 0)
            {
                Debug.Log($"LootLocker––Score is 0 and was not submitted to leaderboard.");
                return;
            }

            if (score < _highScore)
            {
                Debug.Log($"LootLocker––Score ({score}) is less than previous high score ({_highScore}) and was not submitted to leaderboard.");
                return;
            }

            LootLockerSDKManager.SubmitScore(_memberID, score, _leaderboardKey, response =>
            {
                if (response.statusCode == 200)
                {
                    OnScoreSubmit_Completed(score, response);
                }
                else
                {
                    Debug.LogWarning($"<color=#ECAB34>LootLocker––Submit score failed: {response.Error}</color>");
                }
            });
        }

        private void OnScoreSubmit_Completed(int sentScore, LootLockerSubmitScoreResponse response)
        {
#if UNITY_EDITOR
            Debug.Log($"<color=#58AE91>LootLocker––Submit score success–memberID: {_memberID} | player name: {_playerName} | score: (sent {sentScore}) (received: {_highScore}) | leaderboard: {_leaderboardKey}</color>");
#endif
            _highScore = sentScore;
            _rank = response.rank;

            OnPlayerDataUpdated?.Invoke();

            GetLeaderboardData();
        }
        #endregion

        #region Player Data
        private void GetPlayerData()
        {
            LootLockerSDKManager.GetMemberRank(_leaderboardKey, _memberID, response =>
            {
                if (response.statusCode == 200)
                {
                    OnGetPlayerData_Completed(response);
                }
                else
                {
                    Debug.Log("<color=ECAB34>LootLocker––Error getting player name</color>");
                }

                _hasAttemptedPlayerData = true;
                CheckForDataCompletelyDownloaded();
            });
        }

        private void OnGetPlayerData_Completed(LootLockerGetMemberRankResponse response)
        {
#if UNITY_EDITOR
            Debug.Log("<color=#58AE91>LootLocker––Get player data success</color>");
#endif
            _rank = _previousRank = response.rank;
            _highScore = _previousHighScore = response.score;
            _memberID = response.member_id;

            if (response.player != null)
            {
                _playerName = response.player.name;
                _playerID = response.player.id;
                _playerPublicUID = response.player.public_uid;
            }

            _hasPlayerInfo = true;

            GetLeaderboardData();

            OnPlayerDataUpdated?.Invoke();
        }

        public void UpdatePlayerName(string playerName)
        {
            LootLockerSDKManager.SetPlayerName(playerName, response =>
            {
                if (response.success)
                {
                    OnUpdatePlayerName_Completed(response);
                }
                else
                {
                    Debug.Log("<color=ECAB34>LootLocker––Error setting player name</color>");
                }
            });
        }

        private void OnUpdatePlayerName_Completed(PlayerNameResponse response)
        {
#if UNITY_EDITOR
            Debug.Log($"<color=#58AE91>LootLocker––Successfully set player name: {_playerName} (ID: {_playerID})</color>");
#endif
            _playerName = response.name;
            _hasAttemptedPlayerData = true;

            OnPlayerDataUpdated?.Invoke();

            GetLeaderboardData();
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