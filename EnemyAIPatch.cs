using HarmonyLib;
using JetBrains.Annotations;
using System.ComponentModel;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace MasksDropShells.Patches
{
    [HarmonyPatch(typeof(MaskedPlayerEnemy))]
    internal class MaskedPlayerEnemy_Patches
    {
        [HarmonyPatch(nameof(MaskedPlayerEnemy.KillEnemy))]
        [HarmonyPostfix]
        internal static void DropMaskOnDeath(MaskedPlayerEnemy __instance)
        {
            MaskDropShell.Instance.mls.LogInfo("HarmonyPatchOpened");
            if (!NetworkManager.Singleton.IsServer) return;

            var shellPrefab = GetShellItem();

            int shellRolls = MaskDropShell.Instance.ShellRolls.Value;
            int shellChance = MaskDropShell.Instance.ShellChance.Value;
            MaskDropShell.Instance.mls.LogInfo("masked dead");
            for (int i = 0; i < shellRolls; i++)
            {
                MaskDropShell.Instance.mls.LogInfo("rolling");
                int rnd = UnityEngine.Random.Range(0, 100);
                MaskDropShell.Instance.mls.LogInfo(rnd);
                if (rnd < shellChance)
                {
                    var shellObj = GameObject.Instantiate(shellPrefab.spawnPrefab, __instance.transform.position + new Vector3(0, 2.5f, 0), Quaternion.identity);
            
                    shellObj.GetComponentInChildren<GrabbableObject>().fallTime = 0f;
                    shellObj.GetComponentInChildren<GrabbableObject>().SetScrapValue(0);
                    shellObj.GetComponentInChildren<NetworkObject>().Spawn();
                    RoundManager.Instance.SyncScrapValuesClientRpc(new NetworkObjectReference[] { shellObj.GetComponent<NetworkObject>() }, new int[] { shellObj.GetComponent<GrabbableObject>().scrapValue });
                }
            }
        }


        private static Item _shellItem;
        private static Item GetShellItem()
        {
            if (_shellItem == null)
            {
                _shellItem = StartOfRound.Instance.allItemsList.itemsList.First(i => i.name == "GunAmmo");
            }
            return _shellItem;
        }
    }
}