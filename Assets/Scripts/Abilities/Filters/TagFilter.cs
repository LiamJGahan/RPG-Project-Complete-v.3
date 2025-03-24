using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Filters
{
    [CreateAssetMenu(fileName = "Tag Filter", menuName = "Abilities/Filters/Tag")]
    public class TagFilter : FilterStrategy
    {
        [SerializeField] string tagToFilter = "";

        public override IEnumerable<GameObject> Filter(IEnumerable<GameObject> objectsToFilter)
        {
            foreach(GameObject gameObject in objectsToFilter)
            {
                if(gameObject.CompareTag(tagToFilter))
                {
                    yield return gameObject;
                }
            }
        }
    }
}
