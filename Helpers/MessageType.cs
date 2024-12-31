namespace Stellamod.Helpers
{
    public enum MessageType : byte
	{
		None = 0,
		ProjectileData,
		Dodge,
		Dash,
		BossSpawnFromClient,
		SpawnExplosiveBarrel,
		BoonData,
		CompleteMerenaQuest,
		CompleteZuiQuest,
		CreatePortal,
		StartBossFromDialogue,
		StartDialogue,
		STARBLOCK
	}

	public enum DialogueType : byte
	{
		Start_Verlia,
		Start_Irradia,
		Start_Goth
	}

	public enum QuestMessageType : byte
	{
		Deactivate = 0,
		Activate,
		ProgressOrComplete,
		SyncOnNPCLoot,
		SyncOnEditSpawnPool,
		Unlock,
		SyncNPCQueue
	}
}