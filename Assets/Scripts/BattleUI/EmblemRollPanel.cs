using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AutoBattler
{
    public class EmblemRollPanel : MonoBehaviour
    {
        [Header("Root / Visibility")]
        [SerializeField] private CanvasGroup group;

        [Header("Left (Player)")]
        [SerializeField] private Transform leftIconRow;       // parent containing icon Images
        [SerializeField] private Image leftHighlight;         // frame/outline image

        [Header("Right (Enemy)")]
        [SerializeField] private Transform rightIconRow;
        [SerializeField] private Image rightHighlight;

        [Header("Icon Sprites")]
        [SerializeField] private Sprite swordSprite;
        [SerializeField] private Sprite shieldSprite;
        [SerializeField] private Sprite magicSprite;

        [Header("Timing")]
        [SerializeField] private float spinTime = 0.5f;
        [SerializeField] private float flashTime = 0.25f;

        readonly List<Image> _leftIcons = new();
        readonly List<Image> _rightIcons = new();

        void Awake()
        {
            CacheIcons(leftIconRow, _leftIcons);
            CacheIcons(rightIconRow, _rightIcons);
            HideImmediate();
        }

        void CacheIcons(Transform row, List<Image> list)
        {
            list.Clear();
            if (!row) return;
            foreach (Transform child in row)
            {
                if (child.TryGetComponent<Image>(out var img))
                    list.Add(img);
            }
        }

        Sprite SpriteFor(Emblem e) => e switch
        {
            Emblem.Sword  => swordSprite,
            Emblem.Shield => shieldSprite,
            Emblem.Magic  => magicSprite,
            _ => swordSprite
        };

        public void ShowSpin(IList<Emblem> left, Emblem leftPick, IList<Emblem> right, Emblem rightPick, int winnerIndex)
        {
            StopAllCoroutines();
            StartCoroutine(SpinRoutine(left, leftPick, right, rightPick, winnerIndex));
        }

        IEnumerator SpinRoutine(IList<Emblem> left, Emblem leftPick, IList<Emblem> right, Emblem rightPick, int winnerIndex)
        {
            ApplyIcons(_leftIcons, left);
            ApplyIcons(_rightIcons, right);
            Show();

            // simple “spin” by flashing each icon in sequence - may change later????
            float t = 0f;
            int li = 0, ri = 0;
            while (t < spinTime)
            {
                t += Time.unscaledDeltaTime;
                li = (li + 1) % Mathf.Max(1, _leftIcons.Count);
                ri = (ri + 1) % Mathf.Max(1, _rightIcons.Count);
                HighlightIndex(_leftIcons, li);
                HighlightIndex(_rightIcons, ri);
                yield return null;
            }

            // land on picked
            int leftIndex = IndexOf(left, leftPick);
            int rightIndex = IndexOf(right, rightPick);
            HighlightIndex(_leftIcons, Mathf.Max(0, leftIndex));
            HighlightIndex(_rightIcons, Mathf.Max(0, rightIndex));

            // flash the winner (0 left / 1 right / -1 tie)
            if (winnerIndex == 0) yield return FlashOutline(leftHighlight);
            else if (winnerIndex == 1) yield return FlashOutline(rightHighlight);

            Hide();
        }

        int IndexOf(IList<Emblem> list, Emblem e)
        {
            if (list == null) return -1;
            for (int i = 0; i < list.Count; i++)
                if (list[i] == e) return i;
            return -1;
        }

        void ApplyIcons(List<Image> targets, IList<Emblem> src)
        {
            for (int i = 0; i < targets.Count; i++)
            {
                if (src != null && i < src.Count)
                {
                    targets[i].enabled = true;
                    targets[i].sprite  = SpriteFor(src[i]);
                    targets[i].color   = Color.white;
                }
                else
                {
                    targets[i].enabled = false;
                }
            }
        }

        void HighlightIndex(List<Image> icons, int index)
        {
            for (int i = 0; i < icons.Count; i++)
                icons[i].color = (i == index) ? Color.yellow : Color.white;
        }

        IEnumerator FlashOutline(Image outline)
        {
            if (!outline) yield break;
            outline.enabled = true;
            float t = 0f;
            while (t < flashTime)
            {
                t += Time.unscaledDeltaTime;
                yield return null;
            }
            outline.enabled = false;
        }

        void Show()
        {
            if (group)
            {
                group.alpha = 1;
                group.blocksRaycasts = true;
                group.interactable = true;
            }
            gameObject.SetActive(true);
        }

        public void HideImmediate()
        {
            if (group)
            {
                group.alpha = 0;
                group.blocksRaycasts = false;
                group.interactable = false;
            }
            gameObject.SetActive(false);
        }

        void Hide()
        {
            HideImmediate();
        }
    }
}