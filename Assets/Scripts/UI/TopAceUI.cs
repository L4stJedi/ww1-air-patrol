using TMPro;
using UnityEngine;
using SavedVariables;
using SavedVariables.PilotUpgrades;

namespace UI
{
    public class TopAceUI : MonoBehaviour
    {
        private TextMeshProUGUI _textRef;
        private PilotStatData _pilotStatData;
        
        void Start()
        {
            _pilotStatData = PilotSaveSystem.LoadPilotData(CurrentPilot.PilotID).statData;
            _textRef = GetComponent<TextMeshProUGUI>();
            UpdateTopAceText();
        }

       public void UpdateTopAceText()
        {
            _textRef.text = $"Top Ace: {_pilotStatData.pilotName} - {BestScore.CurrentBestScore}";
        }
    }
}
