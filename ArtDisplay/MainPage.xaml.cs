using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.System.Display;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ArtDisplay
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Settings _settings;
        private bool _monitorOn = true;
        public MainPage()
        {
            this.InitializeComponent();
            LoadSettingsAsync();
            SetSource();
            DisplayTrigger();
        }

        private void DisplayTrigger()
        {
            if (!_settings.AutoSleep) return;

            TimeSpan period = TimeSpan.FromSeconds(1);

            ThreadPoolTimer PeriodicTimer = ThreadPoolTimer.CreatePeriodicTimer((source) =>
            {
                var currentTime = DateTime.Now;
                var onH = _settings.GetHour(_settings.TurnOnMonitor);
                var onM = _settings.GetMinutes(_settings.TurnOnMonitor);
                var offH = _settings.GetHour(_settings.TurnOffMonitor);
                var offM = _settings.GetMinutes(_settings.TurnOffMonitor);

                if ((currentTime.Hour > onH && currentTime.Hour < offH) ||
                    (currentTime.Hour == onH && currentTime.Minute >= onM) ||
                    (currentTime.Hour == offH && currentTime.Minute < offM))
                {
                    if (!_monitorOn)
                    {
                        _monitorOn = true;
                        Dispatcher.RunAsync(CoreDispatcherPriority.High,
                              () =>
                              {
                                  // TODO zapni display powershell script

                              });
                    }
                }
                else
                {
                    if (_monitorOn)
                    {
                        _monitorOn = false;
                        Dispatcher.RunAsync(CoreDispatcherPriority.High,
                        () =>
                        {
                            //
                            // TODO vypne display powershell script
                            //
                        });
                    }
                }


            }, period);


        }

        private async void LoadSettingsAsync()
        {
            var storageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/settings.json"));
            if (storageFile == null) throw new Exception("Setting file missing!");
            string contents = await FileIO.ReadTextAsync(storageFile);
            _settings = JsonConvert.DeserializeObject<Settings>(contents);
        }

        public async void SetSource()
        {
            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(_settings.DisplayFolder);

            if (folder == null) return;

            StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);

            IReadOnlyList<StorageFile> sortedItems = await folder.GetFilesAsync();

            if (!sortedItems.Any()) return;

            using (Windows.Storage.Streams.IRandomAccessStream fileStream = await sortedItems[0].OpenAsync(Windows.Storage.FileAccessMode.Read))
            {
                // Set the image source to the selected bitmap.
                BitmapImage bitmapImage = new BitmapImage();

                bitmapImage.SetSource(fileStream);
                DisplayImage.Source = bitmapImage;
            }
        }
    }
}