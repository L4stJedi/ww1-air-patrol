using System;
using SavedVariables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;


namespace Scenes.Menu.MenuScripts
{
    public class HangarTabsHandling : MonoBehaviour
    {
        [SerializeField] private GameObject pilotTab;
        [SerializeField] private GameObject planesTab;

        [SerializeField] private Button pilotButton;
        [SerializeField] private Button planesButton;
        [SerializeField] private GameObject descriptionBox;

        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private GameObject container;
        private RectTransform _containerRectTransform;

        
        private Tabs _currentTab = Tabs.Planes;


        private enum Tabs
        {
            Planes,
            Pilot
        }
    
        public void SwappedMenuToHangar()
        {
            _containerRectTransform = container.GetComponent<RectTransform>();
            TriggerSettingTabs(CurrentHangarTab.CurrentHangarTabValue);
        }


        
        // Hides non-active tabs, shows active tabs and calls buttons color change.
        public void TriggerSettingTabs(int tabNumber)
        {

            switch (tabNumber)
            {
                case 0:
                    _currentTab = Tabs.Planes;
                    CurrentHangarTab.CurrentHangarTabValue = 0;
                    break;
                case 1: 
                    _currentTab = Tabs.Pilot;
                    CurrentHangarTab.CurrentHangarTabValue = 1;
                    break;
            }

            switch (_currentTab)
            {
                case Tabs.Planes:
                    descriptionBox.SetActive(false);
                    pilotTab.SetActive(false);
                    SetButtonColor(pilotButton, false);
                    
                    
                    planesTab.SetActive(true);
                    SetButtonColor(planesButton, true);
                    
                    scrollRect.enabled = false;
                    var currentAnchoredPos = _containerRectTransform.anchoredPosition;
                    currentAnchoredPos.y = -1150;

                    _containerRectTransform.anchoredPosition = currentAnchoredPos;
                    
                    _currentTab = Tabs.Planes;
                    break;
            
                case Tabs.Pilot:
                    descriptionBox.SetActive(true);
                    planesTab.SetActive(false);
                    SetButtonColor(planesButton, false);
                    
                    pilotTab.SetActive(true);
                    SetButtonColor(pilotButton, true);

                    scrollRect.enabled = true;
                    _currentTab = Tabs.Pilot;
                    break;
            }
        }

        // Changes buttons color, first parameter requires button ref, second if tab is active
        private void SetButtonColor(Button button ,bool active)
        {
            var backgroundColor = active ? Color.white : Color.black;
            var textColor = active ? Color.black : Color.white;

            var buttonColors = ColorBlock.defaultColorBlock;
            
            buttonColors.normalColor = backgroundColor;
            buttonColors.highlightedColor = backgroundColor;

            if (button == null) return;
            
            button.colors = buttonColors;
            
            
            var text = button.GetComponentInChildren<TextMeshProUGUI>();
            if (text == null) return;
            text.color = textColor;
        }
        
    }
}
