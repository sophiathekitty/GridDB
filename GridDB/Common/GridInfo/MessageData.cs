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
        // MessageData for interscript communication
        //----------------------------------------------------------------------
        public class MessageData
        {
            public const char KVPairSeparator = '╪'; // =
            public const char DataSeparator = '╫'; // ,
            public const string SENDER = "Sender";
            public const string TAG = "Tag";
            public const string ADDRESS = "Address";
            //------------------------------------------------------
            // static methods
            //------------------------------------------------------
            public static MessageData ParseMessage(string rawData)
            {
                if (string.IsNullOrEmpty(rawData)) return null;
                return new MessageData(rawData);
            }
            public static MessageData ParseMessage(MyIGCMessage msg)
            {
                MessageData md = ParseMessage(msg.Data as string);
                if (md != null)
                {
                    md[SENDER] = msg.Source.ToString();
                    md[TAG] = msg.Tag;
                }
                return md;
            }
            //------------------------------------------------------
            // fields
            //------------------------------------------------------
            protected Dictionary<string, string> data = new Dictionary<string, string>();
            public string Client // who is the client (if applicable)
            {
                get
                {
                    if (this["Client"] == null) return "";
                    return this["Client"];
                }
            }
            public long Sender // who sent the message
            {
                get
                {
                    if (this[SENDER] == null) return 0;
                    long s;
                    if (long.TryParse(this[SENDER], out s)) return s;
                    return 0;
                }
            }
            public string Tag // what kind of message
            {
                get
                {
                    if (this[TAG] == null) return "";
                    return this[TAG];
                }
            }
            public string Data // main data payload
            {
                get
                {
                    if (this["Data"] == null) return "";
                    return this["Data"];
                }
            }
            public string Address // seat address for ScreenAppSeats....
            {
                get
                {
                    if (this[ADDRESS] == null) return "";
                    return this[ADDRESS];
                }
            }
            public string this[string key] // indexer to get/set data values
            {
                get
                {
                    if (data.ContainsKey(key)) return data[key];
                    return null;
                }
                set
                {
                    data[key] = value;
                }
            }
            //------------------------------------------------------
            // constructor
            //------------------------------------------------------
            public MessageData(string rawData)
            {
                ParseData(rawData);
            }
            public MessageData() { }
            public MessageData(string tag, long sender)
            {
                this[TAG] = tag;
                this[SENDER] = sender.ToString();
            }
            public MessageData(string tag, long sender, string address, Dictionary<string, string> data)
            {
                this[TAG] = tag;
                this[SENDER] = sender.ToString();
                this[ADDRESS] = address;
                foreach (var kv in data)
                {
                    this[kv.Key] = kv.Value;
                }
            }
            //------------------------------------------------------
            // parse data
            //------------------------------------------------------
            public void ParseData(string rawData)
            {
                data.Clear();
                if (string.IsNullOrEmpty(rawData)) return;
                if (!rawData.Contains(KVPairSeparator) && !rawData.Contains(KVPairSeparator))
                {
                    data["Data"] = rawData;
                    return;
                }
                string[] parts = rawData.Split(new char[] { DataSeparator }, StringSplitOptions.RemoveEmptyEntries); // ,
                foreach (string part in parts)
                {
                    string[] kv = part.Split(new char[] { KVPairSeparator }, StringSplitOptions.RemoveEmptyEntries); // =
                    if (kv.Length == 2)
                    {
                        data[kv[0]] = kv[1];
                    }
                }
            }
            public bool HasKey(string key)
            {
                return data.ContainsKey(key);
            }
            //------------------------------------------------------
            // serialize data
            //------------------------------------------------------
            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                foreach (var kv in data)
                {
                    if (sb.Length > 0) sb.Append(DataSeparator); // ,
                    sb.Append(kv.Key);
                    sb.Append(KVPairSeparator); // =
                    sb.Append(kv.Value);
                }
                return sb.ToString();
            }
            public MyIGCMessage ToIGCMessage()
            {
                StringBuilder sb = new StringBuilder();
                foreach (var kv in data)
                {
                    if (kv.Key == TAG || kv.Key == SENDER) continue; // skip these (they are in the message header
                    if (sb.Length > 0) sb.Append(DataSeparator); // ,
                    sb.Append(kv.Key);
                    sb.Append(KVPairSeparator); // =
                    sb.Append(kv.Value);
                }
                return new MyIGCMessage(sb.ToString(), Tag, Sender);
            }
        }
        //------------------------------------------------------
    }
}
