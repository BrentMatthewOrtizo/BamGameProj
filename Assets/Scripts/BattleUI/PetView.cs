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

        [Header("Sprites (optional)")]
        [SerializeField] private Sprite aliveSprite;
        [SerializeField] private Sprite deadSprite;

        private Pet _pet;
        private int _displayDamage;
        private bool _isEnemy;
        
        public void Setup(Pet pet, int damageToShow, Sprite sprite = null, bool isEnemy = false)
        {
            _pet = pet;
            _displayDamage = damageToShow;
            _isEnemy = isEnemy;

            if (hpText)     hpText.text     = _pet.CurrentHP.ToString();
            if (damageText) damageText.text = _displayDamage.ToString();

            if (petImage)
            {
                if (sprite) petImage.sprite = sprite;

                var s = petImage.rectTransform.localScale;
                s.x = isEnemy ? Mathf.Abs(s.x) : -Mathf.Abs(s.x);
                s.y = Mathf.Abs(s.y);                              // keep upright
                petImage.rectTransform.localScale = s;

                petImage.color = Color.white;
            }
        }


        public void UpdateHp()
        {
            if (_pet != null && hpText) hpText.text = _pet.CurrentHP.ToString();
        }

        public void SetHighlight(bool on)
        {
            if (!petImage) return;
            petImage.color = on ? new Color(1f, 1f, 0.6f, 1f) : Color.white;
        }

        public void PlayAttackNudge(bool toRight) { /* intentionally empty */ }

        public void ShowDeadVisual()
        {
            if (deadSprite && petImage) petImage.sprite = deadSprite;
            if (petImage) petImage.color = new Color(0.6f, 0.6f, 0.6f, 1f);
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
    }
}