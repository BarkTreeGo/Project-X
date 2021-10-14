using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class PersistentObjectSpawner : MonoBehaviour
    {
        [SerializeField] GameObject persistentObjectPrefab;

        static bool hasSpawned = false; //static variables persistent during an application run (doesn't changes between scenes)

        private void Awake()
        {
            if (hasSpawned) { return; }
            SpawnPersistentObjects();
            hasSpawned = true;
        }

        private void SpawnPersistentObjects()
        {
            GameObject persitentObject = Instantiate(persistentObjectPrefab);
            DontDestroyOnLoad(persitentObject);
        }
    }
}
