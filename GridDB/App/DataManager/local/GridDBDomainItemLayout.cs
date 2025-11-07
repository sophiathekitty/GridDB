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
            public static void Create(ScreenApp screen, Vector2 position, Vector2 size, string domain, Action<IInteractable> AddItem)
            {
                AddItem(new GridDBDomainItemLayout(screen, position, size, domain));
            }
            //-------------------------------------------------------------------
            // fields
            //-------------------------------------------------------------------
            bool isLocal = true;
            LayoutArea textArea;
            LayoutMenu FileActions;
            public string Domain { get; private set; }
            //-------------------------------------------------------------------
            // constructor
            //-------------------------------------------------------------------
            public GridDBDomainItemLayout(ScreenApp screen, Vector2 position, Vector2 size, string Domain, bool isLocal = true) : base(screen, position, size, new TextureSprite(position, size, "SquareSimple", Color.DarkSlateGray * 0.75f), 1)
            {
                this.Domain = Domain;
                this.isLocal = isLocal;
                Init();
                GridData domainHeader = new GridData(GridDB.GetDomainMainDataAddress(Domain), true);
                Id = domainHeader.address.ToString();
                Value = domainHeader.header.ContainsKey("Name") ? domainHeader.header["Name"] : Domain;
                if (domainHeader.header.ContainsKey("Icon")) AddIcon(domainHeader.header["Icon"]);
                // add a vertical area (LayoutArea) for text
                AddTextArea(screen, position, size, GridDB.DomainSize(Domain));

            }
            public GridDBDomainItemLayout(ScreenApp screen, Vector2 position, Vector2 size, DomainInfo domainInfo, bool isLocal = false) : base(screen, position, size, new TextureSprite(position, size, "SquareSimple", Color.DarkSlateGray * 0.75f), 1)
            {
                this.Domain = domainInfo.Domain;
                this.isLocal = isLocal;
                Init();
                Id = domainInfo.Domain;
                Value = domainInfo.Name != null ? domainInfo.Name : domainInfo.Domain;
                if (domainInfo.Icon != null) AddIcon(domainInfo.Icon);
                AddTextArea(screen, position, size,domainInfo.BlockCount);
            }
            void Init()
            {
                FlexibleHeight = false;
                FlexibleWidth = true;
                Margin = new Rectangle(1, 1, 1, 1);
            }
            void AddIcon(string iconData)
            {
                AddItem(new RasterSprite(Position, RasterSprite.DEFAULT_PIXEL_SCALE, Vector2.One * 16 * RasterSprite.PIXEL_TO_SCREEN_RATIO * RasterSprite.DEFAULT_PIXEL_SCALE, iconData), false, false);
            }
            void AddTextArea(ScreenApp screen, Vector2 position, Vector2 size, int blocks)
            {
                textArea = new LayoutArea(screen, position, size);
                textArea.FlexibleHeight = true;
                textArea.FlexibleWidth = true;
                textArea.Margin = new Rectangle(1, 1, 1, 1);
                AddItem(textArea);
                textArea.AddItem(new TextSprite(position, new Vector2(200, 30), Value), true, false);   // title
                // add Horizontal layout area for the details
                HorizontalLayoutArea detailsArea = new HorizontalLayoutArea(screen, position, size);
                detailsArea.FlexibleHeight = true;
                detailsArea.FlexibleWidth = true;
                detailsArea.Margin = new Rectangle(1, 1, 1, 1);
                textArea.AddItem(detailsArea);
                // add details (domain id different from name), and the number of blocks in the domain (domain size)
                if (Domain != Value)
                {
                    detailsArea.AddItem(new TextSprite(position, new Vector2(60, 20), Domain, Scale: 0.75f), true, false);
                }
                detailsArea.AddItem(new TextSprite(position, new Vector2(80, 20), $"{blocks} blocks", Scale: 0.75f), true, false);
                if (border != null) border.Color = screen.AppStyle.BorderColor;
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
            public Action<IInteractable> OnBack { get; set; }
            public Action LayoutChanged { get; set; }
            public Action<GridDBDomainItemLayout> OnDelete;
            public void Click()
            {
                if(FileActions != null) return; // already open
                FileActions = new LayoutMenu(screen, Position, new Vector2(MarginSize.X, 50));
                FileActions.FlexibleHeight = false; FileActions.FlexibleWidth = true;
                FileActions.AddItem(new HorizontalLayoutArea(screen, FileActions.MarginPosition, new Vector2(FileActions.MarginSize.X, 40))); // make it horizontal. will need to add menu items using DOM
                FileActions.AddMenuItem(new Vector2(90, 30), "Cancel", "Cancel",DOM:"0");
                if(isLocal) FileActions.AddMenuItem(new Vector2(90, 30), "Delete", "Delete", DOM: "0",button_value:Domain);
                else FileActions.AddMenuItem(new Vector2(90, 30), "Download", "Download", DOM: "0", button_value: Domain);
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
                    //GridDB.Delete(Domain);
                    OnDelete?.Invoke(this);
                    OnClick?.Invoke(source); // pass button on up the chain
                    return;
                }
                else if (source.Id == "Download")
                {
                    // download the domain
                    OnClick?.Invoke(source); // pass button on up the chain
                }
                RemoveItem(FileActions);
                Size -= new Vector2(0, FileActions.Size.Y);
                LayoutChanged?.Invoke();
                FileActions = null;
            }
        }
    }
}
