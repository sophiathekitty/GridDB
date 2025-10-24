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
        // GridDBDomainItemLayout
        //-----------------------------------------------------------------------
        public class GridDBDomainItemLayout : HorizontalLayoutArea, IInteractable
        {
            //-------------------------------------------------------------------
            // fields
            //-------------------------------------------------------------------
            LayoutArea textArea;
            LayoutMenu FileActions;
            public string Domain { get; private set; }
            //-------------------------------------------------------------------
            // constructor
            //-------------------------------------------------------------------
            public GridDBDomainItemLayout(ScreenApp screen, Vector2 position, Vector2 size, string Domain) : base(screen, position, size, new TextureSprite(position,size, "SquareSimple", Color.DarkSlateGray * 0.75f), 1)
            {
                // don't stretch to fill height
                FlexibleHeight = false;
                FlexibleWidth = true;
                Margin = new Rectangle(1, 1, 1, 1);
                GridData domainHeader = new GridData(GridDB.GetDomainMainDataAddress(Domain),true);
                // add text label
                Id = domainHeader.address.ToString();
                Value = domainHeader.header.ContainsKey("Name") ? domainHeader.header["Name"] : Domain;
                // if it has an icon, add it
                if (domainHeader.header.ContainsKey("Icon"))
                {
                    AddItem(new RasterSprite(position, RasterSprite.DEFAULT_PIXEL_SCALE, Vector2.One * 16 * RasterSprite.PIXEL_TO_SCREEN_RATIO * RasterSprite.DEFAULT_PIXEL_SCALE, domainHeader.header["Icon"]), false, false);
                }
                // add a vertical area (LayoutArea) for text
                textArea = new LayoutArea(screen, position, size);
                textArea.FlexibleHeight = true;
                textArea.FlexibleWidth = true;
                textArea.Margin = new Rectangle(1, 1, 1, 1);
                AddItem(textArea);
                // add name text
                textArea.AddItem(new TextSprite(position, new Vector2(200,30), Value), true, false);
                // add Horizontal layout area for the details
                HorizontalLayoutArea detailsArea = new HorizontalLayoutArea(screen, position, size);
                detailsArea.FlexibleHeight = true;
                detailsArea.FlexibleWidth = true;
                detailsArea.Margin = new Rectangle(1, 1, 1, 1);
                textArea.AddItem(detailsArea);
                // add details (domain id different from name), and the number of blocks in the domain (domain size)
                if(Domain != Value)
                {
                    detailsArea.AddItem(new TextSprite(position, new Vector2(60,20), Domain,Scale:0.75f), true, false);
                }
                detailsArea.AddItem(new TextSprite(position, new Vector2(80, 20), $"{GridDB.DomainSize(Domain)} blocks", Scale: 0.75f), true, false);
                if(border != null) border.Color = screen.AppStyle.BorderColor;
                this.Domain = Domain;
            }
            //-------------------------------------------------------------------
            // IInteractable implementation
            //-------------------------------------------------------------------
            public string Id { get; set; }

            public string Value { get; set; }

            public Vector2 Center
            {
                get
                {
                    return Position + (Size / 2);
                }
            }
            bool isFocused = false;
            public bool IsFocused
            {
                get
                {
                    return isFocused;
                }

                set
                {
                    isFocused = value;
                    if(border != null) border.Color = isFocused ? screen.AppStyle.HighlightTextColor : screen.AppStyle.BorderColor;
                }
            }
            public string KeyPrompt { get; set; }
            LayoutArea IInteractable.Area { get { return this; } }
            public Action<IInteractable> OnFocus { get; set; }
            public Action<IInteractable> OnBlur { get; set; }
            public Action<IInteractable> OnClick { get; set; }
            public Action LayoutChanged { get; set; }
            public Action<GridDBDomainItemLayout> OnDelete;
            public void Click()
            {
                FileActions = new LayoutMenu(screen, Position, new Vector2(MarginSize.X, 50));
                FileActions.FlexibleHeight = false; FileActions.FlexibleWidth = true;
                FileActions.AddItem(new HorizontalLayoutArea(screen, FileActions.MarginPosition, new Vector2(FileActions.MarginSize.X, 40))); // make it horizontal. will need to add menu items using DOM
                FileActions.AddMenuItem(new Vector2(90, 30), "Cancel", "Cancel",DOM:"0");
                FileActions.AddMenuItem(new Vector2(90, 30), "Delete", "Delete", DOM: "0");
                FileActions.OnClick += FileActionClick;
                textArea.AddItem(FileActions);
                Size += new Vector2(0, FileActions.Size.Y);
                LayoutChanged?.Invoke();
            }

            bool IInteractable.RunInput(GameInput input)
            {
                if(FileActions != null)
                {
                    FileActions.RunInput(input);
                    return true;
                }
                return false;
            }
            void FileActionClick(IInteractable source)
            {
                if(source.Id == "Delete")
                {
                    // delete the domain
                    GridDB.Delete(Domain);
                    OnDelete?.Invoke(this);
                }
                else if(source.Id == "Cancel")
                {
                    RemoveItem(FileActions);
                    Size -= new Vector2(0, FileActions.Size.Y);
                    LayoutChanged?.Invoke();
                    FileActions = null;
                }
            }
        }
    }
}
