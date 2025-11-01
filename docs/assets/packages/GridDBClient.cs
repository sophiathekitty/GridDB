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
using System.Web;
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
        //=======================================================================
        // GridDB Client Package
        // 
        // Dependencies: This package requires GridInfo and MessageData classes
        // which should be included in your main script for IGC communication.
        //=======================================================================
        
        //-----------------------------------------------------------------------
        // GridDBAddress (Core dependency)
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
            public int index;
            public bool custom_data;
            public int x;
            public int y;
            public int length;
            //-------------------------------------------------------
            // constructor
            //-------------------------------------------------------
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
            //-------------------------------------------------------
            // to string
            //-------------------------------------------------------
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

        //----------------------------------------------------------------------
        // GridDataBlock (Core dependency)
        //----------------------------------------------------------------------
        public class GridDataBlock
        {
            public static char ListSeparator = '║'; // separates lists
            public static char HeaderKVSeparator = '╬'; // separates header key-value pairs
            public static char HeaderKVPairSeparator = '═'; // separates header key from value
            
            //------------------------------------------------------
            // fields
            //------------------------------------------------------
            public string name;
            public Dictionary<string, string> header = new Dictionary<string, string>();
            public List<string> data = new List<string>();
            
            //------------------------------------------------------
            // constructor
            //------------------------------------------------------
            public GridDataBlock(string name)
            {
                this.name = name;
            }
            
            public GridDataBlock(string name, string data)
            {
                this.name = name;
                ParseData(data);
            }
            
            //------------------------------------------------------
            // ParseData
            //------------------------------------------------------
            void ParseData(string data)
            {
                if (string.IsNullOrEmpty(data)) return;
                
                string[] parts = data.Split(new char[] { '╣' }, 2); // header separator
                if (parts.Length > 0 && !string.IsNullOrEmpty(parts[0]))
                {
                    // parse header
                    string[] headerParts = parts[0].Split(new char[] { HeaderKVSeparator }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string headerPart in headerParts)
                    {
                        string[] kv = headerPart.Split(new char[] { HeaderKVPairSeparator }, 2);
                        if (kv.Length == 2)
                        {
                            header[kv[0].Trim()] = kv[1].Trim();
                        }
                    }
                }
                
                if (parts.Length > 1)
                {
                    // parse data
                    string[] dataLines = parts[1].Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    this.data.AddRange(dataLines);
                }
            }
        }

        //----------------------------------------------------------------------
        // GridData (Core dependency)
        //----------------------------------------------------------------------
        public class GridData
        {
            public static char BlockSeparator = '║'; // separates blocks
            public static char HeaderSeparator = '╣'; // separates header from data
            public static char HeaderKVSeparator = '╬'; // separates header key-value pairs
            public static char HeaderKVPairSeparator = '═'; // separates header key from value
            
            //------------------------------------------------------
            // fields
            //------------------------------------------------------
            public GridDBAddress address { get; private set; }
            public Dictionary<string, string> header = new Dictionary<string, string>();
            public string Data { get; private set; }
            
            //------------------------------------------------------
            // constructor
            //------------------------------------------------------
            public GridData(string address, string data)
            {
                this.address = new GridDBAddress(address);
                this.Data = data;
                ParseHeader(data);
            }
            
            public GridData(GridDBAddress address, string data)
            {
                this.address = address;
                this.Data = data;
                ParseHeader(data);
            }
            
            //------------------------------------------------------
            // ParseHeader
            //------------------------------------------------------
            void ParseHeader(string data)
            {
                if (string.IsNullOrEmpty(data)) return;
                
                string[] parts = data.Split(new char[] { HeaderSeparator }, 2);
                if (parts.Length > 0 && !string.IsNullOrEmpty(parts[0]))
                {
                    string[] headerParts = parts[0].Split(new char[] { HeaderKVSeparator }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string headerPart in headerParts)
                    {
                        string[] kv = headerPart.Split(new char[] { HeaderKVPairSeparator }, 2);
                        if (kv.Length == 2)
                        {
                            header[kv[0].Trim()] = kv[1].Trim();
                        }
                    }
                }
            }
        }

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
            public int BlockCount;
            public string Icon;
            public List<GridDBAddress> Blocks;
            
            //------------------------------------------------
            // constructor
            //------------------------------------------------
            public DomainInfo(string name, string domain, int blockCount, string icon = "")
            {
                this.Name = name;
                this.Domain = domain;
                this.BlockCount = blockCount;
                this.Icon = icon;
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
                        string hostName = "DefaultHost" + GridInfo.IGC.Me.ToString().Substring(0, 4);

                        if (!GridInfo.Me.CustomData.Contains("HostName"))
                        {
                            // host info not setup
                            GridInfo.Me.CustomData = $"HostName={hostName}\n" + GridInfo.Me.CustomData;
                        }
                        string[] lines = GridInfo.Me.CustomData.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
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
        // GridDBClient
        //----------------------------------------------------------------------
        public class GridDBClient
        {
            //------------------------------------------------
            // static properties
            //------------------------------------------------
            public static List<GridDBHostInfo> GridDBHosts = new List<GridDBHostInfo>();
            static DateTime lastHostFindTime = DateTime.MinValue;
            
            //------------------------------------------------
            // Init
            //------------------------------------------------
            public static void Init()
            {
                GridInfo.AddMessageHandler("GridDBHost", GridDBHostReceived);
                //FindHosts();
            }
            
            //------------------------------------------------
            // Find Hosts
            //------------------------------------------------
            public static void FindHosts()
            {
                if ((DateTime.Now - lastHostFindTime).TotalMinutes < 1) return; // limit to once every minute
                GridDBHosts.Clear();
                GridInfo.IGC.SendBroadcastMessage("GetGridDBHosts", "");
                lastHostFindTime = DateTime.Now;
            }
            
            static void GridDBHostReceived(MyIGCMessage msg) => GridDBHostReceived(MessageData.ParseMessage(msg));
            static void GridDBHostReceived(MessageData msg)
            {
                // handle received host list
                if (msg["Type"] != "GridDBHost") return;
                GridDBHostInfo host = new GridDBHostInfo(msg["Host"]);
                if(!GridDBHosts.Any(h => h.HostId==host.HostId)) GridDBHosts.Add(host);
            }
            
            //------------------------------------------------
            // fields
            //------------------------------------------------
            public GridDBHostInfo Host { get; private set; } = null;
            string ClientName;
            public Action<List<DomainInfo>> DomainInfoReceived;
            public Action<DomainInfo, List<GridDBAddress>> DomainAddressReceived;
            public Action<GridData> DataReceived;
            
            //------------------------------------------------
            // constructor
            //------------------------------------------------
            public GridDBClient(string clientName)
            {
                ClientName = $"{clientName}_{DateTime.Now.Ticks}";
                GridInfo.AddMessageHandler("DomainList", DomainListRecieved);
                GridInfo.AddMessageHandler("DomainBlocks", DomainBlocksReceived);
                GridInfo.AddMessageHandler("GridData", GridDataReceived);
            }
            
            //------------------------------------------------
            // Connect to Host
            //------------------------------------------------
            public void ConnectToHost(GridDBHostInfo host, bool getDomainList = true)
            {
                Host = host;
                if (getDomainList) RequestDomainList();
            }
            
            public void ConnectToHost(string hostName, bool getDomainList = true)
            {
                GridDBHostInfo host = GridDBHosts.FirstOrDefault(h => h.HostName == hostName);
                if (host != null)
                {
                    ConnectToHost(host, getDomainList);
                }
            }
            
            //------------------------------------------------
            // Request Domain List
            //------------------------------------------------
            public void RequestDomainList() 
            { 
                MessageData msg = new MessageData();
                msg["Client"] = ClientName;
                GridInfo.IGC.SendUnicastMessage(Host.HostId, "GetDomainList", msg.ToString());
            }
            
            void DomainListRecieved(MyIGCMessage msg) => DomainListRecieved(MessageData.ParseMessage(msg));
            void DomainListRecieved(MessageData msg)
            {
                if (msg["Client"] != ClientName) return;
                Host.Domains = new List<DomainInfo>();
                string[] domainParts = msg["Domains"].Split(new char[] { GridDataBlock.ListSeparator }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string domainData in domainParts)
                {
                    Host.Domains.Add(new DomainInfo(domainData));
                }
                DomainInfoReceived?.Invoke(Host.Domains);
            }
            
            //------------------------------------------------
            // Request Domain Blocks
            //------------------------------------------------
            public void RequestDomainBlocks(string domain)
            {
                MessageData msg = new MessageData();
                msg["Domain"] = domain;
                msg["Client"] = ClientName;
                GridInfo.IGC.SendUnicastMessage(Host.HostId, "GetDomainBlocks", msg.ToString());
            }
            
            void DomainBlocksReceived(MyIGCMessage msg) => DomainBlocksReceived(MessageData.ParseMessage(msg));
            void DomainBlocksReceived(MessageData msg)
            {
                if (msg["Client"] != ClientName) return;
                DomainInfo domain = Host[msg["Domain"]];
                if (domain == null) return;
                string[] addresses = msg["Addresses"].Split(new char[] { GridDataBlock.ListSeparator }, StringSplitOptions.RemoveEmptyEntries);
                domain.Blocks = new List<GridDBAddress>();
                foreach (string addr in addresses)
                {
                    domain.Blocks.Add(new GridDBAddress(addr));
                }
                DomainAddressReceived?.Invoke(domain, domain.Blocks);
            }
            
            //------------------------------------------------
            // Request Data
            //------------------------------------------------
            public void RequestData(GridDBAddress address)
            {
                MessageData msg = new MessageData();
                msg["Address"] = address.ToString();
                msg["Client"] = ClientName;
                GridInfo.IGC.SendUnicastMessage(Host.HostId, "GetData", msg.ToString());
            }
            
            void GridDataReceived(MyIGCMessage msg) => GridDataReceived(MessageData.ParseMessage(msg));
            void GridDataReceived(MessageData msg)
            {
                if (msg["Client"] != ClientName) return;
                GridData data = new GridData(msg.Address, msg.Data);
                DataReceived?.Invoke(data);
            }
        }
        
        //=======================================================================
    }
}