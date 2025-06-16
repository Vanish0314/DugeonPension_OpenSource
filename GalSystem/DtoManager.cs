using System;
using System.Collections.Generic;
using System.Linq;
using GameFramework;
using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dungeon.Gal
{
	public enum TutorialType
	{
		[LabelText("第一波布署开始时")] FirstDungeon,
		[LabelText("第二波布署开始时")] SecondDungeon,
		[LabelText("第三波布署开始时")] ThirdDungeon,
		[LabelText("第一波经营开始时")] FirstFactory,
		[LabelText("第二波经营开始时")] SecondFactory,
		[LabelText("第三波经营开始时")] ThirdFactory,
		[LabelText("第一波勇者来临后(播放完intro对话)")] FirstTeamArrival,
		[LabelText("第二波勇者来临后(播放完intro对话)")] SecondTeamArrival,
		[LabelText("第三波勇者来临后(播放完intro对话)")] ThirdTeamArrival
	}

	public class DtoManager : MonoBehaviour
	{
		[BoxGroup("主角对话树管理")]
		[LabelText("主角对话树"), SerializeReference]
		[TableList(ShowIndexLabels = true)]
		public List<HeroDialogueMap> HeroDtoMap = new();

		[BoxGroup("龙套对话树管理")]
		[LabelText("龙套对话树"), Tooltip("根据龙套的性格选择通用对话")]
		[SerializeReference]
		public NpcDialogueMap NpcDtoMap = new();

		[BoxGroup("教程对话树管理")]
		[LabelText("教程对话树"), Tooltip("根据教程名称寻找对话")]
		[ShowInInspector]
		public List<TutorialDialogueEntry> TutorialDtoMap = new();

		public DialogueTree GetClonedHeroDialogue(string characterName, DialogueType type) =>
			HeroDtoMap
				.FirstOrDefault(m => m.CharacterId == characterName)?
				.GetDialogue(type)
				.Pipe(tree => tree != null
					? PostProcessDialogueTree(Graph.Clone(tree, null))
					: WarnAndReturnNull($"[DtoManager][对话树] 未找到角色 {characterName} 的 {type} 对话树。"));

		public DialogueTree GetClonedNpcDialogue(CharacterType characterType) =>
			NpcDtoMap
				?.GetDialogue(characterType)
				.Pipe(tree => tree != null
					? PostProcessDialogueTree(Graph.Clone(tree, null))
					: WarnAndReturnNull($"[DtoManager][对话树] 未找到龙套的 {characterType} 对话树。"));

		public DialogueTree GetClonedTutorialDialogue(TutorialType tutorialType) =>
    		TutorialDtoMap
    		    .FirstOrDefault(m => m.Key == tutorialType)
    		    ?.Tree
    		    .Pipe(tree => tree != null
    		        ? PostProcessDialogueTree(Graph.Clone(tree, null))
    		        : WarnAndReturnNull($"[DtoManager][对话树] 未找到教程 {tutorialType} 的对话树。"));


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
	public class TutorialDialogueEntry
	{
		public TutorialType Key;
		public DialogueTree Tree;
	}
	
	[Serializable]
	public class CharacterDialogueMap<TEnum>
	{
		[LabelText("角色名称"), Tooltip("角色名称需要和冒险者工会系统中的勇者prefab名称一致")] public string CharacterId;

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
	[Serializable]
	public class HeroDialogueMap : CharacterDialogueMap<DialogueType> { }

	[Serializable]
	public class NpcDialogueMap : CharacterDialogueMap<CharacterType> { }



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
		[LabelText("堕落度对话1")] CorruptLevel1,
		[LabelText("堕落度对话2")] CorruptLevel2,
		[LabelText("堕落度对话3")] CorruptLevel3,
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