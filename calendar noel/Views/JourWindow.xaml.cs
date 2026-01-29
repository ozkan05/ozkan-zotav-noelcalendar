using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace calendar_noel.Views
{
    public partial class JourWindow : Window
    {
        public JourWindow(string imageUri)
        {
            InitializeComponent();

            // Chargement de l’image
            CarteImage.Source = new BitmapImage(
                new Uri(imageUri, UriKind.Absolute));

            // Chemin du fichier texte
            string textFilePath = "messages.txt"; // adapte le chemin si nécessaire

            try
            {
                if (File.Exists(textFilePath))
                {
                    // Lire toutes les lignes du fichier
                    string[] lignes = File.ReadAllLines(textFilePath);

                    if (lignes.Length > 0)
                    {
                        // Sélectionner une ligne au hasard
                        Random rnd = new Random();
                        int index = rnd.Next(lignes.Length);
                        CarteTexte.Text = lignes[index];
                    }
                    else
                    {
                        CarteTexte.Text = "Le fichier est vide !";
                    }
                }
                else
                {
                    CarteTexte.Text = "Fichier messages.txt introuvable !";
                }
            }
            catch (Exception ex)
            {
                CarteTexte.Text = $"Erreur lors de la lecture : {ex.Message}";
            }
        }
    }
}
