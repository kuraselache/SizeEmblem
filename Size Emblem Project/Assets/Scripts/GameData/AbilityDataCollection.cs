using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Scripts.GameData
{
    [Serializable]
    public class AbilityDataCollection
    {
        public List<AbilityData> AbilityData;

        public bool LookUpID(string id, out AbilityData abilityData)
        {
            // Start assuming we can't find the data so set it to null
            abilityData = null;

            // If our collection is empty then we don't have anything to search
            if (AbilityData == null || !AbilityData.Any()) return false;

            abilityData = AbilityData.FirstOrDefault(x => x.IDName == id);
            return abilityData != null;
        }
    }
}
