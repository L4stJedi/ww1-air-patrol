using System;
using SavedVariables.Settings;
using CandyCoded.HapticFeedback;

public enum HapticType
{
    Light,
    Medium,
    Heavy
}

public class HapticWrapper
{


    public static void Feedback(HapticType hapticType)
    {
        if (!SavedSettings.IsHapticEnabled) return;

            switch (hapticType)
        {
            case HapticType.Light: HapticFeedback.LightFeedback(); break;
            case HapticType.Medium: HapticFeedback.MediumFeedback(); break;
            case HapticType.Heavy: HapticFeedback.HeavyFeedback();  break;
            default:
                throw new ArgumentOutOfRangeException(nameof(hapticType), hapticType, null);
        }
    }
}
