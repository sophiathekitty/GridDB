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
        // GridDBHostInfo
        //----------------------------------------------------------------------
        public class GridDBHostInfo
        {
            //------------------------------------------------
            // static properties
            //------------------------------------------------
            static GridDBHostInfo _me = null;
            public static GridDBHostInfo Me
            {
                get
                {
                    if (_me == null)
                    {
                        if (!GridInfo.Me.CustomData.Contains("HostName"))
                        {
                            // host info not setup
                            GridInfo.Me.CustomData = "HostName=DefaultHost\n" + GridInfo.Me.CustomData;
                        }
                        string[] lines = GridInfo.Me.CustomData.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        string hostName = "DefaultHost";
                        foreach (string line in lines)
                        {
                            if (line.StartsWith("HostName="))
                            {
                                hostName = line.Substring("HostName=".Length).Trim();
                                break;
                            }
                        }
                        _me = new GridDBHostInfo(GridInfo.IGC.Me, hostName);
                    }
                    return _me;
                }
            }
            //------------------------------------------------
            // fields
            //------------------------------------------------
            public long HostId;
            public string HostName;
            public List<DomainInfo> Domains;
            //------------------------------------------------
            // constructor
            //------------------------------------------------
            public GridDBHostInfo(long hostId, string hostName)
            {
                HostId = hostId;
                HostName = hostName;
            }
            public GridDBHostInfo(string data)
            {
                string[] parts = data.Split(new char[] { GridData.HeaderKVSeparator }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string part in parts)
                {
                    string[] kv = part.Split(new char[] { GridData.HeaderKVPairSeparator }, 2);
                    if (kv.Length == 2)
                    {
                        if (kv[0] == "HostId")
                        {
                            long.TryParse(kv[1], out HostId);
                        }
                        else if (kv[0] == "HostName")
                        {
                            HostName = kv[1];
                        }
                    }
                }
            }
            public override string ToString()
            {
                return "HostId" + GridData.HeaderKVPairSeparator + HostId.ToString() + GridData.HeaderKVSeparator +
                       "HostName" + GridData.HeaderKVPairSeparator + HostName + GridData.HeaderKVSeparator;
            }
            //------------------------------------------------
            // [] operator for Domains by Domain name
            //------------------------------------------------
            public DomainInfo this[string domain]
            {
                get
                {
                    if (Domains != null)
                    {
                        foreach (var d in Domains)
                        {
                            if (d.Domain == domain) return d;
                        }
                    }
                    return null;
                }
                set
                {
                    if (Domains == null) Domains = new List<DomainInfo>();
                    for (int i = 0; i < Domains.Count; i++)
                    {
                        if (Domains[i].Domain == domain)
                        {
                            Domains[i] = value;
                            return;
                        }
                    }
                    Domains.Add(value);
                }
            }
        }
        //----------------------------------------------------------------------
    }
}
