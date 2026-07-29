using UnityEngine;

namespace Scenes.Menu.MenuScripts
{
    public class NationButton : MonoBehaviour
    {
        [SerializeField] private NationTabs nationTabs;
        [SerializeField] private NationTabs.Nations nation = NationTabs.Nations.Germany;


        public void CallSwapNations()
        {
            nationTabs.SwapNationTab(nation);
        }
    }
}
