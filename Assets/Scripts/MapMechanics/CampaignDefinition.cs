using UnityEngine;

namespace MapMechanics
{
    [CreateAssetMenu(fileName = "CampaignDefinition", menuName = "WONML/Campaign Definition")]
    public class CampaignDefinition : ScriptableObject
    {
        public string campaignId = "campaign";
        public string displayName = "Campaign";
        public StageProfile[] stages;
        public bool isImplemented = true;

        public int StageCount => stages == null ? 0 : stages.Length;

        public StageProfile GetStage(int index)
        {
            if (stages == null || stages.Length == 0)
                return null;

            return stages[Mathf.Clamp(index, 0, stages.Length - 1)];
        }
    }
}
