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
        // GridDBHostList
        //-----------------------------------------------------------------------
        public class GridDBHostList : LayoutVerticalScrollArea<GridDBHostInfo>
        {
            GridDBClient dBClient;
            //-------------------------------------------------------------------
            // constructor
            //-------------------------------------------------------------------
            public GridDBHostList(ScreenApp screen, Vector2 position, Vector2 size,GridDBClient client, ScreenSprite item = null, float borderWidth = 0, bool leftIsBack = true) : base(screen, position, size, GridDBClient.GridDBHosts, item, borderWidth, leftIsBack)
            {
                FlexibleWidth = false;
                Margin = new Rectangle(0, 0, 0, 0);
                dBClient = client;
                GridDBClient.FindHosts();
            }
            //-------------------------------------------------------------------
            // LayoutVerticalScrollArea overrides
            //-------------------------------------------------------------------
            protected override IInteractable CreateItem(GridDBHostInfo data)
            {
                return new LayoutButton(screen, MarginPosition, new Vector2(MarginSize.X,50), data.HostName,"host",button_value: GetItemIdentifyer(data),borderWidth: 1);
            }

            protected override string GetItemIdentifyer(GridDBHostInfo data)
            {
                return data.HostId.ToString();
            }

            //-------------------------------------------------------------------
            // event handlers
            protected override void OnItemClicked(IInteractable source)
            {
                if(source.Id.ToLower() == "host")
                {
                    // connect to host
                    GridInfo.Echo($"Connecting to host {source.Value}...");
                    long hostId = 0;
                    if (long.TryParse(source.Value, out hostId)) dBClient.ConnectToHost(hostId);
                    base.OnItemClicked(source);
                }
            }
        }
        //-----------------------------------------------------------------------
    }
}
