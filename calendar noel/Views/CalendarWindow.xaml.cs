using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Resources;

namespace calendar_noel.Views
{
    /// <summary>
    /// Logique d'interaction pour CalendarWindow.xaml
    /// </summary>
    public partial class CalendarWindow : Window
    {
        // Liste des URI des images (ressources WPF)
        private static List<string> imagesCartes;

        // Générateur aléatoire
        private static Random random = new Random();
        public CalendarWindow()
        {
            InitializeComponent();
            // Initialisation des listes UNE SEULE FOIS
            InitialiserCartes();
        }
        /// <summary>
        /// Initialise les listes d’images
        /// </summary>
        private void InitialiserCartes()
        {
            // Empêche la réinitialisation (évite la répétition)
            //if (imagesCartes != null && textesCartes != null)
                //return;

            // 🔹 LISTE DES IMAGES (pack URI WPF)
            imagesCartes = new List<string>
            {
                "pack://application:,,,/Images/cartetest.png",
                "pack://application:,,,/Images/cartetest2.png",
                "pack://application:,,,/Images/bougie.png",
                "pack://application:,,,/Images/chocolat.png",
                "pack://application:,,,/Images/cadeau.png",
                "pack://application:,,,/Images/coeur.png",
                "pack://application:,,,/Images/decoration.png",
                "pack://application:,,,/Images/entraide.png",
                "pack://application:,,,/Images/etoile.png",
                "pack://application:,,,/Images/neige.png",
                "pack://application:,,,/Images/sapin.png",
                "pack://application:,,,/Images/film.png",
                "pack://application:,,,/Images/instant.png",
                "pack://application:,,,/Images/joie.png",
                "pack://application:,,,/Images/musique.png",
                "pack://application:,,,/Images/lettre.png",
                "pack://application:,,,/Images/liste.png",
                "pack://application:,,,/Images/message.png",
                "pack://application:,,,/Images/sourire.png",
                "pack://application:,,,/Images/surprise.png",


                // ➜ ajoute autant d’images que tu veux
            };
        }

        /// <summary>
        /// Événement déclenché au clic sur une étoile
        /// </summary>
        private void Case_Click(object sender, RoutedEventArgs e)
        {
            // Sécurité : plus de cartes disponibles
            if (imagesCartes.Count == 0)
            {
                MessageBox.Show("Toutes les cartes ont déjà été ouvertes 🎄");
                return;
            }

            // 🔹 Tirage aléatoire d’une image
            int indexImage = random.Next(imagesCartes.Count);
            string imageUri = imagesCartes[indexImage];

            // 🔹 Suppression des éléments utilisés (ANTI-DOUBLON)
            imagesCartes.RemoveAt(indexImage);

            // 🔹 Désactivation visuelle du bouton cliqué
            Button bouton = sender as Button;
            bouton.IsEnabled = false;
            bouton.Opacity = 0.5;

            // 🔹 Ouverture de la fenêtre JourWindow
            JourWindow window = new JourWindow(imageUri);
            window.ShowDialog();
        }
    }
}
                