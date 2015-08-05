using System;
using Android.App;
using Android.Widget;
using Android.OS;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using Android.Views;
using System.Collections.Generic;

namespace SearchFromGoogle
{
    [Activity(Label = "SearchFromGoogle", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int count = 1;
        Button btnSearch;
        EditText txtBoxSearch;
        ListView listSearchList;
        GoogleSearchResults Results;
        TextView textView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            btnSearch = FindViewById<Button>(Resource.Id.btnSearch);
            txtBoxSearch = FindViewById<EditText>(Resource.Id.txtBoxSearch);
            textView = FindViewById<TextView>(Resource.Id.textView);
            listSearchList = FindViewById<ListView>(Resource.Id.listSearchList);
            listSearchList.FastScrollEnabled = true;
            btnSearch.Click += OnSearch;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSearch(object sender, EventArgs e)
        {
            try
            {
                var rxcui = txtBoxSearch.Text;
                txtBoxSearch.Text = string.Empty;
                textView.Text = rxcui;
                textView.SetTextColor(Android.Graphics.Color.Green);

                string GOOGLE_SEARCH_URL = "https://www.googleapis.com/customsearch/v1?";

                //api key
                String API_KEY = "AIzaSyCiq12ppo8v-MOFOo2dyPJos9V2w86zhlY";
                //custom search engine ID
                String SEARCH_ENGINE_ID = "005482977183111778816:jezhwiwf14e";

                String FINAL_URL = GOOGLE_SEARCH_URL + "key=" + API_KEY + "&cx=" + SEARCH_ENGINE_ID;

                
                var request = HttpWebRequest.Create(string.Format(FINAL_URL + "&q=" + rxcui));
                request.ContentType = "application/json";
                request.Method = "GET";

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        Console.Out.WriteLine("Error fetching data. Server returned status code: {0}", response.StatusCode);
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        var content = reader.ReadToEnd();


                        if (string.IsNullOrWhiteSpace(content))
                        {
                            Console.Out.WriteLine("Response contained empty body...");
                        }


                        Results = JsonConvert.DeserializeObject<GoogleSearchResults>(content);

                    }
                }

              

                listSearchList.Adapter = new HomeScreenAdapter(this, Results.items);
                listSearchList.ItemClick += OnListItemClick;

            }
            catch(Exception ex)
            {
               Console.WriteLine("There is some error with network!! please contect to admin.!!"+ ex.InnerException);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var listView = sender as ListView;
            var t = Results.items[e.Position].link;
            Android.Widget.Toast.MakeText(this, t, Android.Widget.ToastLength.Long).Show();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class HomeScreenAdapter : BaseAdapter<GoogleSearchItem>
    {
        GoogleSearchItem[] items;
        Activity context;
        public HomeScreenAdapter(Activity context, GoogleSearchItem[] items)
            : base()
        {
            this.context = context;
            this.items = items;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
       


        public override int Count
        {
            get { return items.Length; }
        }

        public override GoogleSearchItem this[int position]
        {
            get
            {
                return items[position];
            }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = items[position];
            View view = convertView;
            if (view == null) // no view to re-use, create new
                view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, null);
            view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = items[position].title;
            return view;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class GoogleSearchItem
    {
        public string kind { get; set; }
        public string title { get; set; }
        public string link { get; set; }
        public string displayLink { get; set; }
    
    }
    /// <summary>
    /// 
    /// </summary>
    public class SourceUrl
    {
        public string type { get; set; }
        public string template { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class GoogleSearchResults
    {
        public string kind { get; set; }
        public SourceUrl url { get; set; }
        public GoogleSearchItem[] items { get; set; }
        
    }
}

