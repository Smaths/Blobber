using System.Globalization;
using LootLocker.Requests;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UI_ScoreRowItem : MonoBehaviour
    {
        [SerializeField] private LootLockerLeaderboardMember _leaderboardMember;

        [SerializeField] private TMP_Text _rankLabel;
        [SerializeField] private TMP_Text _playerName;
        [SerializeField] private TMP_Text _scoreLabel;

        public void Initialize(LootLockerLeaderboardMember member)
        {
            _rankLabel.text = member.rank.ToString(CultureInfo.CurrentCulture);
            _playerName.text = member.player.name.IsNullOrWhitespace() ? "Some blob" : member.player.name;
            _scoreLabel.text = member.score.ToString(CultureInfo.CurrentCulture);
        }
    }
}