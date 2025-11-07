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
        // RemoteDomainListLayout
        //----------------------------------------------------------------------
        public class RemoteDomainListLayout : LayoutVerticalScrollArea<DomainInfo>
        {
            //----------------------------------------------------------------------
            // constructor
            //----------------------------------------------------------------------
            public RemoteDomainListLayout(ScreenApp screen, Vector2 position, Vector2 size, List<DomainInfo> list, ScreenSprite item = null, float borderWidth = 0, bool leftIsBack = true) : base(screen, position, size, list, item, borderWidth, leftIsBack)
            {
            }
            //----------------------------------------------------------------------
            // LayoutVerticalScrollArea
            //----------------------------------------------------------------------
            protected override IInteractable CreateItem(DomainInfo data)
            {
                return new GridDBDomainItemLayout(screen, Position, new Vector2(MarginSize.X - 10, 80), data);
            }

            protected override string GetItemIdentifyer(DomainInfo data)
            {
                return data.Name;
            }
        }
        //----------------------------------------------------------------------
    }
}
