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
        // GridDBServer
        //-----------------------------------------------------------------------
        public class GridDBServer
        {
            //------------------------------------------------------
            // Init
            //------------------------------------------------------
            public static void Init()
            {
                GridInfo.AddBroadcastListener(GridDBClient.GETGRIDDBHOSTS);
                GridInfo.AddMessageHandler(GridDBClient.GETGRIDDBHOSTS, GetGridDBHosts);
                GridInfo.AddMessageHandler(GridDBClient.GETDOMAINLIST, GetDomainList);
                GridInfo.AddMessageHandler(GridDBClient.GETDOMAINBLOCKS, GetDomainBlocks);
                GridInfo.AddMessageHandler(GridDBClient.GETGRIDDATA, GetData);
                AnnounceExistence();
                GridInfo.AddMainLoop(Main);
                announceIntervalMinutes = GridInfo.RandomInt(4, 20);
            }
            static int announceIntervalMinutes = 5;
            static DateTime lastAnnounceTime = DateTime.MinValue;
            static void Main(string argument)
            {
                if ((DateTime.Now - lastAnnounceTime).TotalMinutes >= announceIntervalMinutes)
                {
                    AnnounceExistence();
                    lastAnnounceTime = DateTime.Now;
                }
            }
            static void AnnounceExistence()
            {
                MessageData msg = new MessageData();
                msg["Host"] = GridDBHostInfo.Me.ToString();
                GridInfo.IGC.SendBroadcastMessage(GridDBClient.GRIDDBHOST, msg.ToString());
            }
            static void GetGridDBHosts(MyIGCMessage msg) => GetGridDBHosts(MessageData.ParseMessage(msg));
            static void GetGridDBHosts(MessageData msg)
            {
                MessageData response = new MessageData();
                response["Host"] = GridDBHostInfo.Me.ToString();
                GridInfo.IGC.SendUnicastMessage(msg.Sender, GridDBClient.GRIDDBHOST, response.ToString());
            }
            //------------------------------------------------------
            // Get Domain List
            //------------------------------------------------------
            static void GetDomainList(MyIGCMessage msg) => GetDomainList(MessageData.ParseMessage(msg));
            static void GetDomainList(MessageData msg)
            {
                MessageData response = new MessageData();
                response["Client"] = msg["Client"];
                StringBuilder sb = new StringBuilder();
                foreach (string domain in GridDB.GetDomains())
                {
                    if (sb.Length > 0) sb.Append(GridDataBlock.ListSeparator);
                    sb.Append(new DomainInfo(domain, new GridData(GridDB.GetDomainMainDataAddress(domain), true)).ToString());
                }
                response["Domains"] = sb.ToString();
                GridInfo.IGC.SendUnicastMessage(msg.Sender, GridDBClient.DOMAINLIST,response.ToString());
            }
            static void GetDomainBlocks(MyIGCMessage msg) => GetDomainBlocks(MessageData.ParseMessage(msg));
            static void GetDomainBlocks(MessageData msg)
            {
                if(!msg.HasKey("Domain")) return;
                MessageData response = new MessageData();
                response["Domain"] = msg["Domain"];
                response["Client"] = msg["Client"];
                StringBuilder sb = new StringBuilder();
                foreach (string gdbAddr in GridDB.GetDomainAddresses(msg["Domain"]))
                {
                    if (sb.Length > 0) sb.Append(GridDataBlock.ListSeparator);
                    sb.Append(gdbAddr);
                }
                response["Addresses"] = sb.ToString();
                GridInfo.IGC.SendUnicastMessage(msg.Sender, GridDBClient.DOMAINBLOCKS, response.ToString());
            }
            //------------------------------------------------------
            // Get Data
            //------------------------------------------------------
            static void GetData(MyIGCMessage msg) => GetData(MessageData.ParseMessage(msg));
            static void GetData(MessageData msg)
            {
                if(!msg.HasKey("Address")) return;
                MessageData response = new MessageData();
                response["Client"] = msg["Client"];
                response["Address"] = msg["Address"];
                response["Data"] = GridDB.Get(msg["Address"]);
                GridInfo.IGC.SendUnicastMessage(msg.Sender, GridDBClient.GRIDDATA, response.ToString());
            }
        }
        //-----------------------------------------------------------------------
    }
}
