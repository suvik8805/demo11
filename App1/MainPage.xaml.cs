using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //butterfly
        private Butterfly butterfly;

        //flowers, flower
        private List<Flower> flowers;

        // which keys are pressed
        private bool UpPressed;
        private bool LeftPressed;
        private bool RightPressed;

        //game loop timer
        private DispatcherTimer timer;

        //audio
        private MediaElement mediaElement;


        public MainPage()
        {
            this.InitializeComponent();

            // create a  butterfly
            butterfly = new Butterfly
            {
                LocationX = MyCanvas.Width / 2,
                LocationY = MyCanvas.Height / 2
            };
            //add butterfly to canvas
            MyCanvas.Children.Add(butterfly);

            //init list of flowers
            flowers = new List<Flower>();



            //key listeners
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            Window.Current.CoreWindow.KeyUp += CoreWindow_KeyUp;

            // mouse listeners
            Window.Current.CoreWindow.PointerPressed += CoreWindow_PointerPressed;

            //load audio
            LoadAudio();


            // start game loop
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / 60);
            timer.Tick += Timer_Tick;
            timer.Start();


        }

        //load audio use when colliding has happened
        private async void LoadAudio()
        {
            StorageFolder folder = 
                await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
            StorageFile file =
                await folder.GetFileAsync("ding.wav");
            var stream = await file.OpenAsync(FileAccessMode.Read);

            mediaElement = new MediaElement();
            mediaElement.AutoPlay = false;
            mediaElement.SetSource(stream, file.ContentType);

        }


        private void CoreWindow_PointerPressed(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.PointerEventArgs args)
        {
            Flower flower = new Flower();
            flower.LocationX = args.CurrentPoint.Position.X - flower.Width / 2;
            flower.LocationY = args.CurrentPoint.Position.Y - flower.Height / 2;
            // add to canvas
            MyCanvas.Children.Add(flower);
            flower.SetLocation();

            // add flowers list
            flowers.Add(flower);
        }

        // game loop
        private void Timer_Tick(object sender, object e)
        {
            // move butterfly if up pressed
            if(UpPressed) butterfly.Move();

            //rotate butterfly if left/right pressed
            if (LeftPressed) butterfly.Rotate(-1); // -1 == left, 1== right
            if (RightPressed) butterfly.Rotate(1);

            //update butterfly 
            butterfly.SetLocation();

            // collision...
            CheckCollision();

        }

        private void CheckCollision()
        {
            //loop flowers list
            foreach(Flower flower in flowers)
            {
                //get rects
                Rect BRect = new Rect(
                    butterfly.LocationX, butterfly.LocationY,
                    butterfly.ActualWidth, butterfly.ActualHeight
                    );
                Rect FRect = new Rect(
                    flower.LocationX, flower.LocationY,
                    flower.ActualWidth, flower.ActualHeight
                    );
                // does objects intersects
                BRect.Intersect(FRect);
                if (!BRect.IsEmpty) //negaatio
                {
                    // collision! area isn't empty
                    //remove flower from canvas
                    MyCanvas.Children.Remove(flower);
                    // remove from list
                    flowers.Remove(flower);
                    // play audio
                    mediaElement.Play();
                    break;
                }
            }
        }

        private void CoreWindow_KeyUp(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case VirtualKey.Up:
                    UpPressed = false;
                    break;
                case VirtualKey.Left:
                    LeftPressed = false;
                    break;
                case VirtualKey.Right:
                    RightPressed = false;
                    break;
            }
        }

        private void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case VirtualKey.Up:
                    UpPressed = true;
                    break;
                case VirtualKey.Left:
                    LeftPressed = true;
                    break;
                case VirtualKey.Right:
                    RightPressed = true;
                    break;
            }
        }
    }
}
