using System.Collections.Generic;
using System.Linq;
using LootLocker.Requests;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UI
{
    public class UI_List : MonoBehaviour
    {
        [SerializeField] private GameObject _rowItemPrefab;
        [SerializeField] private string _missingDataText = "No player data";
        [SerializeField] private List<UI_ScoreRowItem> _rowItems;

        private LootLockerLeaderboardMember[] _members;

        public void Initialize(LootLockerLeaderboardMember[] members)
        {
            _members = members;

            if (_members == null || _members.Length == 0)
            {
#if UNITY_EDITOR
                Debug.Log($"{gameObject.name} - No list members");
#endif
                ClearRowItems();
                CreateEmptyList(_missingDataText);
                return; // guard
            }

            ClearRowItems();
            CreateRowItems(_members);
        }

        private void ClearRowItems()
        {
            FindRowItems();

            // Delete all existing row items.
            foreach (var item in _rowItems)
                Destroy(item.gameObject);
        }

        private void CreateEmptyList(string tooltipText)
        {
            GameObject go = Instantiate(_rowItemPrefab, transform);
            var rowItem = go.GetComponent<UI_ScoreRowItem>();
            rowItem.Initialize(string.Empty, tooltipText, -1);
            _rowItems.Add(rowItem);
        }

        private void CreateRowItems(IEnumerable<LootLockerLeaderboardMember> members)
        {
            // Create row item for each player's score
            _rowItems = new List<UI_ScoreRowItem>();
            foreach (var member in members)
            {
                if (member == null) continue; // Guard

                GameObject go = Instantiate(_rowItemPrefab, transform);
                var rowItem = go.GetComponent<UI_ScoreRowItem>();
                rowItem.Initialize(member);
                _rowItems.Add(rowItem);
            }
        }

        public void SetSelectedRow(int selectedRank)
        {
            foreach (UI_ScoreRowItem item in _rowItems)
            {
                if (item.Rank == selectedRank.ToString())
                {
                    item.SetSelected();
                }
                else
                {
                    item.SetNormal();
                }
            }
        }

        [Button]
        private void FindRowItems()
        {
            _rowItems = GetComponentsInChildren<UI_ScoreRowItem>().ToList();
        }
    }
}