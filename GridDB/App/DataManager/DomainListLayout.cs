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
                headerButton.OnFocus += OnPrevious;
                AddInteractable(headerButton);
                float yPos = 0;
                domainList = GridDB.GetDomains();
                foreach (string domain in domainList)
                {
                    pageSize++;
                    GridDBDomainItemLayout itemLayout = new GridDBDomainItemLayout(screen, position, new Vector2(size.X - 10, 80), domain);
                    AddInteractable(itemLayout);
                    itemLayout.OnDelete += OnDeleteDomain;
                    yPos += 80;
                    if (yPos + 80 > MarginSize.Y - 200)
                    {
                        footerButton = new LayoutButton(screen, position, new Vector2(size.X - 10, 40), "More", "FooterButton");
                        footerButton.TestSize = 0.75f;
                        footerButton.OnFocus += OnMore;
                        AddInteractable(footerButton);
                        break; // stop if we exceed screen height
                    }
                }
            }
            //-------------------------------------------------------
            // event handlers
            //-------------------------------------------------------
            void OnPrevious(IInteractable source)
            {
                if (currentOffset > 0)
                {
                    currentOffset--;
                    if (currentOffset == 0) headerButton.Text = "Back";
                    // remove the second to last interactable (the last domain item)
                    RemoveInteractableAt(pageSize);
                    // add the current top domain item at the top
                    GridDBDomainItemLayout itemLayout = new GridDBDomainItemLayout(screen, Position, new Vector2(MarginSize.X - 10, 80), domainList[currentOffset]);
                    AddInteractableAt(itemLayout, 1);
                    itemLayout.OnDelete += OnDeleteDomain;
                    // ok update the layout
                    LayoutChanged?.Invoke();
                    FocusDown();
                }
            }
            void OnBack(IInteractable source)
            {
               OnClick?.Invoke(source);
            }
            void OnMore(IInteractable source)
            {
                if(currentOffset + pageSize >= domainList.Count) return; // no more
                currentOffset++;
                headerButton.Text = "Previous";
                // remove the first domain item interactable
                // add the next domain item at the bottom
                if (currentOffset + pageSize-1 < domainList.Count)
                {
                    RemoveInteractableAt(1);
                    GridDBDomainItemLayout itemLayout = new GridDBDomainItemLayout(screen, Position, new Vector2(MarginSize.X - 10, 80), domainList[currentOffset + pageSize-1]);
                    AddInteractableAt(itemLayout, pageSize);
                    itemLayout.OnDelete += OnDeleteDomain;
                    LayoutChanged?.Invoke();
                    FocusUp();
                }
            }
            void OnDeleteDomain(GridDBDomainItemLayout domainItem)
            {
                RemoveInteractable(domainItem);
                domainList.Remove(domainItem.Domain);
                if(footerButton != null)
                {
                    // add a new domain item at the bottom if there are more domains
                    if (currentOffset + pageSize - 1 < domainList.Count)
                    {
                        GridDBDomainItemLayout itemLayout = new GridDBDomainItemLayout(screen, Position, new Vector2(MarginSize.X - 10, 80), domainList[currentOffset + pageSize - 1]);
                        AddInteractableAt(itemLayout, pageSize);
                        itemLayout.OnDelete += OnDeleteDomain;
                    }
                }
                LayoutChanged?.Invoke();
            }
        }
        //-----------------------------------------------------------------------
    }
}
