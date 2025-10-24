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
        // GridDataBlock
        //----------------------------------------------------------------------
        public class GridDataBlock
        {
            //------------------------------------------------------
            // fields
            //------------------------------------------------------
            public Dictionary<string, string> header = new Dictionary<string, string>();
            public string data = "";
            public string Name
            {
                get
                {
                    if (header.ContainsKey("Name")) return header["Name"];
                    return "";
                }
                set
                {
                    header["Name"] = value;
                }
            }
            public string Type
            {
                get
                {
                    if (header.ContainsKey("Type")) return header["Type"];
                    return "";
                }
                set
                {
                    header["Type"] = value;
                }
            }
            //------------------------------------------------------
            // constructor
            //------------------------------------------------------
            public GridDataBlock(string name, string type)
            {
                Name = name;
                Type = type;
            }
            public GridDataBlock(string data)
            {
                // split header and data
                var parts = data.Split(new char[] { '┤' }, 2);
                if (parts.Length == 2)
                {
                    // parse header
                    var header_lines = parts[0].Split(new char[] { '┼' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in header_lines)
                    {
                        var kv = line.Split(new char[] { '─' }, 2);
                        if (kv.Length == 2)
                        {
                            header[kv[0]] = kv[1];
                        }
                    }
                    // rest is data
                    this.data = parts[1];
                }
                else
                {
                    // no header, all is data
                    this.data = data;
                }
            }
            //------------------------------------------------------
            // save
            //------------------------------------------------------
            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                bool first = true;
                foreach (var kv in header)
                {
                    if (!first) sb.Append('┼');
                    sb.Append(kv.Key);
                    sb.Append('─');
                    sb.Append(kv.Value);
                    first = false;
                }
                if (sb.Length > 0) sb.Append('┤');
                sb.Append(data);
                return sb.ToString();
            }
        }
        //----------------------------------------------------------------------
    }
}
