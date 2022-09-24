using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UMM;

namespace UltraRandomizer
{
    //[UKPlugin("UltraRandomizer", "Enemy Randomizer for ULTRAKILL", "1.0.0",false,false)]
    [BepInPlugin("radsi.ultrarandomizer", "UltraRandomizer", "1.0.0")]
    public class UltraRandomizer : BaseUnityPlugin
    {
        GameObject player;

        SpawnableObjectsDatabase objectsDatabase;

        ConfigEntry<int> difficulty;

        SpawnableObject newEnemy;
        public List<GameObject> ToDestroyThisFrame = new List<GameObject>();

        private void Start()
        {
            Logger.LogMessage("Plugin UltraRandomizer loaded");
            difficulty = Config.Bind("Enemys Randomizer", "Difficulty", 1, new ConfigDescription("The difficulty of the enemies that can appear (1-6)", new AcceptableValueRange<int>(1, 6)));
        }

        public static object GetPrivate<T>(T instance, string fieldName)
        {
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.NonPublic;
            FieldInfo field = typeof(T).GetField(fieldName, bindFlags);
            return field.GetValue(instance);
        }

        private void Update()
        {
            for (int i = 0; i < ToDestroyThisFrame.Count; i++)
            {
                GameObject enemy = ToDestroyThisFrame[i];
                if (enemy)
                {
                    Destroy(enemy);
                }
            }

            if (player == null)
            {
                player = GameObject.Find("Player");
            }
            else if (player != null && objectsDatabase == null)
            {
                objectsDatabase = (SpawnableObjectsDatabase)GetInstanceField(typeof(SpawnMenu), player.transform.GetChild(10).GetChild(21).gameObject.GetComponent<SpawnMenu>(), "objects");
                foreach (var x in objectsDatabase.enemies)
                {
                    x.objectName += " mod";
                    x.name += " mod";
                    x.gameObject.name += " mod";
                }
            }

            if (NewMovement.Instance)
            {
                GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");

                for (int i = 0; i < enemys.Length; i++)
                {
                    if (enemys[i].transform.childCount > 3 && !enemys[i].name.Contains("mod"))
                    {
                        System.Random r = new System.Random();
                        int[] arr;
                        int rInt = 0;

                        switch (difficulty.Value)
                        {
                            case 1:
                                arr = new int[] { 0, 1, 2, 3, 21 };
                                rInt = arr[r.Next(arr.Length)];
                                break;
                            case 2:
                                arr = new int[] { 0, 1, 2, 3, 4, 9, 14, 21 };
                                rInt = arr[r.Next(arr.Length)];
                                break;
                            case 3:
                                arr = new int[] { 0, 1, 2, 3, 4, 9, 14, 15, 21 };
                                rInt = arr[r.Next(arr.Length)];
                                break;
                            case 4:
                                arr = new int[] { 0, 1, 2, 3, 4, 9, 14, 15, 16, 19, 21, 22 };
                                rInt = arr[r.Next(arr.Length)];
                                break;
                            case 5:
                                arr = new int[] { 0, 1, 2, 3, 4, 5, 6, 9, 14, 15, 16, 18, 19, 21, 22 };
                                rInt = arr[r.Next(arr.Length)];
                                break;
                            case 6:
                                arr = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 9, 10, 11, 12, 13, 14, 15, 16, 18, 19, 20, 21, 22, 23, 24, 25 };
                                rInt = arr[r.Next(arr.Length)];
                                break;

                        }

                        newEnemy = objectsDatabase.enemies[rInt];

                        GameObject ne = Instantiate(newEnemy.gameObject);

                        ne.transform.position = enemys[i].transform.position;
                        ne.transform.SetParent(enemys[i].transform.parent);

                        GameObject enemy = enemys[i];
                        enemy.name += "mod";
                        ToDestroyThisFrame.Add(enemy);

                        if (enemy.TryGetComponent(out EventOnDestroy eod))
                        {
                            CallInstanceVoid(typeof(EventOnDestroy), eod, "OnDestroy");
                        }
                    }
                }
            }
        }

        internal static object GetInstanceField(Type type, object instance, string fieldName)
        {
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            FieldInfo field = type.GetField(fieldName, bindFlags);
            return field.GetValue(instance);
        }
        
        internal static object CallInstanceVoid(Type type, object instance, string voidName)
        {
            MethodInfo dynMethod = type.GetType().GetMethod(voidName,
            BindingFlags.NonPublic | BindingFlags.Instance);
            return dynMethod.Invoke(instance, null);
        }
    }
}
