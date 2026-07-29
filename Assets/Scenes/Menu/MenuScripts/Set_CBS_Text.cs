using UnityEngine;
using TMPro;
using SavedVariables;

namespace Scenes.Menu.MenuScripts
{
    public class SetCbsText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI cbs;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            cbs.text = "Current Best Score: " + BestScore.CurrentBestScore;
        }
        
    }
}
