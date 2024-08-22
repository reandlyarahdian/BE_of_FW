// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 2.0.34
// 

using Colyseus.Schema;
using Action = System.Action;

public partial class PlaySession : Schema
{
    [Type(0, "string")]
    public string playSessionId = default(string);

    [Type(1, "number")]
    public float playSessionStartTS = default(float);

    [Type(2, "number")]
    public float playSessionActionCount = default(float);

    [Type(3, "boolean")]
    public bool isEligibleForMultiplier = default(bool);

    [Type(4, "number")]
    public float multiplierQuota = default(float);

    /*
     * Support for individual property change callbacks below...
     */

    protected event PropertyChangeHandler<string> __playSessionIdChange;
    public Action OnPlaySessionIdChange(PropertyChangeHandler<string> __handler, bool __immediate = true)
    {
        if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
        __callbacks.AddPropertyCallback(nameof(this.playSessionId));
        __playSessionIdChange += __handler;
        if (__immediate && this.playSessionId != default(string)) { __handler(this.playSessionId, default(string)); }
        return () => {
            __callbacks.RemovePropertyCallback(nameof(playSessionId));
            __playSessionIdChange -= __handler;
        };
    }

    protected event PropertyChangeHandler<float> __playSessionStartTSChange;
    public Action OnPlaySessionStartTSChange(PropertyChangeHandler<float> __handler, bool __immediate = true)
    {
        if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
        __callbacks.AddPropertyCallback(nameof(this.playSessionStartTS));
        __playSessionStartTSChange += __handler;
        if (__immediate && this.playSessionStartTS != default(float)) { __handler(this.playSessionStartTS, default(float)); }
        return () => {
            __callbacks.RemovePropertyCallback(nameof(playSessionStartTS));
            __playSessionStartTSChange -= __handler;
        };
    }

    protected event PropertyChangeHandler<float> __playSessionActionCountChange;
    public Action OnPlaySessionActionCountChange(PropertyChangeHandler<float> __handler, bool __immediate = true)
    {
        if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
        __callbacks.AddPropertyCallback(nameof(this.playSessionActionCount));
        __playSessionActionCountChange += __handler;
        if (__immediate && this.playSessionActionCount != default(float)) { __handler(this.playSessionActionCount, default(float)); }
        return () => {
            __callbacks.RemovePropertyCallback(nameof(playSessionActionCount));
            __playSessionActionCountChange -= __handler;
        };
    }

    protected event PropertyChangeHandler<bool> __isEligibleForMultiplierChange;
    public Action OnIsEligibleForMultiplierChange(PropertyChangeHandler<bool> __handler, bool __immediate = true)
    {
        if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
        __callbacks.AddPropertyCallback(nameof(this.isEligibleForMultiplier));
        __isEligibleForMultiplierChange += __handler;
        if (__immediate && this.isEligibleForMultiplier != default(bool)) { __handler(this.isEligibleForMultiplier, default(bool)); }
        return () => {
            __callbacks.RemovePropertyCallback(nameof(isEligibleForMultiplier));
            __isEligibleForMultiplierChange -= __handler;
        };
    }

    protected event PropertyChangeHandler<float> __multiplierQuotaChange;
    public Action OnMultiplierQuotaChange(PropertyChangeHandler<float> __handler, bool __immediate = true)
    {
        if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
        __callbacks.AddPropertyCallback(nameof(this.multiplierQuota));
        __multiplierQuotaChange += __handler;
        if (__immediate && this.multiplierQuota != default(float)) { __handler(this.multiplierQuota, default(float)); }
        return () => {
            __callbacks.RemovePropertyCallback(nameof(multiplierQuota));
            __multiplierQuotaChange -= __handler;
        };
    }

    protected override void TriggerFieldChange(DataChange change)
    {
        switch (change.Field)
        {
            case nameof(playSessionId): __playSessionIdChange?.Invoke((string)change.Value, (string)change.PreviousValue); break;
            case nameof(playSessionStartTS): __playSessionStartTSChange?.Invoke((float)change.Value, (float)change.PreviousValue); break;
            case nameof(playSessionActionCount): __playSessionActionCountChange?.Invoke((float)change.Value, (float)change.PreviousValue); break;
            case nameof(isEligibleForMultiplier): __isEligibleForMultiplierChange?.Invoke((bool)change.Value, (bool)change.PreviousValue); break;
            case nameof(multiplierQuota): __multiplierQuotaChange?.Invoke((float)change.Value, (float)change.PreviousValue); break;
            default: break;
        }
    }
}
