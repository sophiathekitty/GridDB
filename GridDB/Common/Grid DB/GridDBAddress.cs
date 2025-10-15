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
        //-----------------------------------------------------------------------
        // GridDBAddress
        //-----------------------------------------------------------------------
        public class GridDBAddress
        {
            public string domain;
            public string sub;
            public int index;
            public bool custom_data;
            public int x;
            public int y;
            public int length;
            // constructor
            public GridDBAddress(string address)
            {
                if (address.StartsWith("DB:")) address = address.Substring(3);
                string[] parts = address.Split('.');
                length = parts.Length;
                if (parts.Length == 6)
                {
                    int index = 0;
                    int x = 0;
                    int y = 0;
                    if (int.TryParse(parts[2], out index) && int.TryParse(parts[4], out x) && int.TryParse(parts[5], out y))
                    {
                        this.domain = parts[0];
                        this.sub = parts[1];
                        this.index = index;
                        this.custom_data = parts[3].ToLower() == "customdata";
                        this.x = x;
                        this.y = y;
                    }
                }
                else if (parts.Length == 4)
                {
                    int index = 0;
                    if (int.TryParse(parts[2], out index))
                    {
                        this.domain = parts[0];
                        this.sub = parts[1];
                        this.index = index;
                        this.custom_data = parts[3].ToLower() == "customdata";
                    }
                }
                else if (parts.Length == 3)
                {
                    int index = 0;
                    if (int.TryParse(parts[2], out index))
                    {
                        this.domain = parts[0];
                        this.sub = parts[1];
                        this.index = index;
                        this.custom_data = false;
                    }
                }
                else if (parts.Length == 2)
                {
                    this.domain = parts[0];
                    this.sub = parts[1];
                    this.index = -1;
                    this.custom_data = false;
                }
            }
            // to string
            public override string ToString()
            {
                if (index >= 0) return domain + "." + sub + "." + index + "." + (custom_data ? "customdata" : "text");
                return domain + "." + sub;
            }
            public string ToBlockName()
            {
                if (index < 10) return "DB:" + domain + "." + sub + ".0" + index;
                if (index >= 0) return "DB:" + domain + "." + sub + "." + index;
                return "TV." + domain + "." + sub + ".0";
            }
        }
        //-----------------------------------------------------------------------
    }
}
