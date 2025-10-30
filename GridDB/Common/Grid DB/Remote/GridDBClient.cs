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
        //----------------------------------------------------------------------
    }
}
