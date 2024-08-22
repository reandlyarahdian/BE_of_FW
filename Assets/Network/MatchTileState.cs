// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 2.0.34
// 

using Colyseus.Schema;
using Action = System.Action;

public partial class MatchTileState : Schema
{
    [Type(0, "ref", typeof(PlaySession))]
    public PlaySession playSession = new PlaySession();

    [Type(1, "number")]
    public float energy = default(float);

    [Type(2, "number")]
    public float score = default(float);

    [Type(3, "number")]
    public float currentLevel = default(float);

    /*
     * Support for individual property change callbacks below...
     */

    protected event PropertyChangeHandler<PlaySession> __playSessionChange;
    public Action OnPlaySessionChange(PropertyChangeHandler<PlaySession> __handler, bool __immediate = true)
    {
        if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
        __callbacks.AddPropertyCallback(nameof(this.playSession));
        __playSessionChange += __handler;
        if (__immediate && this.playSession != null) { __handler(this.playSession, null); }
        return () => {
            __callbacks.RemovePropertyCallback(nameof(playSession));
            __playSessionChange -= __handler;
        };
    }

    protected event PropertyChangeHandler<float> __energyChange;
    public Action OnEnergyChange(PropertyChangeHandler<float> __handler, bool __immediate = true)
    {
        if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
        __callbacks.AddPropertyCallback(nameof(this.energy));
        __energyChange += __handler;
        if (__immediate && this.energy != default(float)) { __handler(this.energy, default(float)); }
        return () => {
            __callbacks.RemovePropertyCallback(nameof(energy));
            __energyChange -= __handler;
        };
    }

    protected event PropertyChangeHandler<float> __scoreChange;
    public Action OnScoreChange(PropertyChangeHandler<float> __handler, bool __immediate = true)
    {
        if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
        __callbacks.AddPropertyCallback(nameof(this.score));
        __scoreChange += __handler;
        if (__immediate && this.score != default(float)) { __handler(this.score, default(float)); }
        return () => {
            __callbacks.RemovePropertyCallback(nameof(score));
            __scoreChange -= __handler;
        };
    }

    protected event PropertyChangeHandler<float> __currentLevelChange;
    public Action OnCurrentLevelChange(PropertyChangeHandler<float> __handler, bool __immediate = true)
    {
        if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
        __callbacks.AddPropertyCallback(nameof(this.currentLevel));
        __currentLevelChange += __handler;
        if (__immediate && this.currentLevel != default(float)) { __handler(this.currentLevel, default(float)); }
        return () => {
            __callbacks.RemovePropertyCallback(nameof(currentLevel));
            __currentLevelChange -= __handler;
        };
    }

    protected override void TriggerFieldChange(DataChange change)
    {
        switch (change.Field)
        {
            case nameof(playSession): __playSessionChange?.Invoke((PlaySession)change.Value, (PlaySession)change.PreviousValue); break;
            case nameof(energy): __energyChange?.Invoke((float)change.Value, (float)change.PreviousValue); break;
            case nameof(score): __scoreChange?.Invoke((float)change.Value, (float)change.PreviousValue); break;
            case nameof(currentLevel): __currentLevelChange?.Invoke((float)change.Value, (float)change.PreviousValue); break;
            default: break;
        }
    }
}