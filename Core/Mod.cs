using System;
using ICities;

namespace CSL.RoadExtensions
{
    public partial class Mod : IUserMod
    {
        private const UInt64 WORKSHOP_ID = 478820060;

        public const string REX_OBJECT_NAME = "Road Extensions";

        public const string ROAD_NETCOLLECTION = "Road";
        public const string NEWROADS_NETCOLLECTION = "NewRoad";

        public string Name
        {
            get { return "Road Extensions"; }
        }

        public string Description
        {
            get { return "An addition of highways and roads"; }
        }
    }
}
