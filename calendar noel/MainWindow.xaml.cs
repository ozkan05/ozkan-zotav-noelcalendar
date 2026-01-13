using calendar_noel.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace calendar_noel
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // === FICHIERS ===
        private readonly string configPath = "config.txt";
        private readonly string messagesPath = "messages.txt";
        private readonly string usedMessagesPath = "historique.txt";
        private readonly string imagesFolder = "Images";
        private MediaPlayer player  = new MediaPlayer();

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

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;

            LoadConfig();
            LoadMessages();

            ShowTodayCard();

            StartSnow();
            StartGarlands();
            AddGlowToButtons();

            // Vérification null / gestion d'erreur lors du chargement des ressources embarquées
            try
            {
                var msgsResource = ChargerMessagesDepuisResource();
                // Si nécessaire, intégrer msgsResource au comportement existant ici.
                // On ne modifie pas l'état global si la structure MessageNoel diffère.
            }
            catch (Exception ex)
            {
                // Journaliser et prévenir l'utilisateur sans planter l'application
                MessageBox.Show(
                    $"Erreur lors du chargement des messages embarqués : {ex.Message}",
                    "Chargement Resources",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }

            ChargerMessageDuJour();

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
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
            {
            // Charger et jouer la musique de fond
            player.Open(new Uri("Music/letitsnow.mp3", UriKind.Relative));
            player.MediaEnded += player_MediaEnded;
            player.Volume = 0.5; // Volume à 50%
            player.Play();
        }
        private void player_MediaEnded(object sender, EventArgs e)
{
     // Remet au début et relance en boucle
     player.Position = TimeSpan.Zero;
     player.Play();
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
                usedMessages = File.ReadAllLines(usedMessagesPath).ToList();
        }

        private string GetDailyMessage()
        {
            var libres = messages.Except(usedMessages).ToList();

            if (libres.Count == 0)
            {
                usedMessages.Clear();
                File.WriteAllText(usedMessagesPath, "");
                libres = messages;
            }

            var msg = libres[random.Next(libres.Count)];
            usedMessages.Add(msg);
            File.WriteAllLines(usedMessagesPath, usedMessages);
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

            string month = DateTime.Now.ToString("MMMM");
            string Day = DateTime.Now.Day.ToString();

            TextBlock title = new TextBlock
            {
                Text = $"🎁 Joyeux {day} {month} {prenom} !",
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
         
            var calendarwindow= new CalendarWindow();
            calendarwindow.Show();
            this.Close();


        }

        private void BTN_Info_Click(object sender, RoutedEventArgs e)
        {
            AnimateTransition();
            MessageBox.Show("❄ Créé par Ozkan & Zotav - Calendrier de Noël 🎄");
        }
        public List<MessageNoel> ChargerMessagesDepuisResource()
        {
            var messages = new List<MessageNoel>();
            var assembly = Assembly.GetExecutingAssembly();

            // chercher le nom réel de la ressource
            var ressources = assembly.GetManifestResourceNames();
            var resourceName = ressources
                .FirstOrDefault(n => n.EndsWith("messages.txt", StringComparison.OrdinalIgnoreCase));

            if (resourceName == null)
            {
                // lève une erreur claire ou journalise les ressources disponibles pour débogage
                throw new InvalidOperationException(
                    $"Ressource embarquée 'messages.txt' introuvable. Ressources disponibles: {string.Join(", ", ressources)}");
            }

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new InvalidOperationException($"Impossible d'ouvrir la ressource '{resourceName}'.");

                using (var reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        var ligne = reader.ReadLine();
                        if (string.IsNullOrWhiteSpace(ligne)) continue;

                        var parties = ligne.Split(new[] { '|' }, 2);
                        var texte = parties.Length > 0 ? parties[0].Trim() : string.Empty;
                        var image = parties.Length > 1 ? parties[1].Trim() : string.Empty;

                        messages.Add(new MessageNoel(texte, image));
                    }
                }
            }

            return messages;
        }
        void ChargerMessageDuJour()
        {
            string messagesPath = "messages.txt";
            string historiquePath = "historique.txt";

            // 1. Charger tous les messages
            var messages = File.ReadAllLines(messagesPath)
                .Select(l => l.Split(new[] {'|'}, 2))
                .Select(parts => new
                {
                    Texte = parts.Length > 0 ? parts[0].Trim() : string.Empty,
                    Image = parts.Length > 1 ? parts[1].Trim() : "default.png"
                })
                .Where(m => !string.IsNullOrEmpty(m.Texte))
                .ToList();

            // 2. Charger l'historique (messages déjà utilisés)
            var utilises = File.Exists(historiquePath)
                ? File.ReadAllLines(historiquePath)
                    .Select(l =>
                    {
                        var p = l.Split(new[] {'|'}, 3);
                        return p.Length > 1 ? p[1].Trim() : string.Empty;
                    })
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToHashSet()
                : new HashSet<string>();

            // 3. Filtrer les messages disponibles
            var disponibles = messages
                .Where(m => !utilises.Contains(m.Texte))
                .ToList();

            if (disponibles.Count == 0)
            {
                MessageText.Text = "Tous les messages ont été utilisés 🎄";
                return;
            }

            // 4. Tirage aléatoire
            Random rnd = new Random();
            var choisi = disponibles[rnd.Next(disponibles.Count)];

            // 5. Affichage
            MessageText.Text = choisi.Texte;
            string imagePath = System.IO.Path.Combine(
    AppDomain.CurrentDomain.BaseDirectory,
    "Images",
    choisi.Image
);

            if (File.Exists(imagePath))
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                ImageNoel.Source = bitmap;
            }
            else
            {
                MessageBox.Show($"Image introuvable : {choisi.Image}");
            }
            // 6. Sauvegarde dans l’historique
            string ligne = $"{DateTime.Today:yyyy-MM-dd} | {choisi.Texte} | {choisi.Image}";
            File.AppendAllText(historiquePath, ligne + Environment.NewLine);
        }
    }
}