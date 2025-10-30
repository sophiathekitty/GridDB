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
using static IngameScript.Program;

namespace IngameScript
{
    partial class Program
    {
        //-----------------------------------------------------------------------
        // LayoutVerticalScrollArea
        //-----------------------------------------------------------------------
        public abstract class LayoutVerticalScrollArea<T> : InteractableGroup
        {
            //-------------------------------------------------------
            // fields
            //-------------------------------------------------------
            protected List<T> list;
            //LayoutButton headerButton; // back or previous page
            //LayoutButton footerButton; // next page (if any)
            int currentOffset = 0;
            int lastIndex {get { return interactables.Count-1; } }
            float pageHeight
            {
                get
                {
                    return ContentSize.Y;
                }
            }
            bool BottomItemSelected
            {
                get
                {
                    if (lastIndex < 0 || focusedItem == null) return false;
                    return interactables[lastIndex] == focusedItem;
                }
            }
            //-------------------------------------------------------
            // constructor
            //-------------------------------------------------------
            public LayoutVerticalScrollArea(ScreenApp screen, Vector2 position, Vector2 size, List<T> list, ScreenSprite item = null, float borderWidth = 0, bool leftIsBack = true) : base(screen, position, size, item, borderWidth)
            {
                FlexibleHeight = true;
                HasBottomEdgeInteractable = true;
                HasTopEdgeInteractable = true;
                HasLeftEdgeInteractable = leftIsBack;
                this.list = list;
                float heightUsed = 0;
                foreach (T data in list)
                {
                    AddInteractableAtBottom(CreateItem(data));
                    heightUsed += interactables[lastIndex].Area.Size.Y;
                    if (heightUsed >= MarginSize.Y) break;
                    /*
                    if (pageHeight > MarginSize.Y)
                    {
                        RemoveInteractableFromBottom();
                        break;
                    }
                    */
                }
            }
            protected abstract IInteractable CreateItem(T data);
            protected virtual void OnItemClicked(IInteractable source)
            {
                OnClick?.Invoke(source);
            }
            //-------------------------------------------------------
            // Add Item
            //-------------------------------------------------------
            public void AddInteractableAtTop(IInteractable item)
            {
                // add the item after the header button
                AddInteractableAt(item, 0);
                item.OnClick += OnItemClicked;
            }
            public void AddInteractableAtBottom(IInteractable item)
            {
                // add the item before the footer button (if any)
                AddInteractable(item);
                item.OnClick += OnItemClicked;
            }
            //-------------------------------------------------------
            // Remove Item
            //-------------------------------------------------------
            public void RemoveInteractableFromTop()
            {
                if (lastIndex < 0) return;
                interactables[0].OnClick -= OnItemClicked;
                RemoveInteractableAt(0);
            }
            public void RemoveInteractableFromBottom()
            {
                if (lastIndex < 0) return;
                interactables[lastIndex].OnClick -= OnItemClicked;
                RemoveInteractableAt(lastIndex);
            }
            public void RemoveInteractableListItem(IInteractable item, string data)
            {
                item.OnClick -= OnItemClicked;
                RemoveInteractable(item);
                list.Remove((T)Convert.ChangeType(data, typeof(T)));
            }
            //-------------------------------------------------------
            // Events
            //-------------------------------------------------------
            // override Top edge focus to handle scrolling
            //-------------------------------------------------------
            protected override void OnTopEdgeInteractableFocused() => OnPrevious(this);
            public virtual void OnPrevious(IInteractable source)
            {
                if(list == null || list.Count == 0|| currentOffset <= 0) return;
                RemoveInteractableFromBottom();
                currentOffset--;
                AddInteractableAtTop(CreateItem(list[currentOffset]));
                LayoutChanged?.Invoke();
                FocusUp();
            }
            //-------------------------------------------------------
            // override Bottom edge focus to handle scrolling
            //-------------------------------------------------------
            protected override void OnBottomEdgeInteractableFocused() => OnMore(this);
            public virtual void OnMore(IInteractable source)
            {
                if (list == null || list.Count == 0 || currentOffset + 1 + lastIndex >= list.Count) return;
                RemoveInteractableFromTop();
                currentOffset++;
                AddInteractableAtBottom(CreateItem(list[currentOffset + lastIndex+1])); // does this need a minu one? interactables used to start at 1 because of header button
                LayoutChanged?.Invoke();
                FocusDown();
            }
            //-------------------------------------------------------
            // override Left edge focus to handle back navigation
            //-------------------------------------------------------
            protected override void OnLeftEdgeInteractableFocused() => OnBack?.Invoke(this);
            //-------------------------------------------------------
            // apply layout
            //-------------------------------------------------------
            public override void ApplyLayout()
            {
                base.ApplyLayout();
                if (pageHeight > MarginSize.Y)
                {
                    if (BottomItemSelected) RemoveInteractableFromTop();
                    else RemoveInteractableFromBottom();
                }
            }
        }
        //-----------------------------------------------------------------------
    }
}
