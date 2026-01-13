using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace calendar_noel.Views
{
    /// <summary>
    /// Logique d'interaction pour JourWindow.xaml
    /// </summary>
    public partial class JourWindow : Window
    {
        public JourWindow(string imageUri)
        {
            InitializeComponent();
            // Chargement de l’image depuis les ressources WPF
            CarteImage.Source = new BitmapImage(
                new Uri(imageUri, UriKind.Relative));

            // Le texte a été retiré de la fenêtre (plus d'affichage de texte)
            // CarteTexte remains in XAML but is not used.
        }
    }
}


