using HarmonyLib;
using System.Linq;
using System;

namespace REPO_Rebalanced
{
    [HarmonyPatch(typeof(ShopManager), "GetAllItemsFromStatsManager")]
    public class ShopManagerPatch
    {
        static void Postfix(ShopManager __instance)
        {
            System.Random rng = new System.Random();

            foreach (var item in StatsManager.instance.itemDictionary.Values)
            {
                // Verificar la probabilidad del ítem usando la lista configurada
                var itemInfo = Plugin.ItemSpawnInfos.FirstOrDefault(info => info.ItemName == item.itemAssetName);

                if (itemInfo != null)
                {
                    Plugin.Logger.LogInfo($"Evaluando aparición de {item.itemAssetName}...");
                    if (rng.NextDouble() < itemInfo.SpawnChance)
                    {
                        Plugin.Logger.LogInfo($"¡Agregando {item.itemAssetName} a la tienda!");
                        __instance.potentialItems.Add(item);
                    }
                }
            }
        }
    }
}