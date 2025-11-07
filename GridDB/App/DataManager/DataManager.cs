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
            DownloadWindow downloadWindow;
            LayoutArea filler;
            IInteractable selectedItem;
            Action<string> updateInfo;
            GridDBClient dBClient = null;
            string focusedMenuId = "";
            const string mainMenuId = "SideMenu";
            const string fillerId = "Filler";
            //----------------------------------------------------------------------
            // constructor
            //----------------------------------------------------------------------
            public DataManager(ScreenAppSeat seat) : base(seat, "Data Manager")
            {
                // layout
                //KeyPrompt = "Q: Launcher Hub  |  Space: Change Domain  |  A: Shrink Window";
                AppStyle.HeaderColor = Color.DarkSeaGreen * 0.85f;
                AppStyle.FooterColor = Color.DarkSlateGray * 0.75f;
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
                dBClient.DomainInfoReceived += RemoteDomainListReceived;
                dBClient.DomainAddressReceived += DomainBlocksRecieved;
            }
            //----------------------------------------------------------------------
            // main update loop
            //----------------------------------------------------------------------
            public override void Update(string argument)
            {
                if (selectedItem != null)
                {
                    //GridInfo.Echo("DataManager Update: selected item " + selectedItem.Id);
                    selectedItem.RunInput(input);
                    input.Reset();
                }
                base.Update(argument);
                updateInfo?.Invoke(GridDB.Info);
                if(dBClient.LoadingDomains) Status = $"Connecting... {dBClient.Host.HostName}...";
                else if(dBClient.DownloadingAddress != "") Status = $"Downloading {dBClient.DownloadingAddress}...";
                else if(dBClient.Connected)
                {
                    if (dBClient.HasDomains) Status = $"{dBClient.Host.HostName} | Domains: {dBClient.Host.Domains.Count}";
                    else Status = $"{dBClient.Host.HostName} | None";
                }
                else Status = $"Used: {GridDB.UsedPercent.ToString("0.0")}% | Hosts: {GridDBClient.GridDBHosts.Count}";
            }
            //----------------------------------------------------------------------
            // handle menu click events
            //----------------------------------------------------------------------
            void MenuItemClicked(IInteractable item)
            {
                //GridInfo.Echo("DataManager MenuItemClicked: " + item.Id);
                if (item.Id == "close")
                {
                    seat.CurrentAppId = new ScreenAppId("LauncherHub", "GameEditor");
                }
                else if (item.Id == "local")
                {
                    // add local menu
                    focusedMenuId = "LocalMenu";
                    contentArea.RemoveContent(fillerId);
                    DomainListLayout domainList = new DomainListLayout(this, contentArea.MarginPosition, new Vector2(contentArea.MarginSize.X - 220, contentArea.MarginSize.Y));
                    contentArea.AddContent(focusedMenuId, domainList, mainMenuId);
                    contentArea.ApplyLayout();
                    selectedItem = domainList;
                    selectedItem.IsFocused = true;
                    //domainList.OnClick += MenuItemClicked;
                    domainList.LayoutChanged += () => { contentArea.ApplyLayout(); };
                    domainList.OnBack += OnBack;
                }
                else if (item.Id == "remote")
                {
                    // add host list menu... is regular menu like main side menu (no flex width)
                    focusedMenuId = "RemoteMenu";
                    GridDBHostList hostList = new GridDBHostList(this, contentArea.MarginPosition, new Vector2(220, contentArea.MarginSize.Y), dBClient);
                    contentArea.AddContent(focusedMenuId, hostList, mainMenuId);
                    contentArea.ApplyLayout();
                    selectedItem = hostList;
                    selectedItem.IsFocused = true;
                    //hostList.OnClick += RemoteHostListClicked;
                    hostList.LayoutChanged += () => { contentArea.ApplyLayout(); };
                    hostList.OnBack += OnBack;
                }
                else if (item.Id == "Download")
                {
                    //GridInfo.Echo($"DataManager downloading domain {item.Value}...");
                    // but need to get blocks...
                    dBClient.RequestDomainBlocks(item.Value);
                }
            }
            void RemoteDomainListReceived(List<DomainInfo> source)
            {
                //GridInfo.Echo("DataManager RemoteDomainListReceived: " + source.Count);
                // remove remote host menu and add domain list menu
                IInteractable layoutMenu = contentArea[focusedMenuId] as IInteractable;
                //layoutMenu.OnClick -= RemoteHostListClicked;
                contentArea.RemoveContent(focusedMenuId);
                contentArea.RemoveContent(fillerId);
                focusedMenuId = "RemoteDomainList";
                RemoteDomainListLayout domainList = new RemoteDomainListLayout(this, contentArea.MarginPosition, new Vector2(contentArea.MarginSize.X - 220, contentArea.MarginSize.Y), source);
                contentArea.AddContent(focusedMenuId, domainList, mainMenuId);
                contentArea.ApplyLayout();
                selectedItem = domainList;
                selectedItem.IsFocused = true;
                selectedItem.OnClick += MenuItemClicked;
                selectedItem.LayoutChanged += () => { contentArea.ApplyLayout(); };
                selectedItem.OnBack += OnBack;
            }
            void DomainBlocksRecieved(DomainInfo domain, List<GridDBAddress> blocks)
            {
                GridInfo.Echo("DataManager DomainBlocksRecieved: " + blocks.Count);
                downloadWindow = new DownloadWindow(this, Size * 0.1f, Size * 0.8f, dBClient, domain);
                selectedItem.IsFocused = false;
                selectedItem = downloadWindow;
                selectedItem.IsFocused = true;
                downloadWindow.OnBack += CloseWindow;
            }
            void OnBack(IInteractable source)
            {
                // go back to main menu
                IInteractable layoutMenu = contentArea[focusedMenuId] as IInteractable;
                layoutMenu.OnClick -= MenuItemClicked;
                contentArea.RemoveContent(focusedMenuId);
                contentArea.AddContent(fillerId, filler, mainMenuId);
                contentArea.ApplyLayout();
                selectedItem = contentArea[mainMenuId] as LayoutMenu;
                selectedItem.IsFocused = true;
                focusedMenuId = "";
            }
            void CloseWindow(IInteractable source)
            {
                GridInfo.Echo("DataManager CloseWindow");
                if (selectedItem == downloadWindow)
                {
                    // close download window
                    downloadWindow.RemoveFromScreen();
                    selectedItem = contentArea[focusedMenuId] as IInteractable;
                    selectedItem.IsFocused = true;
                    downloadWindow = null;
                }
            }
        }
        //----------------------------------------------------------------------
    }
}
