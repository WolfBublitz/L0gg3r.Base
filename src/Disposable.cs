// ----------------------------------------------------------------------------
// <copyright file="Disposable.cs" company="L0gg3r">
// Copyright (c) L0gg3r Project
// </copyright>
// ----------------------------------------------------------------------------

using System;

/// <summary>
/// Represents a disposable object that executes a specified action when disposed.
/// </summary>
internal class Disposable : IDisposable
{
    // ┌────────────────────────────────────────────────────────────────────────────────┐
    // │ Private Fields                                                                 │
    // └────────────────────────────────────────────────────────────────────────────────┘
    private readonly Action action;

    // ┌────────────────────────────────────────────────────────────────────────────────┐
    // │ Public Constructors                                                            │
    // └────────────────────────────────────────────────────────────────────────────────┘

    /// <summary>
    /// Initializes a new instance of the <see cref="Disposable"/> class with the specified action.
    /// </summary>
    /// <param name="action">The action to execute when the object is disposed.</param>
    public Disposable(Action action)
    {
        this.action = action;
    }

    // ┌────────────────────────────────────────────────────────────────────────────────┐
    // │ Public Methods                                                                 │
    // └────────────────────────────────────────────────────────────────────────────────┘

    /// <inheritdoc/>
    public void Dispose() => action();
}
