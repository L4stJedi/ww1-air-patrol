using System;
using SavedVariables.Planes;
using UnityEngine;

namespace Scenes.Menu.MenuScripts
{
    public class EnhanceUIGetCurrentPlaneShowcase : MonoBehaviour
    {
        [NonSerialized] public ListOfPlanes.Planes plane;

        public void GetCurrentPlaneShowcaseEnhanceData(ListOfPlanes.Planes newPlane)
        {
            plane = newPlane;
        }
    }
}
