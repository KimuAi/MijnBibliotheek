using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using MijnBibliotheekModels.Data;
using MijnBibliotheekModels.Models;

namespace MijnBibliotheekWPF.Windows
{
    public partial class AddBoekWindow : Window
    {
        private readonly BibliotheekContext _db;

        public AddBoekWindow(BibliotheekContext db)
        {
            InitializeComponent();
            _db = db;

            Loaded += async (_, __) =>
            {
                CategorieSelect.ItemsSource = await _db.Categorieen
                    .Where(c => !c.IsDeleted)
                    .OrderBy(c => c.Naam)
                    .ToListAsync();
            };
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitelBox.Text) ||
                string.IsNullOrWhiteSpace(AuteurBox.Text) ||
                string.IsNullOrWhiteSpace(ISBNBox.Text) ||
                CategorieSelect.SelectedValue is null)
            {
                MessageBox.Show("Vul alles in.");
                return;
            }

            _db.Boeken.Add(new Boek
            {
                Titel = TitelBox.Text.Trim(),
                Auteur = AuteurBox.Text.Trim(),
                ISBN = ISBNBox.Text.Trim(),
                CategorieId = (int)CategorieSelect.SelectedValue,
                IsBeschikbaar = true
            });

            await _db.SaveChangesAsync();
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
            => Close();
    }
}
