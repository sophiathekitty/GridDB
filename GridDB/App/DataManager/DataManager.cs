using EmptyKeys.UserInterface.Generated.EditFactionIconView_Bindings;
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
        // DataManager - application screen for managing stored data
        //----------------------------------------------------------------------
        public class DataManager : ScreenApp
        {
            public static void Init()
            {
                //GridInfo.Echo("DataManager init");
                // static init
                RegisterApp();
                ScreenAppSeat.GetSeat("Test.Seat", "Data Manager", "Data Manager");
            }
            static void RegisterApp()
            {
                AvailableApps["Data Manager"] = LaunchApp;
            }
            static void LaunchApp(ScreenAppSeat seat)
            {
                new DataManager(seat);
            }
            //----------------------------------------------------------------------
            // fields
            //----------------------------------------------------------------------
            //TextSprite header;
            AppLayout contentArea;
            LayoutArea filler;
            IInteractable selectedItem;
            Action<string> updateInfo;
            GridDBClient dBClient = null;
            string focusedMenuId = "";
            //----------------------------------------------------------------------
            // constructor
            //----------------------------------------------------------------------
            public DataManager(ScreenAppSeat seat) : base(seat, "Data Manager")
            {
                // layout
                KeyPrompt = "Q: Launcher Hub  |  Space: Change Domain  |  A: Shrink Window";
                contentArea = new AppLayout(this,Vector2.Zero, Size, Color.DarkSeaGreen * 0.65f, Color.DarkSlateGray * 0.5f);
                // quick side menu test
                LayoutMenu sideMenu = new LayoutMenu(this, contentArea.MarginPosition, new Vector2(200, contentArea.MarginSize.Y), "GridDB", new TextureSprite(contentArea.MarginPosition, new Vector2(200, contentArea.MarginSize.Y), "SquareSimple", Color.DimGray * 0.4f), 1);
                sideMenu.FlexibleHeight = true;
                sideMenu.FlexibleWidth = false;
                contentArea.AddContent("SideMenu", sideMenu);
                sideMenu.AddMenuItem(new Vector2(130, 40), "Local Data", "local");
                sideMenu.AddMenuItem(new Vector2(130, 30), "IGC Data", "remote");
                sideMenu.AddMenuItem(new Vector2(130, 30), "Close", "close");
                sideMenu.OnClick += MenuItemClicked;
                selectedItem = sideMenu;
                sideMenu.IsFocused = true;
                GridDBInfo infoArea = new GridDBInfo(this, contentArea.MarginPosition,  new Vector2(300, 300), ref updateInfo, Color.DimGray * 0.4f, 1);
                // add an empty item to force layout to expand
                filler = new LayoutArea(this, contentArea.MarginPosition, new Vector2(0, 0));
                filler.FlexibleWidth = true; filler.FlexibleHeight = true;
                contentArea.AddContent("Filler", filler);
                contentArea.AddContent("InfoArea", infoArea);
                contentArea.ApplyLayout();
                dBClient = new GridDBClient("DatManCli");
            }
            //----------------------------------------------------------------------
            // main update loop
            //----------------------------------------------------------------------
            public override void Update(string argument)
            {
                /*
                string txt = "";
                if (input.IsValid)
                {
                    if (input.SpaceReleased)
                    {
                        txt += " (Space Released)";
                        //seat.CurrentApp = "WebBrowser";
                        Domain = "Space Bar";
                        LayoutArea localSubMenu = new LayoutArea(this, contentArea.MarginPosition, new Vector2(200, contentArea.MarginSize.Y), new TextureSprite(contentArea.MarginPosition, new Vector2(190, contentArea.MarginSize.Y), "SquareSimple", Color.DimGray * 0.4f), 1);
                        localSubMenu.FlexibleHeight = true;
                        localSubMenu.FlexibleWidth = false;
                        contentArea.AddContent("LocalSubMenu", localSubMenu, "SideMenu");
                        localSubMenu.AddItem(new TextSprite(localSubMenu.MarginPosition, new Vector2(170, 40), "Local Data", Scale: 1.115f), flexibleHeight: false);
                        localSubMenu.AddItem(new TextSprite(localSubMenu.MarginPosition, new Vector2(170, 30), "Saves", Scale: 0.95f), flexibleHeight: false);
                        localSubMenu.AddItem(new TextSprite(localSubMenu.MarginPosition, new Vector2(170, 30), "Games", Scale: 0.95f), flexibleHeight: false);
                        localSubMenu.AddItem(new TextSprite(localSubMenu.MarginPosition, new Vector2(170, 30), "Delete", Scale: 0.95f), flexibleHeight: false);
                        localSubMenu.AddItem(new TextSprite(localSubMenu.MarginPosition, new Vector2(170, 30), "Back", Scale: 0.95f), flexibleHeight: false);
                        contentArea.ApplyLayout();
                    }
                    if(input.CReleased)
                    {
                        txt += " (C Released)";
                        contentArea.RemoveContent("LocalSubMenu");
                        contentArea.ApplyLayout();
                    }
                    if (input.AReleased)
                    {
                        contentArea.Size -= Vector2.One * 50;
                        FileIndex++;
                    }
                    if (input.QReleased)
                    {
                        txt += " (Q Released)";
                        seat.CurrentAppId = new ScreenAppId("LauncherHub","GameEditor");
                    }
                    input.Reset();
                }
                */
                if (selectedItem != null)
                {
                    //GridInfo.Echo("DataManager Update: selected item " + selectedItem.Id);
                    selectedItem.RunInput(input);
                    input.Reset();
                }
                base.Update(argument);
                updateInfo?.Invoke(GridDB.Info);
                Status = $"Used: {GridDB.UsedCount}/{GridDB.TotalCount} ( {GridDB.UsedPercent.ToString("0.0")}% )";
            }
            //----------------------------------------------------------------------
            // handle menu click events
            //----------------------------------------------------------------------
            void MenuItemClicked(IInteractable item)
            {
                GridInfo.Echo("DataManager MenuItemClicked: " + item.Id);
                if (item.Id == "close")
                {
                    seat.CurrentAppId = new ScreenAppId("LauncherHub", "GameEditor");
                }
                else if (item.Id == "local")
                {
                    // add local menu
                    contentArea.RemoveContent("Filler");
                    DomainListLayout domainList = new DomainListLayout(this, contentArea.MarginPosition, new Vector2(contentArea.MarginSize.X - 220, contentArea.MarginSize.Y));
                    contentArea.AddContent("LocalMenu", domainList, "SideMenu");
                    contentArea.ApplyLayout();
                    selectedItem = domainList;
                    selectedItem.IsFocused = true;
                    domainList.OnClick += MenuItemClicked;
                    domainList.LayoutChanged += () => { contentArea.ApplyLayout(); };
                    domainList.OnBack += OnBack;
                    focusedMenuId = "LocalMenu";
                }
                else if (item.Id == "remote")
                {
                    // uh... load remove data?
                }
            }
            void OnBack(IInteractable source)
            {
                // go back to main menu
                IInteractable layoutMenu = contentArea[focusedMenuId] as IInteractable;
                layoutMenu.OnClick -= MenuItemClicked;
                contentArea.RemoveContent(focusedMenuId);
                contentArea.AddContent("Filler", filler, "SideMenu");
                contentArea.ApplyLayout();
                selectedItem = contentArea["SideMenu"] as LayoutMenu;
                selectedItem.IsFocused = true;
                focusedMenuId = "";
            }
        }
        //----------------------------------------------------------------------
    }
}
