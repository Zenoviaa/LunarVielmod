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
        STARBLOCK,
        BreakString,
        DashPlayerSync,
        ResetColosseum,
        StartColosseum,
        HandleDoor,
        ScarecrowPlayerSync,

        PlaceRibbon,
        BreakRibbon,

        PlaceDecoration,
        BreakDecoration,
        AggroSync
    }

    public enum DialogueType : byte
    {
        Start_Verlia,
        Start_Irradia,
        Start_Goth
    }
}