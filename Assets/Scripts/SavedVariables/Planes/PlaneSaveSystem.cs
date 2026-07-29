using UnityEngine;

namespace SavedVariables.Planes
   
{
    public static class PlaneSaveSystem
    {
        private static string GetSaveKey(ListOfPlanes.Planes plane)
        {
            return $"Plane_{(int)plane}_Properties";
        }

        public static void SavePilotData(ListOfPlanes.Planes plane, PlaneEnhanceSaveData properties)
        {
            var json = JsonUtility.ToJson(properties);
            
            PlayerPrefs.SetString(GetSaveKey(plane), json);
            PlayerPrefs.Save();
        }

        public static PlaneEnhanceSaveData LoadPlaneProperties(ListOfPlanes.Planes plane)
        {
            if (!PlayerPrefs.HasKey(GetSaveKey(plane)))
            {

                PlaneEnhanceSaveData defaultData = new PlaneEnhanceSaveData();

                if (plane is ListOfPlanes.Planes.AlbatrosDV or ListOfPlanes.Planes.AircoDH2)
                {
                    defaultData.isUnlocked = true;
                }
                
                return defaultData;
            }


            
            var json = PlayerPrefs.GetString(GetSaveKey(plane));
            
            return JsonUtility.FromJson<PlaneEnhanceSaveData>(json);
        }
    }
}
