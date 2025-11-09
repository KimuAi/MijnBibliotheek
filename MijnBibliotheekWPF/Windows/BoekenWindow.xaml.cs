using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using MijnBibliotheekModels.Data;
using MijnBibliotheekModels.Models;
using System.Collections.Generic;

namespace MijnBibliotheekWPF.Windows
{
    public partial class BoekenWindow : Window
    {
        private readonly BibliotheekContext _db;
        private bool _canEdit = false;
        private List<Categorie> _catItems = new();

        public BoekenWindow(BibliotheekContext db)
        {
            InitializeComponent();
            _db = db;
            Loaded += async (_, __) => { await LoadCategoriesAsync(); await LoadAsync(); };
        }

        public void SetPermissions(IList<string> roles)
        {
            _canEdit = roles.Contains("Admin") || roles.Contains("Medewerker");

            NieuwBtn.IsEnabled = _canEdit;
            OpslaanBtn.IsEnabled = _canEdit;
            DeleteColumn.Visibility = _canEdit ? Visibility.Visible : Visibility.Collapsed;
            Grid.IsReadOnly = !_canEdit;
        }

        private async Task LoadCategoriesAsync()
        {
            _catItems = await _db.Categorieen
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.Naam)
                .ToListAsync();

            CatFilter.ItemsSource = _catItems;
            ColCategorie.ItemsSource = _catItems;
        }

        private async Task LoadAsync()
        {
            var q = _db.Boeken
                .Include(b => b.Categorie)
                .Where(b => !b.IsDeleted);

            if (CatFilter.SelectedItem is Categorie cat)
                q = q.Where(b => b.CategorieId == cat.Id);

            if (!string.IsNullOrWhiteSpace(ZoekBox.Text))
            {
                var txt = ZoekBox.Text.ToLower();
                q = q.Where(b =>
                    b.Titel.ToLower().Contains(txt) ||
                    b.Auteur.ToLower().Contains(txt) ||
                    b.ISBN.ToLower().Contains(txt));
            }

            Grid.ItemsSource = await q.OrderBy(b => b.Titel).ToListAsync();
        }

        // ✅ HIER VERVANGEN WE — NIEUW BOEK → POPUP VENSTER
        private async void Nieuw_Click(object sender, RoutedEventArgs e)
        {
            var wnd = new AddBoekWindow(_db);
            wnd.Owner = this;

            // Als gebruiker opslaat → refresh
            if (wnd.ShowDialog() == true)
                await LoadAsync();
        }

        private async void Opslaan_Click(object sender, RoutedEventArgs e)
        {
            await _db.SaveChangesAsync();
            await LoadAsync();
            MessageBox.Show("Wijzigingen opgeslagen.");
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).Tag is not Boek b) return;

            b.IsDeleted = true;
            b.DeletedAt = DateTime.Now;
            await _db.SaveChangesAsync();
            await LoadAsync();
        }

        private async void ZoekBox_TextChanged(object sender, TextChangedEventArgs e) => await LoadAsync();
        private async void CatFilter_SelectionChanged(object sender, SelectionChangedEventArgs e) => await LoadAsync();
        private async void Refresh_Click(object sender, RoutedEventArgs e) => await LoadAsync();
    }
}
