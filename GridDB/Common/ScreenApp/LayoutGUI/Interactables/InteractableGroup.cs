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
        // InteractableGroup a container for interactable items
        // - Allows navigation between contained interactables using WASD keys
        //-----------------------------------------------------------------------
        public class InteractableGroup : LayoutArea, IInteractable
        {
            List<IInteractable> interactables = new List<IInteractable>();
            IInteractable focusedItem = null;
            //-------------------------------------------------------------------
            // constructor
            //-------------------------------------------------------------------
            public InteractableGroup(ScreenApp screen, Vector2 position, Vector2 size, ScreenSprite item = null, float borderWidth = 0) : base(screen, position, size, item, borderWidth)
            {
            }
            //-------------------------------------------------------------------
            // IInteractable implementation
            //-------------------------------------------------------------------
            public string Id { get; set; }                                      // unique identifier for this Interactable
            public string Value                                                 // value associated with the focused item
            {
                get
                {
                    if (focusedItem != null) return focusedItem.Value;
                    return null;
                }
                set
                {
                    // set value on focused item
                    if (focusedItem != null) focusedItem.Value = value;
                }
            }
            public Vector2 Center                                               // center point of the item
            {
                get
                {
                    return Position + (Size / 2);
                }
            }
            bool focused = false;                                               // is the item currently focused
            public bool IsFocused                                               // is the item currently focused (setting will trigger OnFocus/OnBlur)
            {
                get
                {
                    return focused;
                }

                set
                {
                    focused = value;
                    if (focused)
                    {
                        OnFocus?.Invoke(this);
                        screen.KeyPrompt = KeyPrompt;
                    }
                    else OnBlur?.Invoke(this);
                    if(border != null) border.Visible = focused;
                }
            }
            public string KeyPrompt { get; set; } = "WASD Navigate | E Select"; // prompt to show when requesting keyboard input
            LayoutArea IInteractable.Area                                       // area the item is contained in
            {
                get
                {
                    return this;
                }
            }
            public Action<IInteractable> OnFocus { get; set; }                  // when the item receives focus (hovered)
            public Action<IInteractable> OnBlur { get; set; }                   // when the item loses focus (unhovered)
            public Action<IInteractable> OnClick { get; set; }                  // when the item is clicked
            public Action LayoutChanged { get; set; }                           // when the layout of the item changes
            public void Click()                                                 // perform click action
            {
                if (focusedItem != null) focusedItem.Click();
                else OnClick?.Invoke(this);
            }
            int delayMove = 0;                                                  // delay between focus moves
            const int MOVE_DELAY = 8;                                           // delay between focus moves
            public virtual bool RunInput(GameInput input)                       // process input for the item
            {
                //GridInfo.Echo("InteractableGroup.RunInput: processing input for group " + Id);
                if (focusedItem != null && focusedItem.RunInput(input)) return true; // let focused item handle input first
                if (input.WReleased || input.SReleased || input.AReleased || input.DReleased) delayMove = 0;
                if (delayMove > 0) 
                {
                    delayMove--;
                    return false;
                }
                if (input.EPressed) { Click(); return true; }   // click on E press
                // move focus up
                if (input.W)
                {
                    IInteractable upItem = MoveUp();
                    if (upItem != null)
                    {
                        focusedItem.IsFocused = false;
                        focusedItem = upItem;
                        focusedItem.IsFocused = true;
                        return true;
                    }
                    else if(interactables.Count > 0 && focusedItem == null)
                    {
                        focusedItem = interactables[0];
                        focusedItem.IsFocused = true;
                    }
                    delayMove = MOVE_DELAY;
                }
                // move focus down
                if (input.S)
                {
                    IInteractable downItem = MoveDown();
                    if (downItem != null)
                    {
                        focusedItem.IsFocused = false;
                        focusedItem = downItem;
                        focusedItem.IsFocused = true;
                        return true;
                    }
                    else if (interactables.Count > 0 && focusedItem == null)
                    {
                        focusedItem = interactables[0];
                        focusedItem.IsFocused = true;
                    }
                    delayMove = MOVE_DELAY;
                }
                // move focus left
                if (input.A)
                {
                    IInteractable leftItem = MoveLeft();
                    if (leftItem != null)
                    {
                        focusedItem.IsFocused = false;
                        focusedItem = leftItem;
                        focusedItem.IsFocused = true;
                        return true;
                    }
                    else if (interactables.Count > 0 && focusedItem == null)
                    {
                        focusedItem = interactables[0];
                        focusedItem.IsFocused = true;
                    }
                    delayMove = MOVE_DELAY;
                }
                // move focus right
                if (input.D)
                {
                    IInteractable rightItem = MoveRight();
                    if (rightItem != null)
                    {
                        focusedItem.IsFocused = false;
                        focusedItem = rightItem;
                        focusedItem.IsFocused = true;
                        return true;
                    }
                    else if (interactables.Count > 0 && focusedItem == null)
                    {
                        focusedItem = interactables[0];
                        focusedItem.IsFocused = true;
                    }
                    delayMove = MOVE_DELAY;
                }
                return false;
            }
            //-------------------------------------------------------------------
            // move up
            //-------------------------------------------------------------------
            IInteractable MoveUp()
            {
                if (focusedItem == null) return null;
                // try to find an interactable above the current one
                List<IInteractable> candidates = new List<IInteractable>();
                foreach (var item in interactables)
                {
                    if (item.Center.Y < focusedItem.Center.Y)
                    {
                        candidates.Add(item);
                    }
                }
                // find the closest one
                IInteractable closest = null;
                float closestDist = float.MaxValue;
                foreach (var item in candidates)
                {
                    float dist = Vector2.DistanceSquared(item.Center, focusedItem.Center);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closest = item;
                    }
                }
                return closest;
            }
            //-------------------------------------------------------------------
            // move down
            //-------------------------------------------------------------------
            IInteractable MoveDown()
            {
                if (focusedItem == null) return null;
                // try to find an interactable below the current one
                List<IInteractable> candidates = new List<IInteractable>();
                foreach (var item in interactables)
                {
                    if (item.Center.Y > focusedItem.Center.Y)
                    {
                        candidates.Add(item);
                    }
                }
                // find the closest one
                IInteractable closest = null;
                float closestDist = float.MaxValue;
                foreach (var item in candidates)
                {
                    float dist = Vector2.DistanceSquared(item.Center, focusedItem.Center);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closest = item;
                    }
                }
                //throw new Exception("InteractableGroup.MoveDown: closest item is " + (closest != null ? closest.Id : "null"));
                return closest;
            }
            //-------------------------------------------------------------------
            // move left
            //-------------------------------------------------------------------
            IInteractable MoveLeft()
            {
                if (focusedItem == null) return null;
                // try to find an interactable to the left of the current one
                List<IInteractable> candidates = new List<IInteractable>();
                foreach (var item in interactables)
                {
                    if (item.Center.X < focusedItem.Center.X)
                    {
                        candidates.Add(item);
                    }
                }
                // find the closest one
                IInteractable closest = null;
                float closestDist = float.MaxValue;
                foreach (var item in candidates)
                {
                    float dist = Vector2.DistanceSquared(item.Center, focusedItem.Center);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closest = item;
                    }
                }
                return closest;
            }
            //-------------------------------------------------------------------
            // move right
            //-------------------------------------------------------------------
            IInteractable MoveRight()
            {
                if (focusedItem == null) return null;
                // try to find an interactable to the right of the current one
                List<IInteractable> candidates = new List<IInteractable>();
                foreach (var item in interactables)
                {
                    if (item.Center.X > focusedItem.Center.X)
                    {
                        candidates.Add(item);
                    }
                }
                // find the closest one
                IInteractable closest = null;
                float closestDist = float.MaxValue;
                foreach (var item in candidates)
                {
                    float dist = Vector2.DistanceSquared(item.Center, focusedItem.Center);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closest = item;
                    }
                }
                return closest;
            }
            //-------------------------------------------------------------------
            // methods
            //-------------------------------------------------------------------
            public void AddInteractable(IInteractable item, bool addItem = true)    // add an interactable to the group
            {
                if (!interactables.Contains(item))
                {
                    interactables.Add(item);
                    item.LayoutChanged += () => { LayoutChanged?.Invoke(); };
                    if (addItem) AddItem(item.Area);
                }
            }
            public void AddInteractable(IInteractable item, string DOM)             // add an interactable to the group at specified DOM path 0.0.0.0
            {
                if (!interactables.Contains(item))
                {
                    interactables.Add(item);
                    item.LayoutChanged += () => { LayoutChanged?.Invoke(); };
                    LayoutArea area = LayoutArea.DOM(this,DOM);
                    if (area != null)
                    {
                        area.AddItem(item.Area);
                    } else throw new Exception("InteractableGroup.AddInteractable: could not find area for DOM: " + DOM);
                }
            }
            public void RemoveInteractable(IInteractable item) // remove an interactable from the group
            {
                if (interactables.Contains(item))
                {
                    interactables.Remove(item);
                    RemoveItem(item.Area);
                    if (focusedItem == item)
                    {
                        focusedItem.IsFocused = false;
                        focusedItem = MoveUp();
                        if (focusedItem == null) focusedItem = interactables.FirstOrDefault();
                        if (focusedItem != null) focusedItem.IsFocused = true;
                    }
                }
            }
        }
        //-----------------------------------------------------------------------
    }
}
