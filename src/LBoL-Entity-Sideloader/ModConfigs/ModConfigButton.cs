using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LBoLEntitySideloader.ModConfigs
{
    /// <summary>
    /// Adds standalone action buttons to a mod's page in the options panel.
    /// </summary>
    /// <remarks>
    /// Example:
    /// <c>
    /// ModConfigButtons.Register(PInfo.GUID,
    ///        section: "Buttons",
    ///        key: "HelloButton",
    ///        onClick: () => BepinexPlugin.log.LogInfo("Hello world"), 
    ///        description: "Prints hello world",
    ///        buttonText: "click me!");
    /// </c>
    /// </remarks>
    public static class ModConfigButtons
    {
        internal sealed class ButtonDef
        {
            public string Guid, Section, Key, Description, ButtonText;
            public Action OnClick;
        }

        /// <summary>
        /// Stores all the registered buttons added by mods
        /// </summary>
        private static readonly List<ButtonDef> _all = new List<ButtonDef>();

        /// <summary>
        /// Call from your plugin's Awake.
        /// </summary>
        /// <param name="pluginGuid">GUID from your pinfo.</param>
        /// <param name="section">Section it appears in. "" puts it at the top with no header.</param>
        /// <param name="key">Id of the button. Also the yaml id, so keep it consistent.</param>
        /// <param name="onClick">Runs when the button is clicked.</param>
        /// <param name="description">description text under the row. Yaml overrides it.</param>
        /// <param name="buttonText">Text on the button. Yaml overrides it. If null, the yaml Name or the key of the button is used.</param>
        public static void Register(string pluginGuid, string section, string key, Action onClick, string description = null,
                                    string buttonText = null)
        {
            section = section ?? "";
            _all.RemoveAll(b => b.Guid == pluginGuid && b.Section == section && b.Key == key); // safe on reload
            _all.Add(new ButtonDef
            {
                Guid = pluginGuid,
                Section = section,
                Key = key,
                Description = description,
                ButtonText = buttonText,
                OnClick = onClick
            });
        }

        /// <summary>
        /// Returns all buttons belonging to a particular mod
        /// </summary>
        internal static IEnumerable<ButtonDef> For(string guid) => _all.Where(b => b.Guid == guid);
    }
}
