using System.Collections.Generic;
using System.Linq;
using LootLocker.Requests;
using Sirenix.OdinInspector;
using UnityEngine;
using Utility;

namespace UI
{
    public class UI_Leaderboard : MonoBehaviour
    {
        [SerializeField] private GameObject _leaderboardContainer;
        [SerializeField] private GameObject _rowItemPrefab;
        [SerializeField] private List<UI_ScoreRowItem> _rowItems;

        private LootLockerLeaderboardMember[] _members;

        private void OnEnable()
        {
            // Get players' scores.
            _members = LootLockerTool.Instance.Members;

            if (_members == null || _members.Length == 0) 
            {
                print($"{gameObject.name} - No scoreboard members");
                return; // guard
            }

            // Delete all existing row items.
            foreach (var item in _rowItems)
                Destroy(item.gameObject);

            // Create row item for each player's score
            _rowItems = new List<UI_ScoreRowItem>();
            foreach (var member in _members)
            {
                if (member == null) continue;   // Guard

                GameObject go = Instantiate(_rowItemPrefab, _leaderboardContainer.transform);
                var rowItem = go.GetComponent<UI_ScoreRowItem>();
                rowItem.Initialize(member);
                _rowItems.Add(rowItem);
            }
        }

        [Button("Get Row Items")]
        private void GetRowItemGameObjects()
        {
            _rowItems = GetComponentsInChildren<UI_ScoreRowItem>().ToList();
        }
    }
}