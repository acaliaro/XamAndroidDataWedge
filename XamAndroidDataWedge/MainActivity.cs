using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Xamarin.Essentials;

namespace XamAndroidDataWedge
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        ScanReceiver scanReceiver;

        TextView _textViewReadBarcode;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            _textViewReadBarcode = FindViewById<TextView>(Resource.Id.text_view_barcode_read);

            scanReceiver = new ScanReceiver(_textViewReadBarcode);
        }

        protected override void OnResume()
        {
            base.OnResume();

            EnableDataWedge();

            bool isUpdated = Preferences.Get("datawedge_updated", false);
            if (!isUpdated)
            {
                CreateProfile();
                UpdateProfile();

                Preferences.Set("datawedge_updated", true);
            }

            SwitchToProfile();

            // Register the broadcast receiver
            IntentFilter filterScan = new IntentFilter(ScanReceiver.IntentAction);
            filterScan.AddCategory(ScanReceiver.IntentCategory);
            RegisterReceiver(scanReceiver, filterScan);
        }

        protected override void OnPause()
        {
            UnregisterReceiver(scanReceiver);
            base.OnPause();
        }


        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            /*
            View view = (View) sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
                */

            Toggle();
                
        }

        static string PROFILE_NAME = "XamAndroidDataWedge";

        public static string ACTION_DATAWEDGE_FROM_6_2 = "com.symbol.datawedge.api.ACTION";
        public static string EXTRA_CREATE_PROFILE = "com.symbol.datawedge.api.CREATE_PROFILE";
        public static string EXTRA_ENABLE_DATAWEDGE = "com.symbol.datawedge.api.ENABLE_DATAWEDGE";
        public static string EXTRA_SWITCH_TO_PROFILE = "com.symbol.datawedge.api.SWITCH_TO_PROFILE";
        public static string EXTRA_SET_CONFIG = "com.symbol.datawedge.api.SET_CONFIG";
        public static string DATAWEDGE_EXTRA_KEY_SCANNER_TRIGGER_CONTROL = "com.symbol.datawedge.api.SOFT_SCAN_TRIGGER";
        public static string DATAWEDGE_EXTRA_VALUE_TOGGLE_SCANNER = "TOGGLE_SCANNING";

        private void EnableDataWedge()
        {
            Intent i = new Intent();
            i.SetAction(ACTION_DATAWEDGE_FROM_6_2);
            i.PutExtra(EXTRA_ENABLE_DATAWEDGE, true);
            SendBroadcast(i);
        }

        private void CreateProfile()
        {
            Intent i = new Intent();
            i.SetAction(ACTION_DATAWEDGE_FROM_6_2);
            i.PutExtra(EXTRA_CREATE_PROFILE, PROFILE_NAME);
            SendBroadcast(i);
        }

        private void SwitchToProfile()
        {
            Intent i = new Intent();
            i.SetAction(ACTION_DATAWEDGE_FROM_6_2);
            i.PutExtra(EXTRA_SWITCH_TO_PROFILE, PROFILE_NAME);
            SendBroadcast(i);
        }

        private void ProfileSetConfig(Bundle profileConfig)
        {
            Intent i = new Intent();
            i.SetAction(ACTION_DATAWEDGE_FROM_6_2);
            i.PutExtra(EXTRA_SET_CONFIG, profileConfig);
            SendBroadcast(i);
        }

        private void SendDataWedgeIntentWithExtra(String action, String extraKey, Bundle extras)
        {
            Intent dwIntent = new Intent();
            dwIntent.SetAction(action);
            dwIntent.PutExtra(extraKey, extras);
            SendBroadcast(dwIntent);
        }

        private void SendDataWedgeIntentWithExtra(String action, String extraKey, String extraValue)
        {
            Intent dwIntent = new Intent();
            dwIntent.SetAction(action);
            dwIntent.PutExtra(extraKey, extraValue);
            SendBroadcast(dwIntent);
        }

        private void Toggle()
        {
            Intent intent = new Intent();
            intent.SetAction(MainActivity.ACTION_DATAWEDGE_FROM_6_2);
            intent.PutExtra(MainActivity.DATAWEDGE_EXTRA_KEY_SCANNER_TRIGGER_CONTROL, MainActivity.DATAWEDGE_EXTRA_VALUE_TOGGLE_SCANNER);
            SendBroadcast(intent);
        }

        private void UpdateProfile()
        {
            Bundle profileConfig = new Bundle();
            profileConfig.PutString("PROFILE_NAME", PROFILE_NAME);
            profileConfig.PutString("PROFILE_ENABLED", "true"); //  Seems these are all strings
            profileConfig.PutString("CONFIG_MODE", "UPDATE");
            Bundle barcodeConfig = new Bundle();
            barcodeConfig.PutString("PLUGIN_NAME", "BARCODE");
            barcodeConfig.PutString("RESET_CONFIG", "true"); //  This is the default but never hurts to specify

            // QUI SI IMPOSTANO LE PROPRIETA' DELLO SCANNER
            Bundle barcodeProps = new Bundle();

            barcodeProps.PutString("scanner_input_enabled", "true");
            barcodeProps.PutString("scanner_selection", "auto"); //  Could also specify a number here, the id returned from ENUMERATE_SCANNERS.
                                                                 //  Do NOT use "Auto" here (with a capital 'A'), it must be lower case.
            barcodeProps.PutString("decoder_ean8", "true");
            barcodeProps.PutString("decoder_ean13", "true");
            barcodeProps.PutString("decoder_code39", "true");
            barcodeProps.PutString("decoder_code128", "true");
            barcodeProps.PutString("decoder_upca", "true");
            barcodeProps.PutString("decoder_upce0", "true");
            barcodeProps.PutString("decoder_upce1", "true");
            barcodeProps.PutString("decoder_d2of5", "true");
            barcodeProps.PutString("decoder_i2of5", "true");
            barcodeProps.PutString("decoder_aztec", "true");
            barcodeProps.PutString("decoder_pdf417", "true");
            barcodeProps.PutString("decoder_qrcode", "true");

            barcodeConfig.PutBundle("PARAM_LIST", barcodeProps);
            profileConfig.PutBundle("PLUGIN_CONFIG", barcodeConfig);

            Bundle appConfig = new Bundle();
            appConfig.PutString("PACKAGE_NAME", this.PackageName);      //  Associate the profile with this app
            appConfig.PutStringArray("ACTIVITY_LIST", new String[] { "*" });
            profileConfig.PutParcelableArray("APP_LIST", new Bundle[] { appConfig });
            SendDataWedgeIntentWithExtra(ACTION_DATAWEDGE_FROM_6_2, EXTRA_SET_CONFIG, profileConfig);
            //  You can only configure one plugin at a time, we have done the barcode input, now do the intent output
            profileConfig.Remove("PLUGIN_CONFIG");
            Bundle intentConfig = new Bundle();
            intentConfig.PutString("PLUGIN_NAME", "INTENT");
            intentConfig.PutString("RESET_CONFIG", "true");
            Bundle intentProps = new Bundle();
            intentProps.PutString("intent_output_enabled", "true");
            //  intentProps.PutString("intent_action", DataWedgeReceiver.IntentAction); // We can use this when we're going to define the DataWedgeReceiver class
            intentProps.PutString("intent_action", "barcodescanner.RECVR");
            intentProps.PutString("intent_delivery", "2");
            intentConfig.PutBundle("PARAM_LIST", intentProps);
            profileConfig.PutBundle("PLUGIN_CONFIG", intentConfig);
            SendDataWedgeIntentWithExtra(ACTION_DATAWEDGE_FROM_6_2, EXTRA_SET_CONFIG, profileConfig);

        }

        [BroadcastReceiver]
        public class ScanReceiver : BroadcastReceiver
        {
            public static DateTime UltimaLetturaBarcode = DateTime.MinValue;

            // This intent string contains the source of the data as a string  
            private static string SOURCE_TAG = "com.motorolasolutions.emdk.datawedge.source";
            // This intent string contains the barcode symbology as a string  
            private static string LABEL_TYPE_TAG = "com.motorolasolutions.emdk.datawedge.label_type";
            // This intent string contains the captured data as a string  
            // (in the case of MSR this data string contains a concatenation of the track data)  
            private static string DATA_STRING_TAG = "com.motorolasolutions.emdk.datawedge.data_string";
            // Intent Action for our operation
            public static string IntentAction = "barcodescanner.RECVR";
            public static string IntentCategory = "android.intent.category.DEFAULT";
            private TextView _textViewReadBarcode;

            public ScanReceiver(TextView textViewReadBarcode)
            {
                _textViewReadBarcode = textViewReadBarcode;
            }

            public ScanReceiver()
            {

            }

            //public event EventHandler<string> scanDataReceived;

            public override void OnReceive(Context context, Intent i)
            {
                // check the intent action is for us  
                if (i.Action.Equals(IntentAction))
                {
                    // define a string that will hold our output  
                    String Out = "";
                    // get the source of the data  
                    String source = i.GetStringExtra(SOURCE_TAG);
                    // save it to use later  
                    if (source == null)
                        source = "scanner";
                    // get the data from the intent  
                    String data = i.GetStringExtra(DATA_STRING_TAG);
                    // let's define a variable for the data length  
                    int data_len = 0;
                    // and set it to the length of the data  
                    if (data != null)
                        data_len = data.Length;
                    string sLabelType = "";
                    // check if the data has come from the barcode scanner  
                    if (source.Equals("scanner"))
                    {
                        // check if there is anything in the data  
                        if (data != null && data.Length > 0)
                        {
                            // we have some data, so let's get it's symbology  
                            sLabelType = i.GetStringExtra(LABEL_TYPE_TAG);
                            // check if the string is empty  
                            if (sLabelType != null && sLabelType.Length > 0)
                            {
                                // format of the label type string is LABEL-TYPE-SYMBOLOGY  
                                // so let's skip the LABEL-TYPE- portion to get just the symbology  
                                sLabelType = sLabelType.Substring(11);
                            }
                            else
                            {
                                // the string was empty so let's set it to "Unknown"  
                                sLabelType = "Unknown";
                            }

                            // let's construct the beginning of our output string  
                            Out = "Scanner  " + "Symbology: " + sLabelType + ", Length: " + data_len.ToString() + ", Data: " + data.ToString() + "\r\n";
                        }
                    }
                    // check if the data has come from the MSR  
                    if (source.Equals("msr"))
                    {
                        // construct the beginning of our output string  
                        Out = "Source: MSR, Length: " + data_len.ToString() + ", Data: " + data.ToString() + "\r\n";
                    }

                    System.Diagnostics.Debug.WriteLine(Out);

                    /*
                    if (scanDataReceived != null)
                    {

                        // Prendo la data precedente
                        if ((DateTime.Now - UltimaLetturaBarcode).TotalMilliseconds < 1000)
                            return;
                        UltimaLetturaBarcode = DateTime.Now;

                        //scanDataReceived (this, Out);
                        scanDataReceived(this, data);
                    }
                    */
                    data = data.Replace('\n', ' ').TrimEnd();
                    _textViewReadBarcode.Text = data;
                    //MessagingCenter.Send<App, string>((App)Xamarin.Forms.Application.Current, "ScanBarcode", data);
                }
            }
        }

    }
}

