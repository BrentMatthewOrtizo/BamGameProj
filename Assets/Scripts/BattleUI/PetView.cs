using System.Collections;
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

        private Pet _pet;
        private int _displayDamage;

        /// <summary>
        /// Initialize the view for this pet.
        /// </summary>
        /// <param name="pet">Back-end pet data.</param>
        /// <param name="damageToShow">Usually BattleManager.damagePerWin.</param>
        /// <param name="sprite">Optional sprite for the pet.</param>
        public void Setup(Pet pet, int damageToShow, Sprite sprite = null, bool isEnemy = false)
        {
            _pet = pet;
            _displayDamage = damageToShow;

            if (hpText) hpText.text = _pet.CurrentHP.ToString();
            if (damageText) damageText.text = _displayDamage.ToString();

            if (sprite && petImage) petImage.sprite = sprite;

            // Flip horizontally if this is an enemy
            var scale = transform.localScale;
            scale.x = isEnemy ? -1f : 1f;
            transform.localScale = scale;
        }

        public void UpdateHp()
        {
            if (_pet != null && hpText)
                hpText.text = _pet.CurrentHP.ToString();
        }

        public void ShowDead()
        {
            if (petImage) petImage.color = Color.gray;
        }

        public void SetHighlight(bool on)
        {
            if (!petImage) return;
            petImage.color = on ? new Color(1f, 1f, 0.6f, 1f) : Color.white;
        }

        /// <summary>Very simple nudge to imply an attack. Replace with tween/anim later.</summary>
        public void PlayAttackNudge(bool toRight)
        {
            var rt = (RectTransform)transform;
            rt.anchoredPosition += new Vector2(toRight ? 14f : -14f, 0f);
        }
        
        public void FlashHit()
        {
            if (!petImage) return;
            StartCoroutine(FlashCoroutine());
        }

        private IEnumerator FlashCoroutine()
        {
            Color original = petImage.color;
            petImage.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            petImage.color = original;
        }
        
        
        
        
    }
}