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
        // WindowLayout
        //----------------------------------------------------------------------
        public class DownloadWindow : WindowLayout, IInteractable
        {
            //------------------------------------------------
            // fields
            //------------------------------------------------
            GridDBClient client;
            //DomainInfo domain;
            LayoutButton CloseBtn;
            Dictionary<string, TextSprite> downloadProgress = new Dictionary<string , TextSprite>();
            Queue<GridDBAddress> downloadBlocks = new Queue<GridDBAddress>();
            //Stack<GridDBAddress> failedBlocks = new Stack<GridDBAddress>();
            public float Progress
            {
                get
                {
                    if (downloadProgress.Count == 0) return 0f;
                    int done = downloadProgress.Values.Count(v => v.Data == "✓");
                    return (float)done / (float)downloadProgress.Count;
                }
            }
            public bool IsComplete
            {
                get
                {
                    return Progress >= 1f;
                }
            }
            //----------------------------------------------------------------------
            // constructor
            //----------------------------------------------------------------------
            public DownloadWindow(ScreenApp screen, Vector2 position, Vector2 size, GridDBClient client, DomainInfo domain, float borderWidth = 1) : base(screen, position, size, $"Downloading {domain.Domain}", borderWidth)
            {
                //this.domain = domain;
                this.client = client;
                LayoutArea contentArea = new LayoutArea(screen, MarginPosition, MarginSize);
                foreach (var block in domain.Blocks)
                {
                    HorizontalLayoutArea blockArea = new HorizontalLayoutArea(screen, MarginPosition, new Vector2(MarginSize.X, 20f), borderWidth: 0);
                    contentArea.AddItem(blockArea);
                    TextSprite statusLabel = new TextSprite(MarginPosition, new Vector2(20f, 20f), " ", "Monospace", 1, Color.Green);
                    downloadProgress[block.ToString()] = statusLabel;
                    downloadBlocks.Enqueue(block);
                    blockArea.AddItem(statusLabel,false);
                    TextSprite blockLabel = new TextSprite(MarginPosition, new Vector2(size.X - 20, 20f), $"{block}");
                    blockArea.AddItem(blockLabel);
                }
                CloseBtn = new LayoutButton(screen, MarginPosition, new Vector2(100, 30), "Close", "Close");
                contentArea.AddItem(CloseBtn);
                AddContent("Domains", contentArea);
                ApplyLayout();
                client.DataReceived += Client_DataReceived;
                client.RequestData(downloadBlocks.Peek());
                footer.KeyPromptText = $"{Progress* 100f:0.0}%";
                footer.StatusText = "Downloading...";
            }
            //----------------------------------------------------------------------
            // methods
            //----------------------------------------------------------------------
            private void Client_DataReceived(GridData data)
            {
                string addressStr = data.address.ToString();
                //GridInfo.Echo($"Received data for block {addressStr}, saving to GridDB...");
                //if (!downloadProgress.ContainsKey(data.address)) return;
                GridDB.Set(data.address, data.ToString(), true);
                //GridInfo.Echo($"Block {addressStr} saved to GridDB, verifying...");
                //fif(!downloadBlocks.Contains(data.address)) return;// already processed
                /*
                if (GridDB.Get(data.address) == "")
                {
                    GridInfo.Echo($"Failed to save block {data.address} to GridDB.");
                    downloadProgress[addressStr].Data = "x";
                    downloadProgress[addressStr].Color = Color.Red;
                    failedBlocks.Push(data.address);
                }
                else*/ downloadProgress[addressStr].Data = "✓";
                //GridInfo.Echo($"Block {addressStr} saved successfully.");
                downloadBlocks.Dequeue();
                if (downloadBlocks.Count > 0) client.RequestData(downloadBlocks.Peek());
                else footer.StatusText = "Done";
                footer.KeyPromptText = $"{Progress * 100f:0}%";
            }
            //------------------------------------------------
            // IInteractable implementation
            //------------------------------------------------
            public string Id { get; set; }
            public string Value { get; set; }
            public Vector2 Center { get { return MarginPosition + (MarginSize / 2f); } }
            public LayoutArea Area { get { return this; } }
            public bool IsFocused
            {
                get
                {
                    return CloseBtn.IsFocused;
                }

                set
                {
                    CloseBtn.IsFocused = value;
                }
            }
            public Action<IInteractable> OnFocus { get; set; }
            public Action<IInteractable> OnBlur { get; set; }
            public Action<IInteractable> OnClick { get; set; }
            public Action<IInteractable> OnBack { get; set; }
            public Action LayoutChanged { get; set; }
            public string KeyPrompt
            {
                get
                {
                    return KeyPromptText;
                }

                set
                {
                    KeyPromptText = value;
                }
            }
            public void Click()
            {

            }
            public bool RunInput(GameInput input)
            {
                if (input.EPressed)
                {
                    OnBack?.Invoke(this);
                }
                return true;
            }
        }
        //----------------------------------------------------------------------
    }
}