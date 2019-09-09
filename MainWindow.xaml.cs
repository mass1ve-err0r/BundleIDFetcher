using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;


namespace BundleIDFetcher
{

    public partial class MainWindow : Window
    {
        ///////////////////////////////////////////////////////////////////////
        //                             GLOBAL VARS                           //
        ///////////////////////////////////////////////////////////////////////


        // <-- m4gic -->
        private OpenFileDialog BatchBIDGetter;
        public string iTunesAPIBase = "http://itunes.apple.com/lookup?bundleId=";
        public string BundleID, BundleIDJSONString, BundleIDURL, BundleIDJSONString2, BundleIDURL2;


        // <-- PATHS -->
        // Download Destination
        public string DownloadDestination = Directory.GetCurrentDirectory() + "\\_DownloadedImages";
        // File with BundleIDs
        public string BIDListFilePath;


        // <-- FLAGS -->
        // check if BundleID is set
        public bool Flag_BundleIDSet = false;
        // check if Icon was found
        public bool Flag_FoundIcon = false;


        // <-- DATA -->
        // store missed/ faulty BIDs
        public List<string> MissedBIDs = new List<string>();
        public int total = 0;
        public int missed = 0;

        ///////////////////////////////////////////////////////////////////////
        //                           MAIN FUNCTIONS                          //
        ///////////////////////////////////////////////////////////////////////


        // Entry
        public MainWindow()
        {
            InitializeComponent();
            // set our button here
            SearchButton.Click += MainAction;
            BatchButton.Click += BatchGetImages;
        }


        // Button Action
        void MainAction(object sender, RoutedEventArgs e)
        {
            // set BID & call methods
            BundleID = BundleIDTB.Text;
            CreateStringFromJSON();
            // check if BID is properly set
            if (Flag_BundleIDSet)
            {
                ExtractImage();
                // check if any Icon at all was found
                if (Flag_FoundIcon)
                {
                    // success message lul
                    System.Windows.MessageBox.Show("Application Icon fetched.", "BundleIDFetcher - SUCCESS");
                }
            }
            else
            {
                System.Windows.MessageBox.Show("No BundleID set!", "BundleIDFetcher - ERROR");
            }
        }


        // Create one bigass String from JSON-Object because we fetch exactly 1 or 0 items.
        void CreateStringFromJSON()
        {
            // just using the unary operator because style
            if (!(string.IsNullOrWhiteSpace(BundleIDTB.Text)))
            {
                // run as task to avoid overlaps
                Task t = Task.Run(() => {
                    // create a WebClient
                    var CreateStringFromJSON_wc = new WebClient();
                    // save the entire JSON-data as String
                    BundleIDJSONString = CreateStringFromJSON_wc.DownloadString(iTunesAPIBase + BundleID);
                    // flip the Flag
                    Flag_BundleIDSet = true;
                });
                t.Wait();
            }
            else
            {
                Flag_BundleIDSet = false;
            }
        }


        // Smart Extraction thanks to Regexes
        void ExtractImage()
        {
            Task ExtractImage_t = Task.Run(() =>
            {
                // Create two logics: Check for resultCount
                Regex CounterRegex = new Regex(@"(""resultCount""):(\d),");
                // & for stripping URL
                Regex StripRegex = new Regex(@"(""artworkUrl512""):""(.*?)""");
                // check if the counter is 1 (1 = App found)
                Match CounterMatch = CounterRegex.Match(BundleIDJSONString);
                if (CounterMatch.Groups[2].Value.Equals("1"))
                {
                    // start other Match to strip URL
                    Match StripMatch = StripRegex.Match(BundleIDJSONString);
                    // save URL from artwork512 | remember: idx rules w/ Match
                    BundleIDURL = StripMatch.Groups[2].Value;
                    // spawn webclient again and invoke DownloadFile
                    WebClient ExtractImageURL_wc = new WebClient();
                    // Create dir & save the image including correct formatting
                    Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\_DownloadedImages");
                    ExtractImageURL_wc.DownloadFile(BundleIDURL, DownloadDestination + "\\" + BundleID + "-large.jpg");
                    Flag_FoundIcon = true;
                }
                else
                {
                    // in case of error/ no icon found for BID
                    System.Windows.MessageBox.Show("Entered BundleID has no Application associated!", "BundleIDFetcher - ERROR");
                    Flag_FoundIcon = false;
                }
            });
            ExtractImage_t.Wait();
        }


        // Batch-Get Images | List Format: <BundleID>\n
        void BatchGetImages(object sender, RoutedEventArgs e)
        {
            // Generate FileOpenerDialog
            BatchBIDGetter = new OpenFileDialog();
            BatchBIDGetter.Title = "Select (Text-)File with BundleIDs";
            BatchBIDGetter.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            if (BatchBIDGetter.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                BIDListFilePath = BatchBIDGetter.FileName;
                // debug-- Console.WriteLine(BundleIDsListFile);
            }
            // Check if BIDListFilePath was set & safety-valve for wrong file extensions
            if (string.IsNullOrWhiteSpace(BIDListFilePath) |  (!BIDListFilePath.EndsWith(".txt")))
            {
                // string is empty/ is anything else than .txt, terminate with error
                System.Windows.MessageBox.Show("No file or invalid file selected!", "BundleIDFetcher -> BatchGet - ERROR");
            }
            else
            {
                // store the BundleIDs from file into array
                string[] BundleIDArray = File.ReadLines(BIDListFilePath).ToArray();
                // store the size
                total = BundleIDArray.Length;
                // debug
                foreach (string item in BundleIDArray)
                {
                    Console.WriteLine(item);
                }
                // construct for-each loop and cycle through each BID
                foreach (string BID in BundleIDArray)
                {
                    CreateStringFromJSON2(BID);
                    ExtractImage2(BID);
                }
                // Generate total message
                string local_out = "Done!\nTotal BIDs: " + total + "\nFaulty/ NoImageFound for BIDs: " + missed;
                System.Windows.MessageBox.Show(local_out, "BundleIDFetcher -> BatchGet - RESULT");
                // show missed BundleIDs
                var message = string.Join(Environment.NewLine, MissedBIDs);
                System.Windows.MessageBox.Show(message, "BundleIDFetcher -> BatchGet - NO IMAGE FOUND FOR THESE BIDs:");
                // Free allocated shit from list
                MissedBIDs.Clear();
                missed = 0;
                total = 0;
                // fin.
            }
            // fin.
        }

        ///////////////////////////////////////////////////////////////////////
        //                           HELPER FUNCTIONS                        //
        ///////////////////////////////////////////////////////////////////////


        // Silent-variants of the Main-Functions because I couldnt be bothered with another switch
        void CreateStringFromJSON2(string given_bid)
        {
            if (!(string.IsNullOrWhiteSpace(given_bid)))
            {
                Task t = Task.Run(() => {
                    var CreateStringFromJSON_wc = new WebClient();
                    BundleIDJSONString2 = CreateStringFromJSON_wc.DownloadString(iTunesAPIBase + given_bid);
                    Flag_BundleIDSet = true;
                });
                t.Wait();
            }
            else
            {
                Flag_BundleIDSet = false;
            }
        }
        void ExtractImage2(string given_bid)
        {
            Task ExtractImage_t = Task.Run(() =>
            {
                Regex CounterRegex = new Regex(@"(""resultCount""):(\d),");
                Regex StripRegex = new Regex(@"(""artworkUrl512""):""(.*?)""");
                Match CounterMatch = CounterRegex.Match(BundleIDJSONString2);
                if (CounterMatch.Groups[2].Value.Equals("1"))
                {
                    Match StripMatch = StripRegex.Match(BundleIDJSONString2);
                    BundleIDURL2 = StripMatch.Groups[2].Value;
                    WebClient ExtractImageURL_wc = new WebClient();
                    Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\_DownloadedImages");
                    ExtractImageURL_wc.DownloadFile(BundleIDURL2, DownloadDestination + "\\" + given_bid + "-large.jpg");
                    Flag_FoundIcon = true;
                }
                else
                {
                    // in case of error/ no image for BID found
                    MissedBIDs.Add(given_bid);
                    missed++;
                    Flag_FoundIcon = false;
                }
            });
            ExtractImage_t.Wait();
        }


    }
    // <-- FIN -->
}
