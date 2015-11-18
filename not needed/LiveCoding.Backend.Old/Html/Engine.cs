using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using HtmlAgilityPack;
using Frm = System.Windows.Forms;
using System.Net.Http;


namespace LiveCoding.Backend.Html {
    public class Engine : System.Windows.Forms.Form {

        #region Static
        private static bool DisposeAll;

        private static void DisposeAllChilds() {
            new Thread(new ThreadStart(() => { DisposeAll = true; Thread.Sleep(1000); DisposeAll = false; })).Start();
        }
        #endregion

        // Local Variables
        private LiveCoding.Backend.Engine baseEngine;
        private Thread websiteTicker;
        private bool isDisposing = false;
        private int WaitTime;


        // Variables for the thread
        public string _ticker_OverallDashboard = "UPDATE NOW";

        public Engine(LiveCoding.Backend.Engine baseEngine, int WaitTime) {
            this.baseEngine = baseEngine;
            this.WaitTime = WaitTime;
            //websiteTicker = new Thread(new ThreadStart(() => { ThreadTick(WaitTime, this); }));
            //websiteTicker.Start();

            ThreadTick(WaitTime, this);

            System.Diagnostics.Debug.WriteLine("Created HTMLEngine and started Parser Thread");
        }

        private void Report(Information.Report rep) {
            System.Diagnostics.Debug.WriteLine("HTMLEngine firing event to update Report");
            baseEngine.ThrowReportEvent(new Events.ReportEvent(rep));
        }

        public void Dispose() {
            this.isDisposing = true;
        }

        void ThreadTick(int SleepTime, Engine engine) {
            //HttpClient client = new HttpClient();
            Frm.WebBrowser wb = new System.Windows.Forms.WebBrowser();
            WebClient wc = new WebClient();
            System.IO.StreamReader sr;
            Information.Report report;

            do {
                report = new Information.Report();
                System.Diagnostics.Debug.WriteLine("Reparsing LC For changes");

                /* Get Dashboard Information */ {
                    //byte[] htmlResponse = client.GetByteArrayAsync("https://www.livecoding.tv/" + engine.baseEngine.User).Result;
                    //string overallDashboardHTML = Encoding.GetEncoding("utf-8").GetString(htmlResponse, 0, htmlResponse.Length);

                    System.Diagnostics.Debug.WriteLine("Downloading HTML");
                    string overallDashboardHTML = WBGetHTML(wb, "https://www.livecoding.tv/" + engine.baseEngine.User);
                    overallDashboardHTML = WebUtility.HtmlDecode(overallDashboardHTML);
                    bool recheckHTML = (overallDashboardHTML != engine._ticker_OverallDashboard);
                    System.Diagnostics.Debug.WriteLine("Downloaded HTML");
                    System.Diagnostics.Debug.WriteLine("Update Dashboard Information : " + recheckHTML);
                   

                    // EngineUpdate the dash information
                    if (recheckHTML) {
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(overallDashboardHTML);
                        HtmlNode tempNode;

                        tempNode = GetElementByClass(doc, "h1", "live-title ", 0);
                        if (tempNode != null)
                            report.dashboard_Title = tempNode.InnerText;

                        tempNode = GetElementByID(doc, "followers_count");
                        if (tempNode != null)
                            report.dashboard_Followers = tempNode.InnerText;

                        tempNode = GetElementByID(doc, "views_overall");
                        if (tempNode != null)
                            report.dashboard_ViewCount = tempNode.InnerText;

                        tempNode = GetElementByID(doc, "views_live");
                        if (tempNode != null)
                            report.dashboard_Viewers = tempNode.InnerText;

                        engine._ticker_OverallDashboard = overallDashboardHTML;
                    }
                }

                /* Get private Messages */ {
                    // Might do later
                }

                engine.Report(report);
                Thread.Sleep(SleepTime);
            } while (!engine.isDisposing || !DisposeAll);
        }
        public static HtmlNode GetElementByID(HtmlDocument doc, string ID) {
            var Array = doc.GetElementbyId(ID);
            if (Array != null)
                return Array;
            return null;
        }

        public static HtmlNode GetElementByClass(HtmlDocument doc, string Tag, string cssClass, int Index) {

            // -----
            string queryString = ("//div[contains(@class,'" + cssClass + "')]");
            HtmlNodeCollection dataArray = doc.DocumentNode.SelectNodes(queryString);

            var findclasses = doc.DocumentNode.Descendants().Where(
                d => d.Name.Contains(Tag) && d.Attributes.Contains("class")
            );

            List<HtmlNode> returnElements = new List<HtmlNode>();

            foreach (var element in findclasses) {
                foreach (var attr in element.Attributes) {
                    string tagClass = attr.Value;
                    bool containsClass = (tagClass == (cssClass));
                    System.Diagnostics.Debug.WriteLine("TAG: " + element.Name + ", CLASS: " + tagClass + Environment.NewLine);
                    

                    if (containsClass) {
                        returnElements.Add(element);
                    }
                }
            }

            if (returnElements.Count-1 < Index) {
                return null;
            }

            return returnElements[Index];
        }

        

        public static string WBGetHTML(Frm.WebBrowser wb, string site) {
            wb.ScriptErrorsSuppressed = true;
            wb.Navigate(site);
            WBWaitFor(wb);
            return wb.DocumentText;
        }

        public static void WBWaitFor(Frm.WebBrowser webBrControl) {
            Frm.WebBrowserReadyState loadStatus;
            int waittime = 100000;
            int counter = 0;
            while (true) {
                loadStatus = webBrControl.ReadyState;
                Frm.Application.DoEvents();
                if ((counter > waittime) || (loadStatus == Frm.WebBrowserReadyState.Uninitialized) || (loadStatus == Frm.WebBrowserReadyState.Loading) || (loadStatus == Frm.WebBrowserReadyState.Interactive)) {
                    break;
                }
                counter++;
            }

            counter = 0;
            while (true) {
                loadStatus = webBrControl.ReadyState;
                Frm.Application.DoEvents();
                if (loadStatus == Frm.WebBrowserReadyState.Complete && webBrControl.IsBusy != true) {
                    break;
                }
                counter++;
            }
        }

        public static HtmlNode GetElementByAttribute(HtmlDocument doc, string Tag, string Attribute, string AttrInfo, int Index) {
            return null;
        }
    }
}
