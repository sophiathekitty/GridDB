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
        // DomainInfo
        //----------------------------------------------------------------------
        public class DomainInfo
        {
            //------------------------------------------------
            // fields
            //------------------------------------------------
            public string Name;
            public string Domain;
            public string MainAddress;
            public int BlockCount;
            public string Icon;
            public List<GridDBAddress> Blocks;
            //------------------------------------------------
            // constructor
            //------------------------------------------------
            public DomainInfo(string name, string domain, int blockCount, string icon = "", string mainAddress = null)
            {
                this.Name = name;
                this.Domain = domain;
                this.BlockCount = blockCount;
                this.Icon = icon;
                MainAddress = mainAddress;
            }
            public DomainInfo(string domain, GridData dd)
            {
                if (dd.header.ContainsKey("Name")) Name = dd.header["Name"];
                if (dd.header.ContainsKey("Domain")) Domain = domain;
                BlockCount = GridDB.DomainSize(domain);
                if (dd.header.ContainsKey("Icon")) Icon = dd.header["Icon"];
                MainAddress = dd.address.ToString();
            }
            public DomainInfo(string data)
            {
                string[] parts = data.Split(new char[] { GridDataBlock.HeaderKVSeparator }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string part in parts)
                {
                    string[] kv = part.Split(new char[] { GridDataBlock.HeaderKVPairSeparator }, StringSplitOptions.RemoveEmptyEntries);
                    if (kv.Length != 2) continue;
                    string key = kv[0].Trim();
                    string value = kv[1].Trim();
                    switch (key)
                    {
                        case "Name":
                            this.Name = value;
                            break;
                        case "Domain":
                            this.Domain = value;
                            break;
                        case "BlockCount":
                            int bc;
                            if (int.TryParse(value, out bc)) this.BlockCount = bc;
                            break;
                        case "Icon":
                            this.Icon = value;
                            break;
                        case "MainAddress":
                            this.MainAddress = value;
                            break;
                    }
                }
            }
            //------------------------------------------------
            // to string
            //------------------------------------------------
            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Name").Append(GridDataBlock.HeaderKVPairSeparator).Append(Name).Append(GridDataBlock.HeaderKVSeparator);
                sb.Append("Domain").Append(GridDataBlock.HeaderKVPairSeparator).Append(Domain).Append(GridDataBlock.HeaderKVSeparator);
                sb.Append("BlockCount").Append(GridDataBlock.HeaderKVPairSeparator).Append(BlockCount.ToString()).Append(GridDataBlock.HeaderKVSeparator);
                sb.Append("Icon").Append(GridDataBlock.HeaderKVPairSeparator).Append(Icon).Append(GridDataBlock.HeaderKVSeparator);
                return sb.ToString();
            }
        }
        //----------------------------------------------------------------------
    }
}
