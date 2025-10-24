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
        // GridData
        //----------------------------------------------------------------------
        public class GridData
        {
            //------------------------------------------------------
            // fields
            //------------------------------------------------------
            public GridDBAddress address { get; private set; }
            public Dictionary<string, string> header = new Dictionary<string, string>();
            List<GridDataBlock> blocks = new List<GridDataBlock>();
            Dictionary<string, GridDataBlock> BlocksByName = new Dictionary<string, GridDataBlock>();
            //------------------------------------------------------
            // constructor
            //------------------------------------------------------
            public GridData(string address, bool headerOnly = false)
            {
                this.address = new GridDBAddress(address);
                ParseData(GridDB.Get(address),headerOnly);
            }
            public GridData(string address, string data)
            {
                this.address = new GridDBAddress(address);
                ParseData(data);
            }
            //------------------------------------------------------
            // methods
            //------------------------------------------------------
            public void ParseData(string data, bool headerOnly = false)
            {
                BlocksByName.Clear();
                blocks.Clear();
                header.Clear();
                if (data == null) return;
                string[] parts = data.Split(new char[] { '╣' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                {
                    // parse header
                    foreach (string h in parts[0].Split(new char[] { '╬' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string[] kv = h.Split(new char[] { '═' }, 2);
                        if (kv.Length == 2)
                        {
                            header[kv[0]] = kv[1];
                        }
                    }
                    // update data to just the data part
                    data = parts[1];
                }
                else data = parts[0];
                if(headerOnly) return;
                foreach (string b in data.Split(new char[] { '║' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    blocks.Add(new GridDataBlock(b));
                }
                foreach (var b in blocks)
                {
                    if (BlocksByName.ContainsKey(b.Name))
                    {
                        GridInfo.Echo("Warning: Duplicate block name in GridData: " + address.domain + "." + address.sub + "." + address.index + " - " + b.Name);
                        header["Warning"] = "Duplicate block name: " + b.Name;
                        continue;
                    }
                    if (b.Name != "") BlocksByName[b.Name] = b;
                }
            }
            //------------------------------------------------------
            // add [] operator for BlocksByName
            //------------------------------------------------------
            public GridDataBlock this[string name]
            {
                get
                {
                    if (BlocksByName.ContainsKey(name)) return BlocksByName[name];
                    return null;
                }
            }
            //------------------------------------------------------
            // add [] operator for blocks by index
            //------------------------------------------------------
            public GridDataBlock this[int index]
            {
                get
                {
                    if (index >= 0 && index < blocks.Count) return blocks[index];
                    return null;
                }
            }
            //------------------------------------------------------
            // save
            //------------------------------------------------------
            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                // header
                bool first = true;
                foreach (var kv in header)
                {
                    if (first) first = false;
                    else sb.Append('╬');
                    sb.Append(kv.Key);
                    sb.Append('═');
                    sb.Append(kv.Value);
                }
                if (!first) sb.Append('╣');
                // blocks
                first = true;
                foreach (var b in blocks)
                {
                    if (first) first = false;
                    else sb.Append('║');
                    sb.Append(b.ToString());
                }
                return sb.ToString();
            }
            public void Save(bool addifnew = false)
            {
                GridDB.Set(address, this.ToString(), addifnew);
            }
        }
        //----------------------------------------------------------------------
    }
}
