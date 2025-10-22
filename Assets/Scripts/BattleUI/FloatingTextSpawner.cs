using TMPro;
using UnityEngine;

namespace AutoBattler
{
    public class FloatingTextSpawner : MonoBehaviour
    {
        [SerializeField] private FloatingText floatingTextPrefab;
        [SerializeField] private RectTransform layer;

        public void Spawn(RectTransform over, string text, Color color)
        {
            if (!floatingTextPrefab || !layer || !over) return;
            var ft = Instantiate(floatingTextPrefab, layer);
            var world = over.TransformPoint(Vector3.zero);
            Vector2 screenPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                layer, RectTransformUtility.WorldToScreenPoint(null, world), null, out screenPos);
            var r = ft.GetComponent<RectTransform>();
            r.anchoredPosition = screenPos + new Vector2(0, 32f);
            ft.Play(text, color);
        }
    }
}