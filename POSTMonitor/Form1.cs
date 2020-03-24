using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Fiddler;

namespace POSTMonitor
{
    public partial class Form1 : Form
    {
        delegate void update();
        public Form1()
        {
            InitializeComponent();
        }

        private void startFiddler()
        {
            FiddlerApplication.AfterSessionComplete += FiddlerApplication_AfterSessionComplete; ;
            FiddlerApplication.Startup(8888, true, true, true);
            installCertificate();
        }

        private bool installCertificate()
        {
            if (!CertMaker.rootCertExists())
            {
                if (!CertMaker.createRootCert())
                    return false;

                if (!CertMaker.trustRootCert())
                    return false;

                FiddlerApplication.Prefs.SetStringPref("fiddler.certmaker.bc.key", FiddlerApplication.Prefs.GetStringPref("fiddler.certmaker.bc.key", null));
                FiddlerApplication.Prefs.SetStringPref("fiddler.certmaker.bc.cert", FiddlerApplication.Prefs.GetStringPref("fiddler.certmaker.bc.cert", null));
            }
            return true;
        }

        private void FiddlerApplication_AfterSessionComplete(Session session)
        {
            string output = "";

            if (session.RequestMethod == "POST")
            {
                string headers = session.oRequest.headers.ToString();
                var reqBody = session.GetRequestBodyAsString();

                string firstLine = session.RequestMethod + " " + session.fullUrl + " " + session.oRequest.headers.HTTPVersion;
                int at = headers.IndexOf("\r\n");
                if (at < 0)
                    return;
                headers = firstLine + "\r\n" + headers.Substring(at + 1);
                string separator = new String('-', 200);

                output = headers + "\r\n" +
                         (!string.IsNullOrEmpty(reqBody) ? reqBody + "\r\n" : string.Empty) +
                         separator + "\r\n\r\n";
            }

            richTextBox.Invoke(new update(() =>
            {
                richTextBox.AppendText(output);
            }));
        }

        private void stopFiddler()
        {
            FiddlerApplication.AfterSessionComplete -= FiddlerApplication_AfterSessionComplete;
            FiddlerApplication.Shutdown();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            startFiddler();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            stopFiddler();
        }

        private void richTextBox_TextChanged(object sender, EventArgs e)
        {
            richTextBox.SelectionStart = richTextBox.Text.Length;
            richTextBox.ScrollToCaret();
        }
    }
}
