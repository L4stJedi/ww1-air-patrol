using UnityEngine;
using UnityEngine.SceneManagement;
using SavedVariables;

namespace PlayerPlaneScripts
{
    public class PlayerDeath : MonoBehaviour
    {
        private Xp _xpRef;

        private void Awake()
        {
            _xpRef = GetComponent<Xp>();
        }

        //called in PlayerHP
        public void Die()
        {
            BankedXp.AddXpToBank(_xpRef.xP);
            SceneManager.LoadScene(sceneName: "Scenes/Menu/Menu");
        }
        
    }
}
