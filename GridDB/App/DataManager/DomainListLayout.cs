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
        public class DomainListLayout : InteractableGroup
        {
            //-------------------------------------------------------
            // fields
            //-------------------------------------------------------
            List<string> domainList;
            LayoutButton headerButton; // back or previous page
            LayoutButton footerButton; // next page (if any)
            int currentOffset = 0;
            int pageSize = 0;
            //-------------------------------------------------------
            // constructor
            //-------------------------------------------------------
            public DomainListLayout(ScreenApp screen, Vector2 position, Vector2 size, ScreenSprite item = null, float borderWidth = 0) : base(screen, position, size, item, borderWidth)
            {
                Margin = new Rectangle(0, 0, 0, 0);
                headerButton = new LayoutButton(screen, position, new Vector2(size.X - 10, 40), "Back", "back");
                headerButton.TestSize = 0.75f;
                headerButton.OnClick += OnBack;
                AddInteractable(headerButton);
                float yPos = 0;
                domainList = GridDB.GetDomains();
                foreach (string domain in domainList)
                {
                    pageSize++;
                    GridDBDomainItemLayout itemLayout = new GridDBDomainItemLayout(screen, position, new Vector2(size.X - 10, 80), domain);
                    AddInteractable(itemLayout);
                    itemLayout.OnDelete += OnDeleteDomain;
                    yPos += itemLayout.Size.Y;
                    if (yPos > size.Y - 80)
                    {
                        footerButton = new LayoutButton(screen, position, new Vector2(size.X - 10, 40), "More", "FooterButton");
                        footerButton.TestSize = 0.75f;
                        footerButton.OnClick += OnMore;
                        AddInteractable(footerButton);
                        break; // stop if we exceed screen height
                    }
                }
            }
            //-------------------------------------------------------
            // event handlers
            //-------------------------------------------------------
            void OnBack(IInteractable source)
            {
                if (currentOffset > 0)
                {
                    currentOffset--;
                    if (currentOffset == 0) headerButton.Text = "Back";
                }
                else OnClick?.Invoke(source);
            }
            void OnMore(IInteractable source)
            {
                if(currentOffset + pageSize >= domainList.Count) return; // no more
                currentOffset++;
                headerButton.Text = "Previous";
            }
            void OnDeleteDomain(GridDBDomainItemLayout domainItem)
            {
                RemoveInteractable(domainItem);
                LayoutChanged?.Invoke();
            }
        }
        //-----------------------------------------------------------------------
    }
}
