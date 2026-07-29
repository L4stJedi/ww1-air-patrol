using UnityEngine;

namespace EnemyPlaneScripts
{
    public class BossVisualMarker : MonoBehaviour
    {
        [SerializeField] private Color bossTint = new Color(1f, 0.55f, 0.25f, 1f);
        [SerializeField] private float scaleMultiplier = 1.25f;

        private Vector3 _baseScale;
        private SpriteRenderer[] _renderers;

        private void Awake()
        {
            _baseScale = transform.localScale;
            _renderers = GetComponentsInChildren<SpriteRenderer>(true);
        }

        private void OnEnable()
        {
            transform.localScale = _baseScale * scaleMultiplier;

            if (_renderers == null)
                return;

            foreach (var renderer in _renderers)
                renderer.color = bossTint;
        }
    }
}
