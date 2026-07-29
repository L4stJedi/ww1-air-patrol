using PlayerPlaneScripts;
using UnityEngine;
using UnityEngine.UI;


namespace UI
{
    public class HpUI : MonoBehaviour
    {
        public bool isShield; 
        
        private bool _damaged = true;
        private Image _img;
        private PlayerHp _hpRef;
        private int _numberInRow;
        

        private void Start()
        {
            _img = GetComponent<Image>();
            _hpRef = FindAnyObjectByType<PlayerHp>();
        }

        private void Update()
        {
            
            
            if (_hpRef.Hp <= _numberInRow && !_damaged)
            {
                DamageEnemyUI();
            }
        }

        private void DamageEnemyUI()
        {
                _img.color = Color.black;
                _damaged = true;
        }
        
    }
}
