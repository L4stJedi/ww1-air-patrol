using System;

namespace SavedVariables.Planes
{
    public static class ListOfPlanes
    {
        public enum Planes
        {
            // Keep these values explicit: they are persisted in PlayerPrefs.
            // Germany
            FokkerEIII = 0,
            AlbatrosDV = 1,
            FokkerDRI = 2,
            FokkerDVII = 3,
            
            // Britain
            AircoDH1 = 4,
            AircoDH2 = 5,
            SopwithTriplane = 6,
            SopwithPup = 7,
            SopwithCamel = 8,
            AircoDH9 = 9
        }

        public enum PlaneNation
        {
            Germany,
            Britain,
            France,
            Italy,
            Russia,
            America
        }

        public static PlaneNation GetNation(Planes plane)
        {
            return plane switch
            {
                Planes.FokkerEIII or
                Planes.AlbatrosDV or
                Planes.FokkerDRI or
                Planes.FokkerDVII => PlaneNation.Germany,

                Planes.AircoDH1 or
                Planes.AircoDH2 or
                Planes.SopwithTriplane or
                Planes.SopwithPup or
                Planes.SopwithCamel or
                Planes.AircoDH9 => PlaneNation.Britain,

                _ => throw new ArgumentOutOfRangeException(nameof(plane), plane, null)
            };
        }

        public static string GetDisplayName(Planes plane)
        {
            return plane switch
            {
                Planes.FokkerEIII => "Fokker E.III",
                Planes.AlbatrosDV => "Albatros D.V",
                Planes.FokkerDRI => "Fokker Dr.I",
                Planes.FokkerDVII => "Fokker D.VII",
                Planes.AircoDH1 => "Airco D.H.1",
                Planes.AircoDH2 => "Airco D.H.2",
                Planes.SopwithTriplane => "Sopwith Triplane",
                Planes.SopwithPup => "Sopwith Pup",
                Planes.SopwithCamel => "Sopwith Camel",
                Planes.AircoDH9 => "Airco D.H.9",
                _ => throw new ArgumentOutOfRangeException(nameof(plane), plane, null)
            };
        }
    }
}
