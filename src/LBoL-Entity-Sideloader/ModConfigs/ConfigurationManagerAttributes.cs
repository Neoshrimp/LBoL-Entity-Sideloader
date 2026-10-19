using System;
using System.Collections.Generic;
using System.Text;

namespace LBoLEntitySideloader.ModConfigs
{
    /// <summary>
    /// Custom metadata attached to a config entry to control its appearance and behavior in the options panel.
    /// <br>See remarks for examples on how to use.</br>
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>GUI Control Cheat Sheet:</b><br/>
    /// • <b>Toggle Switch:</b> Bind a <c>bool</c>.<br/>
    /// • <b>Slider:</b> Pass an <c>AcceptableValueRange&lt;T&gt;</c> to <c>ConfigDescription</c>.<br/>
    /// • <b>Dropdown:</b> Pass an <c>AcceptableValueList&lt;T&gt;</c> (or standard <c>Enum</c>) to <c>ConfigDescription</c>.<br/>
    /// • <b>Action Button:</b> See <see cref="ModConfigButtons"/><br/>
    /// • <b>Text Box:</b> Default fallback for strings or unbounded numbers.
    /// </para>
    /// <para>Example: Creating a Slider (Range):</para>
    /// <code>
    /// Config.Bind("Gameplay", "SpawnRate", 1.0f, new ConfigDescription("Spawn multiplier",
    ///     new AcceptableValueRange&lt;float&gt;(0.1f, 5.0f)));
    /// </code>
    /// <para>Example: Creating a Dropdown (List):</para>
    /// <code>
    /// Config.Bind("Gameplay", "Difficulty", "Normal", new ConfigDescription("Game mode",
    ///     new AcceptableValueList&lt;string&gt;("Easy", "Normal", "Hard")));
    /// </code>
    /// <para>Example: Requiring restart:</para>
    /// <code>
    /// Config.Bind("Extra", "ResolutionScale", 1.0f, new ConfigDescription("Internal scale", null,
    ///     new ConfigurationManagerAttributes
    ///     {
    ///         RequiresRestart = true
    ///     }));
    /// </code>
    /// <para>Example: Key binding:</para>
    /// <code>
    /// // Single key
    /// var reloadKey = Config.Bind("Controls", "ReloadKey", KeyCode.F5, "Reloads the thing.");
    /// // Key + Modifier (like shift, ctrl, etc) (use keyboardshortcut)
    /// var reloadKey = Config.Bind("Controls", "ReloadKey",
    ///     new KeyboardShortcut(KeyCode.F5, KeyCode.LeftControl), "Reloads the thing.");
    ///
    /// // In an Update method:
    /// if (Input.GetKeyDown(reloadKey.Value)) Reload(); // single key
    /// if (reloadKey.Value.IsDown()) Reload(); // multi key
    /// </code>
    /// <para>
    /// Use <c>KeyboardShortcut.Empty</c> (or <c>KeyCode.None</c>) as the default for an unbound key.
    /// </para>
    /// </remarks>
    public sealed class ConfigurationManagerAttributes
    {
        /// <summary>
        /// This config will be hidden from the panel. It will only be in view in the actual .cfg file.
        /// </summary>
        public bool? Hidden;
        /// <summary>
        /// This config cannot be interacted with inside the panel.
        /// </summary>
        public bool? ReadOnly;
        /// <summary>
        /// Tags this option as requiring a restart. When changed, shows a notice at the bottom.
        /// </summary>
        public bool? RequiresRestart;
    }
}
