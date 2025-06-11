using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace CopyModIDtoClipboard
{
	public class CopyModIDtoClipboardMod : Mod
	{
		public CopyModIDtoClipboardMod(ModContentPack pack) : base(pack)
		{
			new Harmony("CopyModIDtoClipboardMod").PatchAll();
		}
	}
	
	[HarmonyPatch(typeof(Page_ModsConfig), "DoModInfo")]
	public static class Page_ModsConfig_Patch
	{
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
		{
			foreach (var item in codeInstructions)
			{
				yield return item;
				if (item.OperandIs("ModPackageId"))
				{
					yield return new CodeInstruction(OpCodes.Ldloc_S, 4);
					yield return new CodeInstruction(OpCodes.Ldarg_1);
					yield return new CodeInstruction(OpCodes.Ldarg_2);
					yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Page_ModsConfig_Patch), "CopyModIDtoClipboard"));
				}
			}
		}
		
		public static void CopyModIDtoClipboard(float num, Rect r, ModMetaData mod)
		{
			Rect rect = new Rect(0f, num, r.width / 2f, Text.LineHeight);
			if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown && Event.current.button == 0)
			{
				GUIUtility.systemCopyBuffer = mod.packageIdLowerCase;
				Messages.Message("Copied to clipboard: " + mod.packageIdLowerCase, MessageTypeDefOf.TaskCompletion, false);
				Event.current.Use();
			}
		}
	}
}