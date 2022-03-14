using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AsyncDemo
{
    public partial class MainPage : ContentPage
    {
        ObservableCollection<OperationResult> resultList = new ObservableCollection<OperationResult>();
        public ObservableCollection<OperationResult> Results { get { return resultList; } }

        public MainPage()
        {
            InitializeComponent();
            ResultsView.ItemsSource = resultList;
            //PerformTasks();
        }

        public void RunSync()
        {
            //empty the list
            resultList.Clear();
            executionTime.Text = "";

            var timelapse = new Stopwatch();
            timelapse.Start();

            for (int i = 1; i <= 10; i++)
            {
                resultList.Add(ExecuteTheAction(120 * i, $"Task-{i}", "Sync").Result);
            }

            timelapse.Stop();
            executionTime.Text = $"Total in MS: {timelapse.ElapsedMilliseconds}";
        }

        public async Task RunAsync()
        {
            //empty the list
            resultList.Clear();
            executionTime.Text = "";

            for (int i = 1; i <= 10; i++)
            {
                var result = await Task.Run(() => ExecuteTheAction(120 * i, $"Task-{i}", "Async"));
                resultList.Add(result);
            }
        }

        public async Task RunAsyncParallel()
        {
            //empty the list
            resultList.Clear();
            executionTime.Text = "";

            var parallelTaskList = new List<Task<OperationResult>>();
            for (int i = 1; i <= 10; i++)
            {
                parallelTaskList.Add(Task.Run(() => ExecuteTheAction(120 * i, $"Task-{i}", "Parallel")));
            }

            var results = await Task.WhenAll(parallelTaskList);
            results.ToList().ForEach(result =>
            {
                resultList.Add(result);
            });
        }


        private void DoSyncClick(object sender, EventArgs e)
        {
            var timelapse = new Stopwatch();
            timelapse.Start();

            RunSync();

            timelapse.Stop();
            executionTime.Text = $"Total in MS: {timelapse.ElapsedMilliseconds}";
        }

        private async void DoAsyncClick(object sender, EventArgs e)
        {
            var timelapse = new Stopwatch();
            timelapse.Start();

            await RunAsync();

            timelapse.Stop();
            executionTime.Text = $"Total in MS: {timelapse.ElapsedMilliseconds}";
        }

        private async void DoAsyncParallelClick(object sender, EventArgs e)
        {
            var timelapse = new Stopwatch();
            timelapse.Start();

            await RunAsyncParallel();

            timelapse.Stop();
            executionTime.Text = $"Total in MS: {timelapse.ElapsedMilliseconds}";
        }
        

        private async void TryClick(object send, EventArgs e)
        {
            await DisplayAlert("Hey!", "That Worked!", "Close");
        }

        private Task<OperationResult> ExecuteTheAction(int miliseconds, string taskName, string type)
        {
            //Thread.Sleep(miliseconds);
            return Task.FromResult<OperationResult>(new OperationResult { TaskName = taskName, TimeSpent = miliseconds, ExecutionType = type});
            //return 
        }
    }

    public class OperationResult
    {
        public decimal TimeSpent { get; internal set; }
        public string TaskName { get; internal set; }
        public string DisplayValue => $"{TaskName}{ExecutionType} - {TimeSpent}MS";

        public string ExecutionType { get; internal set; }
    }
}
