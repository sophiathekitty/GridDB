using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        //----------------------------------------------------------------------
        // ScreenAppId
        //----------------------------------------------------------------------
        public class ScreenAppId
        {
            //----------------------------------------------------------------------
            // fields
            //----------------------------------------------------------------------
            public string Name { get; private set; }
            public long Host { get; private set; }
            public bool Local { get { return Host == GridInfo.IGC.Me; } }
            public string Id { get { return ToString(); } }
            //----------------------------------------------------------------------
            // constructor
            //----------------------------------------------------------------------
            public ScreenAppId(string name, long host)
            {
                Name = name;
                Host = host;
            }
            public ScreenAppId(string name, string script)
            {
                Name = name;
                Host = ScreenApp.ScriptId(script);
            }
            public ScreenAppId(string id)
            {
                string[] parts = id.Split('@');
                if (parts.Length == 2)
                {
                    long h;
                    Name = parts[0];
                    long.TryParse(parts[1], out h);
                    Host = h;
                }
                else
                {
                    Name = id;
                    Host = GridInfo.IGC.Me;
                }
            }
            public override string ToString()
            {
                return $"{Name}@{Host}";
            }
        }
        //----------------------------------------------------------------------
    }
}
