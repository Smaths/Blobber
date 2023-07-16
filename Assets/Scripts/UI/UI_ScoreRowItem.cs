using System.Globalization;
using LootLocker.Requests;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace UI
{
    public class UI_ScoreRowItem : MonoBehaviour
    {
        [SerializeField] private LootLockerLeaderboardMember _leaderboardMember;
        [SerializeField] private Image _background;
        [SerializeField] private TMP_Text _rankLabel;
        [SerializeField] private TMP_Text _playerName;
        [SerializeField] private TMP_Text _scoreLabel;

        public LootLockerLeaderboardMember Member => _leaderboardMember;
        public string Rank => _rankLabel.text;
        public string PlayerName => _playerName.text;
        public string Score => _scoreLabel.text;

        public void Initialize(LootLockerLeaderboardMember member)
        {
            _rankLabel.text = member.rank.ToString(CultureInfo.CurrentCulture);
            _playerName.text = member.player.name.IsNullOrWhitespace() ? "Some blob" : member.player.name;
            _scoreLabel.text = NumberFormatter.FormatNumberWithCommas(member.score);
        }

        public void Initialize(string rank, string playerName, int score)
        {
            _rankLabel.text = rank;
            _playerName.text = playerName;
            _scoreLabel.text = score < 0 ? string.Empty : NumberFormatter.FormatNumberWithCommas(score);
        }

        public void SetSelected()
        {
            _background.enabled = true;
        }

        public void SetNormal()
        {
            _background.enabled = false;
        }
    }
}