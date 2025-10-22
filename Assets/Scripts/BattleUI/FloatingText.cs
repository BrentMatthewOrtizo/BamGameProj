using TMPro;
using UnityEngine;

namespace AutoBattler
{
    public class FloatingText : MonoBehaviour
    {
        [SerializeField] TMP_Text label;
        [SerializeField] float rise = 30f;
        [SerializeField] float lifetime = 0.6f;

        RectTransform _rect;
        Color _startColor;

        void Awake()
        {
            _rect = GetComponent<RectTransform>();
            if (label) _startColor = label.color;
        }

        public void Play(string text, Color color)
        {
            if (label) { label.text = text; label.color = color; }
            StopAllCoroutines();
            StartCoroutine(Run());
        }

        System.Collections.IEnumerator Run()
        {
            Vector2 start = _rect.anchoredPosition;
            Vector2 end = start + new Vector2(0, rise);

            float t = 0f;
            while (t < lifetime)
            {
                t += Time.unscaledDeltaTime;
                float k = t / lifetime;
                _rect.anchoredPosition = Vector2.Lerp(start, end, k);
                if (label) label.color = new Color(_startColor.r, _startColor.g, _startColor.b, 1f - k);
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}