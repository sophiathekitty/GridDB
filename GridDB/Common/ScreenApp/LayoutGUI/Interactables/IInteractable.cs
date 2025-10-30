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
        // IInteractable
        //-----------------------------------------------------------------------
        public interface IInteractable
        {
            string Id { get; set; }                     // unique identifier for the item
            string Value { get; set; }                  // value associated with the item
            Vector2 Center { get; }                     // center position of the item
            LayoutArea Area { get; }                    // area the item is contained in
            bool IsFocused { get; set; }                // is the item currently focused
            Action<IInteractable> OnFocus { get; set; } // when the item receives focus (hovered)
            Action<IInteractable> OnBlur { get; set; }  // when the item loses focus (unhovered)
            Action<IInteractable> OnClick { get; set; } // when the item is clicked
            Action<IInteractable> OnBack { get; set; }  // when the item requests a back action
            Action LayoutChanged { get; set; }          // when the layout of the item changes
            void Click();                               // perform click action
            bool RunInput(GameInput input);             // process input for the item
            string KeyPrompt { get; set; }              // prompt to show when requesting keyboard input
        }
        //-----------------------------------------------------------------------
    }
}
