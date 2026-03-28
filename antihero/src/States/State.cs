// SPDX-FileCopyrightText: 2026 Sukaretto
// SPDX-License-Identifier: AGPL-3.0-only

namespace antihero.States;

public abstract class State
{
    public virtual void OnEnter() { }
    public virtual void OnExit() { }
    public virtual void OnUpdate() { }
}
