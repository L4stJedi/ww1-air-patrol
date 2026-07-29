using System;
using SavedVariables;
using UnityEngine;

namespace Scenes.Menu.MenuScripts
{
    public class NationTabs : MonoBehaviour
    {


        public enum Nations
        {
            Germany = 0,
            Britain,
            France,
            Italy,
            Russia,
            America
        }
    
        [SerializeField] private GameObject germany;
        [SerializeField] private GameObject britain;
        [SerializeField] private GameObject france;
        [SerializeField] private GameObject italy;
        [SerializeField] private GameObject russia;
        [SerializeField] private GameObject america;
        
    
    
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            SwapNationTab((Nations)CurrentNationTab.CurrentNationTabValue);
        }

        public void SwapNationTab(Nations newCurrentTab)
        {
            germany.SetActive(false);
            britain.SetActive(false);
            france.SetActive(false);
            italy.SetActive(false);
            russia.SetActive(false);
            america.SetActive(false);

            switch (newCurrentTab)
            {
                case Nations.Germany: 
                    germany.SetActive(true);
                    CurrentNationTab.CurrentNationTabValue = (int)Nations.Germany;
                    break;
                case Nations.Britain: 
                    britain.SetActive(true);
                    CurrentNationTab.CurrentNationTabValue = (int)Nations.Britain;
                    break;
                case Nations.France: 
                    france.SetActive(true);
                    CurrentNationTab.CurrentNationTabValue = (int)Nations.France;
                    break;
                case Nations.Italy: 
                    italy.SetActive(true);
                    CurrentNationTab.CurrentNationTabValue = (int)Nations.Italy;
                    break;
                case Nations.Russia: 
                    russia.SetActive(true); 
                    CurrentNationTab.CurrentNationTabValue = (int)Nations.Russia;
                    break;
                case Nations.America: 
                    america.SetActive(true);
                    CurrentNationTab.CurrentNationTabValue = (int)Nations.America;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newCurrentTab), newCurrentTab, null);
            }
            
            
        }
    }
}
