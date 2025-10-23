using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AutoBattler
{
    public class PetView : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image petImage;
        [SerializeField] private TextMeshProUGUI hpText;
        [SerializeField] private TextMeshProUGUI damageText;
        [SerializeField] private Image emblemImage;

        [Header("Sprites (optional)")]
        [SerializeField] private Sprite aliveSprite;
        [SerializeField] private Sprite deadSprite;
        [SerializeField] private Sprite swordSprite;
        [SerializeField] private Sprite shieldSprite;
        [SerializeField] private Sprite magicSprite;

        private Pet _pet;
        private int _displayDamage;
        private bool _isEnemy;
        private Coroutine _emblemRoutine;

        // --------------------  SETUP  --------------------
        public void Setup(Pet pet, int damageToShow, Sprite sprite = null, bool isEnemy = false)
        {
            _pet = pet;
            _displayDamage = damageToShow;
            _isEnemy = isEnemy;

            // Update text
            if (hpText) hpText.text = Mathf.Max(0, _pet.CurrentHP).ToString();
            if (damageText) damageText.text = _displayDamage.ToString();

            // Configure sprite + facing direction
            if (petImage)
            {
                if (sprite) petImage.sprite = sprite;
                var s = petImage.rectTransform.localScale;
                s.x = isEnemy ? -Mathf.Abs(s.x) : Mathf.Abs(s.x);
                s.y = Mathf.Abs(s.y);
                petImage.rectTransform.localScale = s;
                petImage.color = Color.white;
            }

            // Hide emblem initially
            if (emblemImage)
                emblemImage.enabled = false;

            // NEW: if pet already dead (like after rebuild), immediately gray it out
            if (!_pet.IsAlive)
                ShowDeadVisual();
        }

        // --------------------  EMBLEM LOGIC  --------------------
        public void ShowEmblem(Emblem emblem)
        {
            if (!emblemImage) return;
            emblemImage.enabled = true;
            emblemImage.sprite = GetEmblemSprite(emblem);
        }

        public void HideEmblem()
        {
            if (emblemImage)
                emblemImage.enabled = false;
        }

        // This now cycles ONLY through the monster’s *actual* emblems
        public void PlayEmblemRoll()
        {
            if (_emblemRoutine != null)
                StopCoroutine(_emblemRoutine);
            _emblemRoutine = StartCoroutine(EmblemSlotRoutine());
        }

        private IEnumerator EmblemSlotRoutine()
        {
            if (!emblemImage || _pet == null || _pet.Emblems == null || _pet.Emblems.Count == 0)
                yield break;

            emblemImage.enabled = true;

            // Build the actual pool of sprites from THIS pet’s equipped emblems
            List<Sprite> emblemSprites = new List<Sprite>();
            foreach (var emblem in _pet.Emblems)
                emblemSprites.Add(GetEmblemSprite(emblem));

            float duration = 0.6f;
            float elapsed = 0f;
            float interval = 0.05f;

            while (elapsed < duration)
            {
                // Pick from this pet’s own emblems, not all possible ones
                emblemImage.sprite = emblemSprites[Random.Range(0, emblemSprites.Count)];
                elapsed += interval;
                yield return new WaitForSeconds(interval);
            }
        }

        private Sprite GetEmblemSprite(Emblem e)
        {
            return e switch
            {
                Emblem.Sword => swordSprite,
                Emblem.Shield => shieldSprite,
                Emblem.Magic => magicSprite,
                _ => null
            };
        }

        // --------------------  HEALTH / DAMAGE  --------------------
        public void UpdateHp()
        {
            if (_pet != null && hpText)
                hpText.text = Mathf.Max(0, _pet.CurrentHP).ToString();
        }

        public void FlashHit()
        {
            if (!petImage) return;
            StartCoroutine(FlashCoroutine());
        }

        private IEnumerator FlashCoroutine()
        {
            var c0 = petImage.color;
            petImage.color = Color.red;
            yield return new WaitForSeconds(0.15f);
            petImage.color = c0;
        }

        // --------------------  DEATH VISUALS  --------------------
        public void ShowDeadVisual()
        {
            if (deadSprite && petImage)
                petImage.sprite = deadSprite;

            if (petImage)
                petImage.color = new Color(0.5f, 0.5f, 0.5f, 1f); // gray tint

            if (emblemImage)
                emblemImage.enabled = false;
        }
    }
}