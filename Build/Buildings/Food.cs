using System.Collections;
using Dungeon.Common;
using UnityEngine;

namespace Dungeon
{
    public class Food : MonoPoolItem
    {
        public int SatietyValue { get; private set; }
        public int MentalValue { get; private set; }
        
        private Coroutine autoDestroyCor;

        public void Initialize(int satiety, int mental)
        {
            SatietyValue = satiety;
            MentalValue = mental;
        }

        IEnumerator AutoReturnToPool(float delay)
        {
            yield return new WaitForSeconds(delay);
            ReturnToPool();
            autoDestroyCor = null;
        }

        public void Consume(float existTime)
        {
            autoDestroyCor = StartCoroutine(AutoReturnToPool(existTime));
        }

        public override void OnSpawn(object data)
        {
            
        }

        public override void Reset()
        {
            SatietyValue = 0;
            MentalValue = 0;
            autoDestroyCor = null;
        }
    }
}
