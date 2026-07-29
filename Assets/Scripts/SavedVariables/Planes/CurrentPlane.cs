using UnityEngine;

namespace SavedVariables.Planes
{
    public static class CurrentPlane
    {
        private const string EquippedPlaneKey = "EquippedPlaneKey";

        public static ListOfPlanes.Planes EquippedPlane
        {
            get => (ListOfPlanes.Planes)PlayerPrefs.GetInt(
                EquippedPlaneKey,
                (int)ListOfPlanes.Planes.AlbatrosDV);
            set
            {
                PlayerPrefs.SetInt(EquippedPlaneKey, (int)value);
                PlayerPrefs.Save();
            }
        }
    }
}
