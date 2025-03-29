using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace REPO_Rebalanced.patches;

[HarmonyPatch(typeof(ShopManager), "Awake")]
public static class ChangeShopValueCurve
{

    // Reflecting these fields
    /*
       internal float itemValueMultiplier = 4f;
       internal float upgradeValueIncrease = 0.5f;
       internal float healthPackValueIncrease = 0.05f;
       internal float crystalValueIncrease = 0.2f;
     */
    private static readonly (string name, float defaultValue)[] ShopMultipliers = 
    {
        ("itemValueMultiplier", 4f),
        ("upgradeValueIncrease", 0.5f),
        ("healthPackValueIncrease", 0.05f),
        ("crystalValueIncrease", 0.2f)
    };
    static void Postfix(ShopManager __instance)
    {
        foreach (var setting in ShopMultipliers)
        {
            var configEntry = Plugin.Config.Bind("Shop Multipliers", setting.name, setting.defaultValue);
            typeof(ShopManager)
                .GetField(setting.name, BindingFlags.Instance | BindingFlags.NonPublic)?
                .SetValue(__instance, configEntry.Value);
        }
    }
}

[HarmonyPatch(typeof(RunManager), "ChangeLevel")]
public static class ChangeShopProportiesPatch
{
    static void Prefix(RunManager __instance)
    {
        var keys = StatsManager.instance.itemDictionary.Keys.ToList();

        foreach (var key in keys)
        {
            var item = StatsManager.instance.itemDictionary[key];
    
            item.maxAmount = Plugin.Config.Bind($"Shop Item {item.name}", "Max amount", item.maxAmount).Value;
            item.maxPurchase = Plugin.Config.Bind($"Shop Item {item.name}", "Max Purchase", item.maxPurchase).Value;
            item.maxAmountInShop = Plugin.Config.Bind($"Shop Item {item.name}", "Max amount in shop", item.maxAmountInShop).Value;
            item.maxPurchaseAmount = Plugin.Config.Bind($"Shop Item {item.name}", "Max purchase amount", item.maxPurchaseAmount).Value;
            item.itemVolume = Plugin.Config.Bind($"Shop Item {item.name}", "Item volume", item.itemVolume).Value;

            StatsManager.instance.itemDictionary[key] = item;
        }
    }
}