using System;
using System.Collections.Generic;
using System.Text;
using ScriptEngine;
using BepInEx;

namespace LBoLEntitySideloader
{
    internal class ScriptEngineWrapper
    {
        static internal void ReloadPlugins(BaseUnityPlugin scriptEngineInstance) 
        { 

            if(scriptEngineInstance is ScriptEngine.ScriptEngine se)
            {
                se.ReloadPlugins();
            }
        }

    }
}
