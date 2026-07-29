using System;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes.Menu.MenuScripts
{
    public class UILineConnector : MonoBehaviour
    {

        [SerializeField] private GameObject linePrefab;
        [SerializeField] private RectTransform pointB;
        [SerializeField] private Transform linesContainer;
        private SkillTreeNode _skillTreeNode; 


        private void Start()
        {
            _skillTreeNode = GetComponent<SkillTreeNode>();
            
            var pointA = GetComponent<RectTransform>();
            if (linePrefab == null || pointB == null) return;

            
            var lineObject = Instantiate(linePrefab, linesContainer);
            var lineRect = lineObject.GetComponent<RectTransform>();

            var worldPosA = pointA.position;
            var worldPosB = pointB.position;

            var startPos = linesContainer.InverseTransformPoint(worldPosA);
            var endPos = linesContainer.InverseTransformPoint(worldPosB);
            
            lineRect.anchoredPosition = startPos;

            var dir = endPos - startPos;
            var distance = dir.magnitude;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            lineRect.sizeDelta = new Vector2(distance, 10f);
        
            lineRect.localRotation = Quaternion.Euler(0, 0, angle);


            var image = lineObject.GetComponent<Image>();
            switch (_skillTreeNode.type)
            {
                case SkillTreeNode.Type.Acrobat:
                    image.color = new Color(0.78f, 0.4f, 0.07f);
                    break;
                case SkillTreeNode.Type.Tactician:
                    image.color = new Color(0.1f, 0.12f, 0.69f);
                    break;
                case SkillTreeNode.Type.Mechanic:
                    image.color = new Color(0f, 0.35f, 0.1f);
                    break;
                case SkillTreeNode.Type.Special:
                    image.color = new Color(0.24f, 0.01f, 0.35f);
                    break;
            }
        }

    }
}
