using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace calendar_noel
{
    public partial class MainWindow : Window
    {
        // === FICHIERS ===
        private readonly string configPath = "config.txt";
        private readonly string messagesPath = "messages.txt";
        private readonly string usedMessagesPath = "used.txt";
        private readonly string imagesFolder = "Images";

        // === DONNÉES ===
        private string prenom = "Invité";
        private string genre = "Homme";
        private string theme = "Rouge";

        private List<string> messages = new List<string>();
        private List<string> usedMessages = new List<string>();

        private DispatcherTimer snowTimer;
        private DispatcherTimer garlandTimer;
        private Random random = new Random();

        private bool garlandState = false;

        public List<string> UsedMessages { get => usedMessages; set => usedMessages = value; }

        public MainWindow()
        {
            InitializeComponent();

            LoadConfig();
            LoadMessages();

            ShowTodayCard();

            StartSnow();
            StartGarlands();
            AddCandles();
            AddTree3D();
            AddGlowToButtons();
        }

        // ================= CONFIG =================
        private void LoadConfig()
        {
            if (!File.Exists(configPath))
            {
                File.WriteAllLines(configPath, new string[]
                {
                    "Prenom=Invité",
                    "Genre=Homme",
                    "Theme=Rouge"
                });
            }

            var lines = File.ReadAllLines(configPath);
            foreach (var l in lines)
            {
                if (l.StartsWith("Prenom=")) prenom = l.Replace("Prenom=", "");
                if (l.StartsWith("Genre=")) genre = l.Replace("Genre=", "");
                if (l.StartsWith("Theme=")) theme = l.Replace("Theme=", "");
            }
        }

        // ================= MESSAGES =================
        private void LoadMessages()
        {
            if (!File.Exists(messagesPath))
            {
                File.WriteAllLines(messagesPath, new string[]
                {
                    "Partage un sourire aujourd'hui 🎄",
                    "Fais une bonne action 🎁",
                    "Regarde un film de Noël 🎬",
                    "Bois un chocolat chaud ☕",
                    "Offre un cadeau surprise 🎉"
                });
            }

            messages = File.ReadAllLines(messagesPath).ToList();
            if (File.Exists(usedMessagesPath))
                UsedMessages = File.ReadAllLines(usedMessagesPath).ToList();
        }

        private string GetDailyMessage()
        {
            var libres = messages.Except(UsedMessages).ToList();

            if (libres.Count == 0)
            {
                UsedMessages.Clear();
                File.WriteAllText(usedMessagesPath, "");
                libres = messages;
            }

            var msg = libres[random.Next(libres.Count)];
            UsedMessages.Add(msg);
            File.WriteAllLines(usedMessagesPath, UsedMessages);
            return msg;
        }

        // ================= AFFICHAGE CARTE =================
        private void ShowTodayCard()
        {
            AnimateTransition();

            Grd_container.Children.Clear();

            int day = DateTime.Now.Day;
            DateTime now = DateTime.Now;
            DateTime noel = new DateTime(now.Year, 12, 25);
            int daysLeft = (noel - now).Days;

            string message = GetDailyMessage();
            string cadeau = GenerateRandomGift();

            StackPanel panel = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            TextBlock title = new TextBlock
            {
                Text = $"🎁 Joyeux {day} décembre {prenom} !",
                FontSize = 34,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.DarkRed,
                TextAlignment = TextAlignment.Center
            };

            Image img = new Image
            {
                Width = 250,
                Margin = new Thickness(10)
            };

            LoadRandomImage(img);

            TextBlock msg = new TextBlock
            {
                Text = message,
                FontSize = 22,
                TextAlignment = TextAlignment.Center
            };

            TextBlock gift = new TextBlock
            {
                Text = $"🎁 Cadeau du jour : {cadeau}",
                FontSize = 20,
                Foreground = Brushes.DarkGoldenrod,
                Margin = new Thickness(0, 10, 0, 10),
                TextAlignment = TextAlignment.Center
            };

            TextBlock countdown = new TextBlock
            {
                Text = $"🎄 Jours avant Noël : {daysLeft}",
                FontSize = 20,
                Foreground = Brushes.DarkGreen,
                TextAlignment = TextAlignment.Center
            };

            panel.Children.Add(title);
            panel.Children.Add(img);
            panel.Children.Add(msg);
            panel.Children.Add(gift);
            panel.Children.Add(countdown);

            Grd_container.Children.Add(panel);
        }

        private void LoadRandomImage(Image img)
        {
            if (!Directory.Exists(imagesFolder))
                Directory.CreateDirectory(imagesFolder);

            var files = Directory.GetFiles(imagesFolder);

            if (files.Length > 0)
            {
                img.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath(files[random.Next(files.Length)])));
            }
        }

        // ================= NEIGE =================
        private void StartSnow()
        {
            snowTimer = new DispatcherTimer();
            snowTimer.Interval = TimeSpan.FromMilliseconds(100);
            snowTimer.Tick += SnowTick;
            snowTimer.Start();
        }

        private void SnowTick(object sender, EventArgs e)
        {
            if (random.Next(3) == 0)
                CreateSnow();

            foreach (var flake in SnowCanvas.Children.OfType<Ellipse>().ToList())
            {
                Canvas.SetTop(flake, Canvas.GetTop(flake) + 3);

                if (Canvas.GetTop(flake) > ActualHeight)
                    SnowCanvas.Children.Remove(flake);
            }
        }

        private void CreateSnow()
        {
            Ellipse snow = new Ellipse
            {
                Width = random.Next(2, 6),
                Height = random.Next(2, 6),
                Fill = Brushes.White
            };

            Canvas.SetLeft(snow, random.Next((int)ActualWidth));
            Canvas.SetTop(snow, 0);
            SnowCanvas.Children.Add(snow);
        }

        // ================= GUIRLANDES =================
        private void StartGarlands()
        {
            garlandTimer = new DispatcherTimer();
            garlandTimer.Interval = TimeSpan.FromMilliseconds(500);
            garlandTimer.Tick += (s, e) =>
            {
                Grd_menu.Background = garlandState
                    ? new SolidColorBrush(Color.FromRgb(214, 40, 40))
                    : new SolidColorBrush(Color.FromRgb(255, 215, 0));

                garlandState = !garlandState;
            };
            garlandTimer.Start();
        }

        // ================= BOUGIES =================
        private void AddCandles()
        {
            for (int i = 0; i < 3; i++)
            {
                Ellipse e = new Ellipse
                {
                    Width = 20,
                    Height = 20,
                    Fill = Brushes.Orange,
                    Opacity = 0.7
                };

                Canvas.SetLeft(e, 60 + i * 40);
                Canvas.SetTop(e, 20);
                SnowCanvas.Children.Add(e);
            }
        }

        // ================= SAPIN 3D =================
        private void AddTree3D()
        {
            Polygon p = new Polygon
            {
                Points = new PointCollection
                {
                    new Point(100, 220),
                    new Point(160, 120),
                    new Point(220, 220)
                },
                Fill = Brushes.Green
            };

            SnowCanvas.Children.Add(p);
        }

        // ================= GLOW BOUTONS =================
        private void AddGlowToButtons()
        {
            foreach (var btn in Grd_menu.Children.OfType<Button>())
            {
                btn.Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = Colors.Gold,
                    BlurRadius = 20,
                    ShadowDepth = 0
                };
            }
        }

        // ================= TRANSITION =================
        private void AnimateTransition()
        {
            DoubleAnimation anim = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(400)
            };

            Grd_container.BeginAnimation(OpacityProperty, anim);
        }

        // ================= CADEAUX =================
        private string GenerateRandomGift()
        {
            string[] gifts =
            {
                "🎮 Console magique",
                "🧸 Peluche enchantée",
                "🎧 Casque gamer",
                "🍫 Chocolats de Noël",
                "🛷 Traîneau surprise"
            };

            return gifts[random.Next(gifts.Length)];
        }

        // ================= BOUTONS =================
        private void BTN_Carte_du_jour_Click(object sender, RoutedEventArgs e)
        {
            ShowTodayCard();
        }

        private void BTN_Portevue_Click(object sender, RoutedEventArgs e)
        {
            AnimateTransition();
            MessageBox.Show("📘 Porte-vue bientôt disponible !");
        }

        private void BTN_Calendrier_Click(object sender, RoutedEventArgs e)
        {
            AnimateTransition();
            MessageBox.Show("📅 Calendrier interactif bientôt !");
        }

        private void BTN_Info_Click(object sender, RoutedEventArgs e)
        {
            AnimateTransition();
            MessageBox.Show("❄ Créé par Ozkan - Calendrier de Noël 🎄");
        }
    }
}
