using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AutoBattler
{
    public class PetView : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private TMP_Text nameLabel;
        [SerializeField] private TMP_Text hpLabel;
        [SerializeField] private Image bodyImage;

        [Header("Visuals")]
        [SerializeField] private Color deadColor = Color.gray;
        [SerializeField] private float attackNudge = 28f;
        [SerializeField] private float attackTime = 0.12f;

        public Pet Pet { get; private set; }

        Color _originalColor;
        RectTransform _rect;

        void Awake()
        {
            _rect = GetComponent<RectTransform>();
            if (bodyImage != null) _originalColor = bodyImage.color;
        }

        public void Setup(Pet pet)
        {
            Pet = pet;
            if (nameLabel) nameLabel.text = pet.Name;
            UpdateHp();
            SetHighlight(false);
        }

        public void UpdateHp()
        {
            if (hpLabel) hpLabel.text = $"{Pet.CurrentHP}/{Pet.MaxHP}";
        }

        public void SetHighlight(bool on)
        {
            if (!bodyImage) return;
            bodyImage.color = on ? new Color(1f, 1f, 0.6f) : _originalColor;
        }

        public void ShowDead()
        {
            if (bodyImage) bodyImage.color = deadColor;
        }

        public void PlayAttackNudge(bool toRight)
        {
            if (!_rect) return;
            StopAllCoroutines();
            StartCoroutine(Nudge(toRight ? +attackNudge : -attackNudge));
        }

        System.Collections.IEnumerator Nudge(float dx)
        {
            Vector2 start = _rect.anchoredPosition;
            Vector2 mid = start + new Vector2(dx, 0);
            float t = 0f;
            while (t < attackTime)
            {
                t += Time.unscaledDeltaTime;
                _rect.anchoredPosition = Vector2.Lerp(start, mid, t / attackTime);
                yield return null;
            }
            t = 0f;
            while (t < attackTime)
            {
                t += Time.unscaledDeltaTime;
                _rect.anchoredPosition = Vector2.Lerp(mid, start, t / attackTime);
                yield return null;
            }
            _rect.anchoredPosition = start;
        }
    }
}