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
        // DomainListLayout
        //-----------------------------------------------------------------------
        public class DomainListLayout : LayoutVerticalScrollArea<string>
        {
            //-------------------------------------------------------
            // constructor
            //-------------------------------------------------------
            public DomainListLayout(ScreenApp screen, Vector2 position, Vector2 size, ScreenSprite item = null, float borderWidth = 0) : base(screen, position, size, GridDB.GetDomains(), item, borderWidth)
            {
                Margin = new Rectangle(0, 0, 0, 0);
            }
            //-------------------------------------------------------
            // event handlers
            //-------------------------------------------------------
            protected override void OnItemClicked(IInteractable source)
            {
                if(source.Id.ToLower() == "delete")
                {
                    // delete the domain (and remove the item)
                    GridInfo.Echo($"Deleting domain {source.Value}...");
                    GridDB.Delete(source.Value);
                    RemoveInteractableListItem(focusedItem, source.Value);
                    LayoutChanged?.Invoke();
                }
                base.OnItemClicked(source);
            }
            //-------------------------------------------------------
            // LayoutVerticalScrollArea overrides
            //-------------------------------------------------------
            // create item
            //-------------------------------------------------------
            protected override IInteractable CreateItem(string data)
            {
                return new GridDBDomainItemLayout(screen, Position, new Vector2(MarginSize.X - 10, 80), data);
            }
        }
        //-----------------------------------------------------------------------
    }
}
