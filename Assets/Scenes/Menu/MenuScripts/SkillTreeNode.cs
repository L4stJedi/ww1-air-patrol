using System;
using SavedVariables.PilotUpgrades;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes.Menu.MenuScripts
{
    public class SkillTreeNode : MonoBehaviour
    {

        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI tmp;
        public Type type;
        private Button _btn;
        private Outline _outline;
        public enum Type
        {
            Acrobat,
            Tactician,
            Mechanic,
            Special
        }
    
        void Awake()
        {
            _outline = GetComponent<Outline>();
            _btn = GetComponent<Button>();
            
            switch (type)
            {
                case Type.Acrobat:
                    SetToAcrobat();
                    break;
                case Type.Tactician:
                    SetToTactician();
                    break;
                case Type.Mechanic:
                    SetToMechanic();
                    break;
                case Type.Special: 
                    SetToSpecial();
                    break;
                    
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Start()
        {
            var details = GetComponent<SendToDescriptionBox>();
            
            if (details == null) return;
            var profile = PilotSaveSystem.LoadPilotData(CurrentPilot.PilotID);

            if (profile.skillData.unlockedSkillsIds.Contains(details.ownedSkill.ToString()))
            {
                SetToUnlocked();
            }
        }

        private void SetToAcrobat()
        {
            _outline.effectColor = new Color(0.78f, 0.4f, 0.07f);
            tmp.color = Color.black;
        }

        private void SetToTactician()
        {
            _outline.effectColor = new Color(0.1f, 0.12f, 0.69f);
            tmp.color = Color.black;
        }

        private void SetToMechanic()
        {
            _outline.effectColor = new Color(0f, 0.35f, 0.1f);
            tmp.color = Color.black;
        }
        
        private void SetToSpecial()
        {
            _outline.effectColor = new Color(0.24f, 0.01f, 0.35f);
            tmp.color = new Color(1f, 0.76f, 0f);
        }

        public void SetToUnlocked()
        {

            _btn.interactable = false;
            _outline.effectColor = Color.black;
            tmp.color = Color.white;
            switch (type)
            {
                case Type.Acrobat:
                    _btn.image.color = new Color(0.78f, 0.4f, 0.07f);
                    break;
                case Type.Tactician:
                    _btn.image.color = new Color(0.1f, 0.12f, 0.69f);
                    break;
                case Type.Mechanic:
                    _btn.image.color = new Color(0f, 0.35f, 0.1f);;
                    break;
                case Type.Special: 
                    _btn.image.color = new Color(0.24f, 0.01f, 0.35f);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
