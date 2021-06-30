using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using rtaNetworking.Streaming;
using System.Net;
using Microsoft.Owin.Hosting;

namespace rtaVideoStreamer
{
    public partial class Form1 : Form
    {

        private ImageStreamingServer _Server;
        private IDisposable _SignalR;

        public Form1()
        {
            InitializeComponent();
            this.linkLabel1.Text = string.Format("http://{0}:8080", Environment.MachineName);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string url = @"http://localhost:8081/";
            _SignalR = WebApp.Start<StartupSignalR>(url);
            _Server = new ImageStreamingServer();
            _Server.Start(8080);
        }

        private DateTime time = DateTime.MinValue;

        private void timer1_Tick(object sender, EventArgs e)
        {
            int count = (_Server.Clients != null) ? _Server.Clients.Count() : 0;

            this.sts.Text = "Clients: " + count.ToString();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Abre o navegador no link indicado!");
            //System.Diagnostics.Process.Start("firefox", this.linkLabel1.Text);
        }
    }
}
