using UnityEngine;
using Scenes.Menu.MenuScripts;

namespace SavedVariables
{
    public static class BankedXp
    {
        private const string BankedXpKey = "BankedXpKey";
        
        
        public static int BankedXpValue
        {
            get => PlayerPrefs.GetInt(BankedXpKey, 0);
            private set
            {
                PlayerPrefs.SetInt(BankedXpKey, value);
                PlayerPrefs.Save();
            }
        }

        public static void AddXpToBank(int xp)
        {
            if (xp <= 0) return;

            BankedXpValue += xp;
        }

        public static bool SpendBankedXp(int cost)
        {
            if (cost > BankedXpValue || cost <= 0)
            {
                return false;
            }
            else
            {
                BankedXpValue -= cost;
                return true;
            }
        }
    }
}
