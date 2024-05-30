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
		StartVerlia,
		StartIrradia,
		STARBLOCK
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