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

        [Header("Sprites")]
        [SerializeField] private Sprite aliveSprite;
        [SerializeField] private Sprite deadSprite;
        [SerializeField] private Sprite swordSprite;
        [SerializeField] private Sprite shieldSprite;
        [SerializeField] private Sprite magicSprite;

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip[] hitSounds;

        private Pet _pet;
        private int _displayDamage;
        private bool _isEnemy;
        private Coroutine _emblemRoutine;
        private bool _spinning;

        public void Setup(Pet pet, int damageToShow, Sprite sprite = null, bool isEnemy = false)
        {
            _pet = pet;
            _displayDamage = damageToShow;
            _isEnemy = isEnemy;

            if (hpText) hpText.text = Mathf.Max(0, _pet.CurrentHP).ToString();
            if (damageText) damageText.text = _displayDamage.ToString();

            if (petImage)
            {
                if (sprite) petImage.sprite = sprite;
                var s = petImage.rectTransform.localScale;
                s.x = isEnemy ? -Mathf.Abs(s.x) : Mathf.Abs(s.x);
                petImage.rectTransform.localScale = s;
                petImage.color = Color.white;
            }

            if (emblemImage) emblemImage.enabled = false;
            if (!_pet.IsAlive) ShowDeadVisual();
        }

        // --- Emblem Rolling ---
        public void BeginEmblemRoll(float intervalSeconds)
        {
            if (!emblemImage || _pet == null || _pet.Emblems == null || _pet.Emblems.Count == 0)
                return;

            emblemImage.enabled = true;
            if (_emblemRoutine != null) StopCoroutine(_emblemRoutine);
            _spinning = true;
            _emblemRoutine = StartCoroutine(SpinEmblems(intervalSeconds));
        }

        public void EndEmblemRoll(Emblem finalEmblem)
        {
            _spinning = false;
            if (_emblemRoutine != null)
            {
                StopCoroutine(_emblemRoutine);
                _emblemRoutine = null;
            }

            if (emblemImage)
            {
                emblemImage.enabled = true;
                emblemImage.sprite = GetEmblemSprite(finalEmblem);
                StartCoroutine(PopScale(emblemImage.rectTransform));
            }
        }

        private IEnumerator SpinEmblems(float interval)
        {
            var pool = _pet.Emblems;
            int i = 0;
            while (_spinning)
            {
                emblemImage.sprite = GetEmblemSprite(pool[i % pool.Count]);
                i++;
                yield return new WaitForSeconds(interval);
            }
        }

        private IEnumerator PopScale(RectTransform target)
        {
            Vector3 original = target.localScale;
            Vector3 enlarged = original * 1.3f;
            float speed = 8f;

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * speed;
                target.localScale = Vector3.Lerp(original, enlarged, t);
                yield return null;
            }

            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * speed;
                target.localScale = Vector3.Lerp(enlarged, original, t);
                yield return null;
            }
        }

        private Sprite GetEmblemSprite(Emblem e) => e switch
        {
            Emblem.Sword => swordSprite,
            Emblem.Shield => shieldSprite,
            Emblem.Magic => magicSprite,
            _ => null
        };

        // --- Damage & Death Visuals ---
        public void UpdateHp()
        {
            if (_pet != null && hpText)
                hpText.text = Mathf.Max(0, _pet.CurrentHP).ToString();
        }

        public void FlashHit()
        {
            if (!petImage) return;
            PlayRandomHit();
            StartCoroutine(FlashCoroutine());
        }

        public void PlayDeathHit()
        {
            PlayRandomHit();
            StartCoroutine(FlashCoroutine());
        }

        private void PlayRandomHit()
        {
            if (audioSource != null && hitSounds != null && hitSounds.Length > 0)
            {
                var clip = hitSounds[Random.Range(0, hitSounds.Length)];
                audioSource.pitch = Random.Range(0.95f, 1.05f); // subtle variation
                audioSource.PlayOneShot(clip);
            }
        }

        private IEnumerator FlashCoroutine()
        {
            var c0 = petImage.color;
            petImage.color = Color.red;
            yield return new WaitForSeconds(0.15f);
            petImage.color = c0;
        }

        public void ShowDeadVisual()
        {
            if (!petImage) return;
            if (deadSprite) petImage.sprite = deadSprite;
            petImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);

            var rt = petImage.rectTransform;
            var rot = rt.localEulerAngles;
            rot.x = 180f;
            rt.localEulerAngles = rot;

            if (emblemImage) emblemImage.enabled = false;
        }

        public void HideEmblem()
        {
            if (emblemImage) emblemImage.enabled = false;
        }
    }
}