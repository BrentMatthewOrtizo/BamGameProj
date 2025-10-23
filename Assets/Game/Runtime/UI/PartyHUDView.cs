using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.com.game399.shared.Models;

namespace Game.Runtime.UI
{
    public class PartyHUDView : ObserverMonoBehaviour
    {
        [SerializeField] private Transform slotParent;     // container for slots
        [SerializeField] private GameObject slotPrefab;    // prefab for each monster

        private PlayerPartyModel _partyModel;

        protected override void Subscribe()
        {
            _partyModel = ServiceResolver.Resolve<PlayerPartyModel>();
            Refresh();
        }

        protected override void Unsubscribe() { }

        /// <summary>
        /// Refreshes all UI slots to match the current party list.
        /// </summary>
        public void Refresh()
        {
            foreach (Transform child in slotParent)
                Destroy(child.gameObject);

            foreach (var monster in _partyModel.Party)
            {
                var slot = Instantiate(slotPrefab, slotParent);
                var label = slot.GetComponentInChildren<TextMeshProUGUI>();
                var image = slot.GetComponentInChildren<Image>();

                label.text = monster.name;
                image.color = Color.white;
                // TODO assign sprites for different monsters
            }
        }
    }
}