using IPA.Utilities;
using UnityEngine;

namespace CountersPlus.Utils
{
    /// <summary>
    /// Class that contains various <see cref="FieldAccessor{T, U}"/>s that see use within Counters+.
    /// </summary>
    static class Accessors
    {
        public static FieldAccessor<CoreGameHUDController, GameObject>.Accessor EnergyPanelGO = FieldAccessor<CoreGameHUDController, GameObject>.GetAccessor("_energyPanelGO");
    }
}
