using UnityEngine;

namespace IV.Core.Interactions
{
    [CreateAssetMenu(fileName = "", menuName = "IV/Interactions/Condition")]
    public class Condition : ScriptableObject
    {
        public bool Validate() => true;
    }
}