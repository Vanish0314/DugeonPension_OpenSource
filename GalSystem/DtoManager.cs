using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Dungeon.DungeonGameEntry;
using GameFramework;
using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;
using Sirenix.OdinInspector;
using UnityEditor.EditorTools;
using UnityEngine;

namespace Dungeon.Gal
{

	public class DtoManager : MonoBehaviour
	{
		[BoxGroup("主角对话树管理")]
		[LabelText("主角对话树"), SerializeReference]
		[TableList(ShowIndexLabels = true)]
		public List<CharacterDialogueMap<DialogueType>> HeroDtoMap = new();

		[BoxGroup("龙套对话树管理")]
		[LabelText("龙套对话树"), Tooltip("根据龙套的性格选择通用对话")]
		[SerializeReference]
		public CharacterDialogueMap<CharacterType> NpcDtoMap = new();

		public DialogueTree GetClonedHeroDialogue(string characterName, DialogueType type) =>
			HeroDtoMap
				.FirstOrDefault(m => m.CharacterId == characterName)?
				.GetDialogue(type)
				.Pipe(tree => tree != null
					? PostProcessDialogueTree(Graph.Clone(tree, null))
					: WarnAndReturnNull($"未找到角色 {characterName} 的 {type} 对话树。"));

		public DialogueTree GetClonedNpcDialogue(CharacterType characterType) =>
			NpcDtoMap
				?.GetDialogue(characterType)
				.Pipe(tree => tree != null
					? PostProcessDialogueTree(Graph.Clone(tree, null))
					: WarnAndReturnNull($"未找到龙套的 {characterType} 对话树。"));

		private static DialogueTree PostProcessDialogueTree(DialogueTree tree) =>
			tree.Tap(t => t.actorParameters.ForEach(p =>
				p.actor = DungeonGameEntry.DungeonGameEntry.GalSystem.GetDungeonGalActor(p.name)));

		private static DialogueTree WarnAndReturnNull(string msg)
		{
			GameFrameworkLog.Warning(msg);
			return null;
		}
	}
	[Serializable]
	public class DialogueEntry<TEnum>
	{
		public TEnum Key;
		public DialogueTree Tree;
	}

	[Serializable]
	public class CharacterDialogueMap<TEnum>
	{
		public string CharacterId;

		[TableList]
		public List<DialogueEntry<TEnum>> Dialogues = new();

		public DialogueTree GetDialogue(TEnum key)
		{
			foreach (var entry in Dialogues)
			{
				if (EqualityComparer<TEnum>.Default.Equals(entry.Key, key))
					return entry.Tree;
			}
			return null;
		}
	}


	public enum CharacterType
	{
		[LabelText("胆小")] Weak,
		[LabelText("勇敢")] Brave,
		[LabelText("狡猾")] Cunning,
		[LabelText("狂暴")] Ruthless,
	}
	public enum DialogueType
	{
		[LabelText("进入地牢对话")] EnterDungeon,
		[LabelText("说服对话")] Convincing,
		[LabelText("捕获对话")] Capture,
	}
}


public static class FunctionalExtensions
{
	public static TResult Pipe<TInput, TResult>(this TInput input, Func<TInput, TResult> func) => func(input);

	public static T Tap<T>(this T value, Action<T> action)
	{
		action(value);
		return value;
	}
}