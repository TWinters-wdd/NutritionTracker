using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NutritionTracker
{
    public partial class MainWindow : Window
    {
        private Button _activeButton = null;
        private Image _activeImage = null;
        private Uri _originalImageUri = null;

        public MainWindow()
        {
            InitializeComponent();


            Main.Navigate(new NutritionPage());

            HighlightButtonOnStartup(SaladBtn, SaladImg, "/Images/003-salad.png");
        }

        private void HighlightButtonOnStartup(Button button, Image image, string originalImagePath)
        {
            button.Background = Brushes.DodgerBlue;

            image.Source = new BitmapImage(new Uri(originalImagePath.Replace(".png", "-white.png"), UriKind.Relative));

            _activeButton = button;
            _activeImage = image;
            _originalImageUri = new Uri(originalImagePath, UriKind.Relative);
        }

        private void HighlightButton(Button newButton, Image newImage, string originalImagePath)
        {
            if (_activeButton != null && _activeButton != newButton)
            {
                _activeButton.Background = Brushes.LightGray;

                if (_activeImage != null && _originalImageUri != null)
                {
                    _activeImage.Source = new BitmapImage(_originalImageUri);
                }
            }

            newButton.Background = Brushes.DodgerBlue; // Change the color to my wireframe's button color

            _originalImageUri = new Uri(originalImagePath, UriKind.Relative);
            newImage.Source = new BitmapImage(new Uri(originalImagePath.Replace(".png", "-white.png"), UriKind.Relative));

            _activeButton = newButton;
            _activeImage = newImage;
        }


        // ========== BUTTON CLICKS CODE ==========
        private void SaladBtn_Click(object sender, RoutedEventArgs e)
        {
            Main.Navigate(new NutritionPage());
            HighlightButton(SaladBtn, SaladImg, "/Images/003-salad.png");
        }

        private void WeightBtn_Click(object sender, RoutedEventArgs e)
        {
            Main.Navigate(new ExerciseTracker());
            HighlightButton(WeightBtn, WeightImg, "/Images/002-dumbbell.png");
        }

        private void TrophyBtn_Click(object sender, RoutedEventArgs e)
        {
            Main.Navigate(new GoalGame());
            HighlightButton(TrophyBtn, TrophyImg, "/Images/001-trophy.png");
        }

        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            Main.Navigate(new AppSettings());
            HighlightButton(SettingsBtn, SettingsImg, "/Images/004-gear.png");
        }

    }
}