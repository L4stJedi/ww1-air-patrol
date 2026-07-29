using PlayerPlaneScripts;
using TMPro;
using UnityEngine;

namespace UI
{
    public class XpUI : MonoBehaviour
    {
        private TextMeshProUGUI _xpText;

        private Xp _xpRef;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            _xpText = GetComponent<TextMeshProUGUI>();
            _xpRef = FindAnyObjectByType<Xp>();
        }

        // Update is called once per frame
        private void Update()
        {
            _xpText.text = ("XP: " + _xpRef.xP + " (" + _xpRef.killCount + "/" + _xpRef.killsForXp +")");

        }
    }
}
