using SavedVariables.Planes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scenes.Menu.MenuScripts
{
    public class LoadLevel : MonoBehaviour
    {
        private enum ProjectScene
        {
            Menu,
            Settings,
            Hangar,
            Campaigns,
            TestLevel,
        }

        [SerializeField] private ProjectScene targetScene;
        [SerializeField] private bool equipPlaneBeforeLoad;
        [SerializeField] private ListOfPlanes.Planes planeToEquip = ListOfPlanes.Planes.AlbatrosDV;


        public void TriggerSceneLoad()
        {
            if (equipPlaneBeforeLoad)
            {
                CurrentPlane.EquippedPlane = planeToEquip;
            }

            switch (targetScene)
            {
                default:
                    SceneManager.LoadScene(sceneName: "MainMenu");
                    break;
                
                case ProjectScene.Menu:
                    SceneManager.LoadScene(sceneName: "MainMenu");
                    break;
                                
                case ProjectScene.Settings:
                    SceneManager.LoadScene(sceneName: "SettingsMenu");
                    break;
                                                            
                case ProjectScene.Hangar:
                    SceneManager.LoadScene(sceneName: "HangarMenu");
                    break;
                
                case ProjectScene.Campaigns:
                    SceneManager.LoadScene(sceneName: "CampaignsMenu");
                    break;

                case ProjectScene.TestLevel:
                    SceneManager.LoadScene(sceneName: "TestLevel");
                    break;
            }
        }

    }
}
