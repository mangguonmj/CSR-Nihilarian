using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSR;
using Fleck;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Nihilarian
{
    public class websocketopen
    {
        public static void Nihilarian(MCCSAPI api)
        {
            if(!File.Exists("./plugins/Nihilarian/config.json"))
            {
                JObject json = new JObject();
                json.Add(new JProperty("password", "114514"));
                json.Add(new JProperty("Port", 8081));
                Directory.CreateDirectory("./plugins/Nihilarian/");
                File.WriteAllText("./plugins/Nihilarian/config.json", json.ToString());
            }
            var cfg = JsonConvert.DeserializeObject<config>(File.ReadAllText("./plugins/Nihilarian/config.json"));
            var ws = new WebSocketServer("ws://127.0.0.1:"+cfg.port);
            var allSockets = new List<IWebSocketConnection>();
            bool runcmdfeelback = false;
            void sendMessage(pack p)
            {
                foreach (var item in allSockets)
                {
                    if (item.IsAvailable == true)
                    {
                        item.Send(JsonConvert.SerializeObject(p));
                    }
                }
            }            
            new Thread(() =>
            {               
                ws.Start(socket =>
                {
                    socket.OnOpen = () =>
                    {
                        Console.WriteLine("Open!");
                        allSockets.Add(socket);
                    };
                    socket.OnClose = () =>
                    {
                        Console.WriteLine("Close!");
                        allSockets.Remove(socket);
                    };
                    socket.OnMessage = message =>
                    {
                        var json = JsonConvert.DeserializeObject<pack>(message);
                        if(json.password == cfg.password)
                        {
                            if (json.@event == "Runcmd")
                            {
                                runcmdfeelback = true;
                                api.runcmd(json.message);
                            }
                            if (json.@event == "Message")
                            {
                                api.runcmd($"tellarw @a {{\"rawtext\":[{{\"text\":\"{json.message}\"}}]}}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("password error!!");
                        }
                        //allSockets.ToList().ForEach(s => s.Send("ClientIP: " + s.ConnectionInfo.ClientIpAddress + " ClientPort: " + s.ConnectionInfo.ClientPort + "This client send: " + message));
                    };
                });
            }).Start();
            api.addBeforeActListener(EventKey.onInputText, x =>
             {
                 var a = BaseEvent.getFrom(x) as InputTextEvent;
                 var pack = new pack("onChat")
                 {
                     message = a.msg,
                     sender = a.playername
                 };
                 sendMessage(pack);
                 return true;
             });
            api.addBeforeActListener(EventKey.onPlayerLeft, x =>
             {
                 var a = BaseEvent.getFrom(x) as PlayerLeftEvent;
                 var pack = new pack("onPlayerLeft")
                 {
                     sender = a.playername
                 };
                 sendMessage(pack);
                 return true;
             });
            api.addBeforeActListener(EventKey.onLoadName, x =>
             {
                 var a = BaseEvent.getFrom(x) as LoadNameEvent;
                 var pack = new pack("onLoadName")
                 {
                     sender = a.playername
                 };
                 sendMessage(pack);
                 return true;
             });
            api.addBeforeActListener(EventKey.onServerCmdOutput, x =>
            {
                var a = BaseEvent.getFrom(x) as ServerCmdOutputEvent;
                if (runcmdfeelback)
                {
                    var pack = new pack("RuncmdFeelBack")
                    {
                        message = a.output
                    };
                    sendMessage(pack);
                    runcmdfeelback = false;
                    return runcmdfeelback;
                }
                return true;
            });
        }
    }
}
namespace CSR
{
    partial class Plugin
    {
        public static void onStart(MCCSAPI api)
        {
            try
            {
                Nihilarian.websocketopen.Nihilarian(api);
                Console.WriteLine("[Nihilarian] 装载成功");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
