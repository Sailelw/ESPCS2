using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace ESPCS2
{
    public class Entity
    {
        public List<Vector3> bones {  get; set; }

        public List<Vector2> bones2d { get; set; }
        public string name { get; set; }
        public Vector3 position { get; set; }
        public Vector3 viewOffset { get; set; }
        public Vector2 position2D { get; set; }
        public Vector2 viewPosition2D { get; set; }
        public int team {  get; set; }
        public IntPtr pawnAddress { get; set; }
        public IntPtr controllerAdress {  get; set; }
        public Vector3 origin { get; set; }
        public Vector3 view { get; set; }
        public int health { get; set; }
        public int lifestate { get; set; }
        public float distance { get; set; }
        public Vector3 head { get; set; }
        public Vector2 head2d { get; set; }

        public int armor { get; set; }

        public short currentWeaponIndex { get; set; }

        public string currentWeaponName { get; set; }
        public float pixelDistance { get; set; }

        public int recoil { get; set; }
    }
    public enum BoneIds
    {
        Waist = 0,
        Neck = 5,
        Head = 6,
        ShoulderLeft = 8,
        ForeLeft = 9,
        HandLeft = 11,
        ShoulderRight = 13,
        ForeRight = 14,
        HandRight = 16,
        KneeLeft = 23,
        FeetLeft = 24,
        KneeRight = 26,
        FeetRight = 27
    }

    public enum Weapon
    {
        Deagle = 1,
        Elite = 2,
        Fiveseven = 3,
        Glock = 4,
        Ak47 = 7,
        Aug = 8,
        Awp = 9,
        Famas = 10,
        G3Sgl = 11,
        M249 = 14,
        Mac10 = 17,
        P90 = 19,
        Ump45 = 24,
        Xm1014 = 25,
        Bizon = 26,
        Mag7 = 27,
        Negev = 28,
        Sawedoff = 29,
        Tec9 = 30,
        Zeus = 31,
        P2000 = 32,
        Mp7 = 33,
        Mp9 = 34,
        Nova = 35,
        P250 = 36,
        Scar20 = 38,
        Sg556 = 39,
        Ssg08 = 40,
        CtKnife = 42,
        Flashbang = 43,
        Hegrenade = 44,
        Smokegrenade = 45,
        Molotov = 46,
        Decoy = 47,
        Incgrenade = 48,
        C4 = 49,
        M4A4 = 16,
        UspS = 61,
        M4A1Silencer = 60,
        Cz75A = 63,
        Revolver = 64,
        TKnife = 59

    }
}
