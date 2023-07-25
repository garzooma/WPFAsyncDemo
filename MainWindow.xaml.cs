using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
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

namespace WPFAsyncDemo
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
    }

    private void executeSync_Click(object sender, RoutedEventArgs e)
    {
      var watch = System.Diagnostics.Stopwatch.StartNew();

      RunDownloadSync();

      watch.Stop();
      var elapsedMs = watch.ElapsedMilliseconds;

      resultsWindow.Text += $"Total execution time: { elapsedMs}";
    }

    private async void executeAsync_Click(object sender, RoutedEventArgs e)
    {
      var watch = System.Diagnostics.Stopwatch.StartNew();

      await RunDownloadParallelAsync();

      watch.Stop();
      var elapsedMs = watch.ElapsedMilliseconds;

      resultsWindow.Text += $"Total execution time: {elapsedMs}";

    }

    private void RunDownloadSync()
    {
      List<string> webSites = PrepData();

      foreach(string site in webSites)
      {
        WebsiteDataModel results = DownloadWebsite(site);
        ReportWebsiteInfo(results);
      }
    }

    private async Task RunDownloadAsync()
    {
      List<string> webSites = PrepData();



      foreach (string site in webSites)
      {
        WebsiteDataModel results = await Task.Run(() => DownloadWebsite(site) );
        ReportWebsiteInfo(results);
      }
    }

    private async Task RunDownloadParallelAsync()
    {
      List<string> webSites = PrepData();

      List<Task<WebsiteDataModel>> tasks = new List<Task<WebsiteDataModel>>();

      foreach (string site in webSites)
      {
        Task<WebsiteDataModel> webSite =  Task.Run(() => DownloadWebsite(site));
        tasks.Add(webSite);
      }
      var results = await Task.WhenAll(tasks);

      foreach (var item in results)
      {
        ReportWebsiteInfo(item);
      }
    }

    private void ReportWebsiteInfo(WebsiteDataModel data)
    {
      resultsWindow.Text += $"{data.WebsiteUrl} downloaded: {data.WebsiteData.Length} characters long.{Environment.NewLine}";
    }

    private WebsiteDataModel DownloadWebsite(string site)
    {
      WebsiteDataModel output = new WebsiteDataModel();
      HttpClient client = new HttpClient();

      output.WebsiteUrl = site;
      client.DefaultRequestHeaders.Add("user-agent", "Chrome/114.0.0.0");
      output.WebsiteData = client.GetStringAsync(site).Result;

      return output;
    }

    private List<string> PrepData()
    {
      List<string> output = new List<string>();

      resultsWindow.Text = "";

      output.Add("https://www.yahoo.com");
      output.Add("https://www.google.com");
      output.Add("https://www.microsoft.com");
      output.Add("https://www.cnn.com");
      output.Add("https://www.codeproject.com");
      output.Add("https://www.stackoverflow.com");

      return output;
    }
  }
}
