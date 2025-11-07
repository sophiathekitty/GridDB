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
            public static string GetAddressString(string domain, string sub, int index = 0, bool custom_data = true)
            {
                return domain + "." + sub + "." + index + "." + (custom_data ? "customdata" : "text");
            }
            //-------------------------------------------------------
            // fields
            //-------------------------------------------------------
            public string domain;
            public string sub;
            public int index = -1;
            public bool custom_data = false;
            //public int length;
            public long host;
            //-------------------------------------------------------
            // constructor
            //-------------------------------------------------------
            public GridDBAddress(string address)
            {
                if (address.StartsWith(GridDB.DB_PREFIX)) address = address.Substring(GridDB.DB_PREFIX.Length);
                if(address.Contains("@"))
                {
                    string[] addrParts = address.Split('@');
                    address = addrParts[0];
                    long h = 0;
                    if (long.TryParse(addrParts[1], out h))
                    {
                        host = h;
                    }
                }
                string[] parts = address.Split('.');
                //length = parts.Length;
                if (parts.Length == 4)
                {
                    int index = 0;
                    if (int.TryParse(parts[2], out index))
                    {
                        domain = parts[0];
                        sub = parts[1];
                        this.index = index;
                        custom_data = parts[3].ToLower() == "customdata";
                    }
                }
                else if (parts.Length == 3)
                {
                    int index = 0;
                    if (int.TryParse(parts[2], out index))
                    {
                        domain = parts[0];
                        sub = parts[1];
                        this.index = index;
                    }
                }
                else if (parts.Length == 2)
                {
                    domain = parts[0];
                    sub = parts[1];
                }
            }
            //-------------------------------------------------------
            // to string
            //-------------------------------------------------------
            public override string ToString()
            {
                string hostPart = host != 0 ? "@" + host.ToString() : "";
                if (index >= 0) return $"{domain}.{sub}.{index}.{(custom_data ? "customdata" : "text")}{hostPart}";
                return $"{domain}.{sub}{hostPart}"; // random index for domain.sub?
            }
            public override bool Equals(object obj)
            {
                return obj.ToString() == ToString();
            }
            public override int GetHashCode()
            {
                return ToString().GetHashCode();
            }
            public string ToBlockName()
            {
                return $"{GridDB.DB_PREFIX}{domain}.{sub}.{index.ToString("D2")}";
            }
        }
        //-----------------------------------------------------------------------
    }
}
