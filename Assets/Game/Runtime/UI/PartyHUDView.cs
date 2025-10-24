using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.com.game399.shared.Models;

namespace Game.Runtime.UI
{
    public class PartyHUDView : ObserverMonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Transform slotParent;
        [SerializeField] private GameObject slotPrefab;

        [Header("Monster Sprites")]
        [SerializeField] private Sprite foxSprite;
        [SerializeField] private Sprite camelSprite;
        [SerializeField] private Sprite chimeraSprite;
        [SerializeField] private Sprite defaultSprite;

        private PlayerPartyModel _partyModel;

        protected override void Subscribe()
        {
            _partyModel = ServiceResolver.Resolve<PlayerPartyModel>();
            Refresh();
        }

        protected override void Unsubscribe() { }

        public void Refresh()
        {
            // Clear existing
            foreach (Transform child in slotParent)
                Destroy(child.gameObject);

            // Create slot for each monster
            foreach (var monster in _partyModel.Party)
            {
                var slot = Instantiate(slotPrefab, slotParent);

                // Get references
                var label = slot.GetComponentInChildren<TextMeshProUGUI>();
                var image = slot.GetComponentInChildren<Image>();

                // Apply correct sprite
                image.sprite = GetSpriteForMonster(monster.name);
                image.color = Color.white;

                // Set label
                label.text = monster.name;
            }
        }

        private Sprite GetSpriteForMonster(string name)
        {
            name = name.ToLower();
            if (name.Contains("fox")) return foxSprite;
            if (name.Contains("camel")) return camelSprite;
            if (name.Contains("chimera")) return chimeraSprite;
            return defaultSprite;
        }
    }
}