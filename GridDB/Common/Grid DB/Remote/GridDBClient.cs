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
            public static string GRIDDBHOST = "GridDBHost";
            public static string DOMAINLIST = "DomainList";
            public static string DOMAINBLOCKS = "DomainBlocks";
            public static string GRIDDATA = "GridData";
            public static string GETGRIDDBHOSTS = "GetGridDBHosts";
            public static string GETDOMAINLIST = "GetDomainList";
            public static string GETDOMAINBLOCKS = "GetDomainBlocks";
            public static string GETGRIDDATA = "GetData";
            public static List<GridDBHostInfo> GridDBHosts = new List<GridDBHostInfo>();
            static DateTime lastHostFindTime = DateTime.MinValue;
            //------------------------------------------------
            // Init
            //------------------------------------------------
            public static void Init()
            {
                GridInfo.AddBroadcastListener(GRIDDBHOST);
                GridInfo.AddMessageHandler(GRIDDBHOST, GridDBHostReceived);
                FindHosts();
            }
            //------------------------------------------------
            // Find Hosts
            //------------------------------------------------
            public static void FindHosts()
            {
                if ((DateTime.Now - lastHostFindTime).TotalMinutes < 1) return; // limit to once every minute
                GridInfo.IGC.SendBroadcastMessage(GETGRIDDBHOSTS, GETGRIDDBHOSTS);
                lastHostFindTime = DateTime.Now;
            }
            static void GridDBHostReceived(MyIGCMessage msg) => GridDBHostReceived(MessageData.ParseMessage(msg));
            static void GridDBHostReceived(MessageData msg)
            {
                // handle received host list
                GridDBHostInfo host = new GridDBHostInfo(msg["Host"]);
                if (!GridDBHosts.Any(h => h.HostId==host.HostId)) GridDBHosts.Add(host);
                else
                {
                    // update existing host info
                    GridDBHostInfo existingHost = GridDBHosts.First(h => h.HostId == host.HostId);
                    existingHost.HostName = host.HostName;
                } 
            }
            //------------------------------------------------
            // fields
            //------------------------------------------------
            public GridDBHostInfo Host { get; private set; } = null;
            string ClientName;
            public Action<List<DomainInfo>> DomainInfoReceived;
            public Action<DomainInfo, List<GridDBAddress>> DomainAddressReceived;
            public Action<GridData> DataReceived;
            public bool Connected => Host != null;
            public bool HasDomains => Connected && Host.Domains != null && Host.Domains.Count > 0;
            public bool LoadingDomains => Connected && Host.Domains == null;
            public string DownloadingAddress { get; private set; } = "";
            //------------------------------------------------
            // constructor
            //------------------------------------------------
            public GridDBClient(string clientName)
            {
                ClientName = $"{clientName}_{DateTime.Now.Ticks}";
                GridInfo.AddMessageHandler(DOMAINLIST, DomainListRecieved);
                GridInfo.AddMessageHandler(DOMAINBLOCKS, DomainBlocksReceived);
                GridInfo.AddMessageHandler(GRIDDATA, GridDataReceived);
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
            public void ConnectToHost(long hostId, bool getDomainList = true)
            {
                GridDBHostInfo host = GridDBHosts.FirstOrDefault(h => h.HostId == hostId);
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
                GridInfo.IGC.SendUnicastMessage(Host.HostId, GETDOMAINLIST, msg.ToString());
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
                GridInfo.IGC.SendUnicastMessage(Host.HostId, GETDOMAINBLOCKS, msg.ToString());
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
                // sort domain blocks by address block name
                domain.Blocks.Sort((a, b) => a.ToBlockName().CompareTo(b.ToBlockName()));
                DomainAddressReceived?.Invoke(domain, domain.Blocks);
            }
            //------------------------------------------------
            // Request Data
            //------------------------------------------------
            public void RequestData(GridDBAddress address)
            {
                if(DownloadingAddress!="") return; // already downloading
                DownloadingAddress = address.ToString();
                MessageData msg = new MessageData();
                msg["Address"] = address.ToString();
                msg["Client"] = ClientName;
                GridInfo.IGC.SendUnicastMessage(Host.HostId, GETGRIDDATA, msg.ToString());
            }
            void GridDataReceived(MyIGCMessage msg) => GridDataReceived(MessageData.ParseMessage(msg));
            void GridDataReceived(MessageData msg)
            {
                if (msg["Client"] != ClientName) return;
                DownloadingAddress = "";
                GridData data = new GridData(msg.Address, msg.Data);
                DataReceived?.Invoke(data);
            }
        }
        //----------------------------------------------------------------------
    }
}
