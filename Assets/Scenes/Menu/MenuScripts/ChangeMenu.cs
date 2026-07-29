using System;
using UnityEngine;

namespace Scenes.Menu.MenuScripts
{
    public class ChangeMenu : MonoBehaviour
    {
        [SerializeField] private Canvas mainMenuCanvas;
    
        [SerializeField] private Canvas hangarCanvas;
        [SerializeField] private Canvas settingsCanvas;
        [SerializeField] private Canvas campaignsCanvas;

        private HangarTabsHandling _hangarTabsHandling; 
        

        public enum Canvases
        {
            MainMenu,
            Hangar,
            Settings,
            Campaigns
        }

        public Canvases newMenu = Canvases.MainMenu;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            hangarCanvas.enabled = false;
            settingsCanvas.enabled = false;
            campaignsCanvas.enabled = false;

            mainMenuCanvas.enabled = true;

            _hangarTabsHandling = FindAnyObjectByType<HangarTabsHandling>();
        }

        public void SwapCanvas()
        {
            switch (newMenu)
            {
                case Canvases.MainMenu:
                    hangarCanvas.enabled = false;
                    settingsCanvas.enabled = false;
                    campaignsCanvas.enabled = false;

                    mainMenuCanvas.enabled = true;
                    break;
                
                case Canvases.Hangar:
                    mainMenuCanvas.enabled = false;
                    settingsCanvas.enabled = false;
                    campaignsCanvas.enabled = false;

                    hangarCanvas.enabled = true;
                    _hangarTabsHandling.SwappedMenuToHangar();
                    break;
                
                case Canvases.Settings:
                    hangarCanvas.enabled = false;
                    mainMenuCanvas.enabled = false;
                    campaignsCanvas.enabled = false;

                    settingsCanvas.enabled = true;
                    break;
                
                case Canvases.Campaigns:
                    mainMenuCanvas.enabled = false;
                    hangarCanvas.enabled = false;
                    settingsCanvas.enabled = false;
                    
                    campaignsCanvas.enabled = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newMenu), newMenu, null);
            }
        }


    }
}
