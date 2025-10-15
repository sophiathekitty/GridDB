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
        // ProgScreen - main program screen
        //----------------------------------------------------------------------
        public class ProgScreen : Screen
        {
            //----------------------------------------------------------------------
            // static fields
            //----------------------------------------------------------------------
            static ProgScreen Info;
            static int updateCooldown = 1000;
            static int updateCD;
            //----------------------------------------------------------------------
            // Init
            //----------------------------------------------------------------------
            public static void Init()
            {
                Info = new ProgScreen(GridInfo.Me.GetSurface(0));
                GridInfo.AddMainLoop(Info.Main);
            }
            //----------------------------------------------------------------------
            // fields
            //----------------------------------------------------------------------
            TextSprite title;
            TextSprite info;
            //----------------------------------------------------------------------
            // constructor
            //----------------------------------------------------------------------
            public ProgScreen(IMyTextSurface drawingSurface) : base(drawingSurface)
            {
                //GridInfo.Echo("ProgScreen init");
                title = new TextSprite(Vector2.One * 10, new Vector2(200, 50),"Grid DB: Local");
                info = new TextSprite(new Vector2(10,50), new Vector2(400, 400), "");
                AddSprite(title);
                AddSprite(info);
            }
            //----------------------------------------------------------------------
            // update
            //----------------------------------------------------------------------
            public override void Update(string argument)
            {
                if (updateCD > 0)
                {
                    updateCD--;
                    return;
                }
                updateCD = updateCooldown;
                info.Text = GridDB.Info;
                //GridInfo.Echo(info.Text);
                base.Update(argument);
            }
        }
        //----------------------------------------------------------------------
    }
}
