using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin.Cors;
using Owin;
using rtaNetworking.Streaming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rtaVideoStreamer
{
    public class StartupEpicoHub
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
        }
    }

    public class Broadcaster
    {
        private readonly static Lazy<Broadcaster> _instancia = new Lazy<Broadcaster> (() => new Broadcaster());
        private readonly IHubContext _hubContexto;

        public Broadcaster()
        {
            _hubContexto = GlobalHost.ConnectionManager.GetHubContext<EpicoSPAHub>();
        }
       
        public static Broadcaster Instancia
        {
            get
            {
                return _instancia.Value;
            }
        }
    }

    [HubName("EpicoSPA_Hub")]
    public class EpicoSPAHub : Hub<IEpicoSPAHub>
    {
        private Broadcaster _broadcaster;
        public EpicoSPAHub()
            : this(Broadcaster.Instancia)
        {
        }

        public EpicoSPAHub(Broadcaster broadcaster)
        {
            _broadcaster = broadcaster;
        }


        public void MouseClick(string connectionId, int X, int Y)
        {
            Screen.Epico.Camera.DispararMouseClick(X, Y);
        }
        public void MouseDown(string connectionId, int X, int Y)
        {
            Screen.Epico.Camera.DispararMouseDown(X, Y);
        }
        public void MouseUp(string connectionId, int X, int Y)
        {
            Screen.Epico.Camera.DispararMouseUp(X, Y);
        }
    }

    public interface IEpicoSPAHub
    {

    }
}
