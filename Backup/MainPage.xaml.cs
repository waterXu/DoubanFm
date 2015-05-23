using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Mangopollo.Tasks;
using Mangopollo.Tiles;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace Mangopollo.Sample
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }


        private void TestIfWP8_Click(object sender, RoutedEventArgs e)
        {
            if (Utils.IsWP8)
            {
                MessageBox.Show("It's a Windows Phone 8 !");
            }
            else
            {
                MessageBox.Show("It's a Windows Phone 7 !");
            }
        }

        private void TestIfWP78_Click(object sender, RoutedEventArgs e)
        {
            if (Utils.CanUseLiveTiles)
            {
                MessageBox.Show("It's a Windows Phone 7.8 or sup !");
            }
            else
            {
                MessageBox.Show("It's a Windows Phone 7 !");
            }
        }

        private void CreateCycleTile_Click(object sender, RoutedEventArgs e)
        {
            if (!Utils.CanUseLiveTiles)
            {
                MessageBox.Show("This feature needs Windows Phone 8");
                return;
            }
            try
            {
                var mytile = new CycleTileData
                    {
                        Title = "cyclic tile",
                        Count = 42,
                        SmallBackgroundImage = new Uri("/Assets/logo159x159.png", UriKind.Relative),
                        CycleImages = new List<Uri> {new Uri("/Assets/Image1.png", UriKind.Relative), new Uri("/Assets/Image2.png", UriKind.Relative), new Uri("/Assets/Image3.png", UriKind.Relative)}
                    };


#if ALTERNATIVE_SOLUTION
                var mytile = Mangopollo.Tiles.TilesCreator.CreateCyclicTile("cyclic tile", 42, new Uri("/Assets/logo159x159.png", UriKind.Relative), new List<Uri>() { new Uri("/Assets/Image1.png", UriKind.Relative), new Uri("/Assets/Image2.png", UriKind.Relative), new Uri("/Assets/Image3.png", UriKind.Relative) });
#endif
                ShellTileExt.Create(new Uri("/MainPage.xaml?msg=from%20cycle%20tile", UriKind.Relative), mytile, false);
            }
            catch
            {
                MessageBox.Show("remove tile before create it again");
            }
        }

        private void CreateCycleTileWide_Click(object sender, RoutedEventArgs e)
        {
            if (!Utils.CanUseLiveTiles)
            {
                MessageBox.Show("This feature needs Windows Phone 8");
                return;
            }

            try
            {
                var mytile = new CycleTileData
                    {
                        Title = "cyclic wide tile",
                        Count = 42,
                        SmallBackgroundImage = new Uri("/Assets/logo159x159.png", UriKind.Relative),
                        CycleImages = new List<Uri> {new Uri("/Assets/Image1.png", UriKind.Relative), new Uri("/Assets/Image2.png", UriKind.Relative), new Uri("/Assets/Image3.png", UriKind.Relative)}
                    };

#if ALTERNATIVE_SOLUTION
                var mytile = Mangopollo.Tiles.TilesCreator.CreateCyclicTile("cyclic wide tile", 42, new Uri("/Assets/logo159x159.png", UriKind.Relative), new List<Uri>() { new Uri("/Assets/Image1.png", UriKind.Relative), new Uri("/Assets/Image2.png", UriKind.Relative), new Uri("/Assets/Image3.png", UriKind.Relative) });
#endif
                ShellTileExt.Create(new Uri("/MainPage.xaml?msg=from%20wide%20cycle%20tile", UriKind.Relative), mytile, true);
            }
            catch
            {
                MessageBox.Show("remove tile before create it again");
            }
        }

        private void CreateIconicTile_Click(object sender, RoutedEventArgs e)
        {
            if (!Utils.CanUseLiveTiles)
            {
                MessageBox.Show("This feature needs Windows Phone 8");
                return;
            }

            try
            {
                var mytile = new IconicTileData
                    {
                        Title = "iconic tile",
                        Count = 8,
                        BackgroundColor = Colors.Purple,
                        IconImage = new Uri("/Assets/logo202x202.png", UriKind.Relative),
                        SmallIconImage = new Uri("/Assets/logo110x110.png", UriKind.Relative)
                    };

#if ALTERNATIVE_SOLUTION
                var mytile = Mangopollo.Tiles.TilesCreator.CreateIconicTile("iconic tile", 8, Colors.Purple, new Uri("/Assets/logo202x202.png", UriKind.Relative), new Uri("/Assets/logo110x110.png", UriKind.Relative));
#endif

                ShellTileExt.Create(new Uri("/MainPage.xaml?msg=from%20iconic%20tile", UriKind.Relative), mytile, false);
            }
            catch
            {
                MessageBox.Show("remove tile before create it again");
            }
        }


        private void CreateIconicTileWide_Click(object sender, RoutedEventArgs e)
        {
            if (!Utils.CanUseLiveTiles)
            {
                MessageBox.Show("This feature needs Windows Phone 8");
                return;
            }

            try
            {
                var mytile = new IconicTileData
                    {
                        Title = "iconic tile",
                        Count = 8,
                        BackgroundColor = Color.FromArgb(255, 200, 10, 30),
                        IconImage = new Uri("/Assets/logo202x202.png", UriKind.Relative),
                        SmallIconImage = new Uri("/Assets/logo110x110.png", UriKind.Relative),
                        WideContent1 = "Mangopollo Library",
                        WideContent2 = "use Windows Phone 8 features",
                        WideContent3 = "on Windows Phone 7 apps"
                    };

#if ALTERNATIVE_SOLUTION
                var mytile = Mangopollo.Tiles.TilesCreator.CreateIconicTile("iconic wide tile", 8, Colors.Gray, new Uri("/Assets/logo202x202.png", UriKind.Relative), new Uri("/Assets/logo110x110.png", UriKind.Relative), "Mangopollo Library", "created by", "Rudy Huyn");
#endif
                ShellTileExt.Create(new Uri("/MainPage.xaml?msg=from%20wide%20iconic%20tile", UriKind.Relative), mytile, true);
            }
            catch
            {
                MessageBox.Show("remove tile before create it again");
            }
        }

        private void CreateFlipTile_Click(object sender, RoutedEventArgs e)
        {
            if (!Utils.CanUseLiveTiles)
            {
                MessageBox.Show("This feature needs Windows Phone 8");
                return;
            }

            try
            {
                var mytile = new FlipTileData
                    {
                        Title = "wide flip tile",
                        BackTitle = "created by",
                        BackContent = "Rudy Huyn",
                        Count = 9,
                        SmallBackgroundImage = new Uri("/Assets/logo159x159.png", UriKind.Relative),
                        BackgroundImage = new Uri("/Assets/Background336x336_1.png", UriKind.Relative),
                        BackBackgroundImage = new Uri("/Assets/Background336x336_2.png", UriKind.Relative)
                    };


#if ALTERNATIVE_SOLUTION
                var mytile = Mangopollo.Tiles.TilesCreator.CreateFlipTile("wide flip tile", "created by", "Rudy Huyn", 9, new Uri("/Assets/logo159x159.png", UriKind.Relative), new Uri("/Assets/Background336x336_1.png", UriKind.Relative), new Uri("/Assets/Background336x336_2.png", UriKind.Relative));
#endif
                ShellTileExt.Create(new Uri("/MainPage.xaml?msg=from%20flip%20tile", UriKind.Relative), mytile, false);
            }
            catch
            {
                MessageBox.Show("remove tile before create it again");
            }
        }

        private void CreateFlipTileWide_Click(object sender, RoutedEventArgs e)
        {
            if (!Utils.CanUseLiveTiles)
            {
                MessageBox.Show("This feature needs Windows Phone 8");
                return;
            }

            try
            {
                var mytile = new FlipTileData
                    {
                        Title = "wide flip tile",
                        BackTitle = "created by",
                        BackContent = "Rudy Huyn",
                        Count = 9,
                        SmallBackgroundImage = new Uri("/Assets/logo159x159.png", UriKind.Relative),
                        BackgroundImage = new Uri("/Assets/Background336x336_1.png", UriKind.Relative),
                        BackBackgroundImage = new Uri("/Assets/Background336x336_2.png", UriKind.Relative),
                        WideBackContent = "This is a very long long text to demonstrate the back content of a wide flip tile",
                        WideBackgroundImage = new Uri("/Assets/Background691x336_1.png", UriKind.Relative),
                        WideBackBackgroundImage = new Uri("/Assets/Background691x336_2.png", UriKind.Relative)
                    };

#if ALTERNATIVE_SOLUTION
                var mytile = Mangopollo.Tiles.TilesCreator.CreateFlipTile("flip tile", "created by", "Rudy Huyn", "This is a very long long text to demonstrate the back content of a wide flip tile", 9, new Uri("/Assets/logo159x159.png", UriKind.Relative), new Uri("/Assets/Background336x336_1.png", UriKind.Relative), new Uri("/Assets/Background336x336_2.png", UriKind.Relative), new Uri("/Assets/Background691x336_1.png", UriKind.Relative), new Uri("/Assets/Background691x336_2.png", UriKind.Relative));
#endif
                ShellTileExt.Create(new Uri("/MainPage.xaml?msg=from%20wipe%20flip%20tile", UriKind.Relative), mytile, true);
            }
            catch
            {
                MessageBox.Show("remove tile before create it again");
            }
        }

        private void UpdateMainTile_Click(object sender, RoutedEventArgs e)
        {
            if (!Utils.CanUseLiveTiles)
            {
                MessageBox.Show("This feature needs Windows Phone 8");
                return;
            }

            var maintile = new FlipTileData
                {
                    Title = "main tile",
                    BackTitle = "this is",
                    BackContent = "main tile",
                    Count = 3,
                    SmallBackgroundImage = new Uri("/Assets/logo159x159.png", UriKind.Relative),
                    BackgroundImage = new Uri("/Assets/Background336x336_2.png", UriKind.Relative),
                    BackBackgroundImage = new Uri("/Assets/Background336x336_1.png", UriKind.Relative),
                    WideBackContent = "This is a very long long text to demonstrate the back content of a wide flip tile",
                    WideBackgroundImage = new Uri("/Assets/Background691x336_2.png", UriKind.Relative),
                    WideBackBackgroundImage = new Uri("/Assets/Background691x336_1.png", UriKind.Relative)
                };

#if ALTERNATIVE_SOLUTION
            var maintile = Mangopollo.Tiles.TilesCreator.CreateFlipTile("main tile", "This is", "main tile", "This is a very long long text to demonstrate the back content of a wide flip tile", 9, new Uri("/Assets/logo159x159.png", UriKind.Relative), new Uri("/Assets/Background336x336_1.png", UriKind.Relative), new Uri("/Assets/Background336x336_2.png", UriKind.Relative), new Uri("/Assets/Background691x336_1.png", UriKind.Relative), new Uri("/Assets/Background691x336_2.png", UriKind.Relative));
#endif
            ShellTile.ActiveTiles.First().Update(maintile);
        }

        private void MapTask_Click(object sender, RoutedEventArgs e)
        {
            if (!Utils.IsWP8)
            {
                MessageBox.Show("This feature needs Windows Phone 8");
                return;
            }

            var maptask = new MapsTask {SearchTerm = "Rennes"};
            maptask.Show();
        }

        private void MapDirectionTask_Click(object sender, RoutedEventArgs e)
        {
            if (!Utils.IsWP8)
            {
                MessageBox.Show("This feature needs Windows Phone 8");
                return;
            }

            var maptask = new MapsDirectionsTask {Start = new LabeledMapLocation {Label = "Rennes"}, End = new LabeledMapLocation {Label = "Paris"}};
            maptask.Show();
        }

        private void MapDownloaderTask_Click(object sender, RoutedEventArgs e)
        {
            if (!Utils.IsWP8)
            {
                MessageBox.Show("This feature needs Windows Phone 8");
                return;
            }

            var maptask = new MapDownloaderTask();
            maptask.Show();
        }

        private void SaveAppointmentTask_Click(object sender, RoutedEventArgs e)
        {
            if (!Utils.IsWP8)
            {
                MessageBox.Show("This feature needs Windows Phone 8");
                return;
            }

            var task = new SaveAppointmentTask
                {
                    StartTime = DateTime.Now.AddMinutes(10),
                    EndTime = DateTime.Now.AddMinutes(20),
                    Details = "See you in France",
                    Subject = "Meeting with Rudy Huyn",
                    Location = "Rennes, France"
                };
            task.Show();
        }

        private void MapUpdaterTask_Click(object sender, RoutedEventArgs e)
        {
            if (!Utils.IsWP8)
            {
                MessageBox.Show("This feature needs Windows Phone 8");
                return;
            }

            var maptask = new MapUpdaterTask();
            maptask.Show();
        }
    }
}