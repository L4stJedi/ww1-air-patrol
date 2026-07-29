using UnityEngine;

namespace Scenes.Menu.MenuScripts
{
    public class EnhanceButton : MonoBehaviour
    {

        [SerializeField] private GameObject enhanceUI;


        public void TriggerEnhanceUIPopUp(bool open)
        {
            enhanceUI.SetActive(open);
        }
    }
}
