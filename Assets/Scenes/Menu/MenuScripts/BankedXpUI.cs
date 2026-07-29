using System;
using PlayerPlaneScripts;
using SavedVariables;
using TMPro;
using UnityEngine;

public class BankedXpUI : MonoBehaviour
{
    private TextMeshProUGUI _text;
    private int currentXp;
    
    private void Start()
    {
        currentXp = BankedXp.BankedXpValue;
        _text = GetComponent<TextMeshProUGUI>();
        _text.text = $"XP: {BankedXp.BankedXpValue}";
    }

    public void Update()
    {
        if (currentXp == BankedXp.BankedXpValue) return;
        _text.text = $"XP: {BankedXp.BankedXpValue}";
        currentXp = BankedXp.BankedXpValue;
    }
}
