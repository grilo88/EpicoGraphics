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
    public class StartupSignalR
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
        }
    }

    public class Broadcaster
    {
        // Instância Singleton
        private readonly static Lazy<Broadcaster> _instancia = new Lazy<Broadcaster>(() => new Broadcaster());

        private readonly IHubContext _hubContexto;
        public Broadcaster()
        {
            _hubContexto = GlobalHost.ConnectionManager.GetHubContext<SPAHub>();

        }

        public static Broadcaster Instancia => _instancia.Value;
    }

    [HubName("EpicoSPA_Hub")]
    public class SPAHub : Hub<ISPACliente>
    {
        private Broadcaster _broadcaster;

        public SPAHub()
            : this(Broadcaster.Instancia)
        {
        }

        public SPAHub(Broadcaster broadcaster)
        {
            _broadcaster = broadcaster;
        }

        public override Task OnConnected()
        {
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }

        public void MouseDown(string connectionId, int X, int Y)
        {
            Screen.Epico.Camera.MouseDown(X, Y);
        }

        public void MouseUp(string connectionId, int X, int Y)
        {
            Screen.Epico.Camera.MouseUp(X, Y);
        }

        public void MouseClick(string connectionId, int X, int Y)
        {
            Screen.Epico.Camera.MouseClick(X, Y);
        }
    }

    public interface ISPACliente
    {

    }
}
