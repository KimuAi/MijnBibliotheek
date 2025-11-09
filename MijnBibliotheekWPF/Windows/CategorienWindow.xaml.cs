using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using MijnBibliotheekModels.Data;
using MijnBibliotheekModels.Models;
using System.Collections.Generic;

namespace MijnBibliotheekWPF.Windows
{
    public partial class CategorienWindow : Window
    {
        private readonly BibliotheekContext _db;
        private bool _canEdit = false;

        public CategorienWindow(BibliotheekContext db)
        {
            InitializeComponent();
            _db = db;
            Loaded += async (_, __) => await LoadData();
        }

        public void SetPermissions(IList<string> roles)
        {
            _canEdit = roles.Contains("Admin") || roles.Contains("Medewerker");
            NaamBox.IsEnabled = _canEdit;
            AddBtn.IsEnabled = _canEdit;
            DeleteColumn.Visibility = _canEdit ? Visibility.Visible : Visibility.Collapsed;
        }

        public async Task LoadData()
        {
            Grid.ItemsSource = await _db.Categorieen.Where(c => !c.IsDeleted)
                .OrderBy(c => c.Naam).ToListAsync();
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e) => await LoadData();

        private async void Add_Click(object sender, RoutedEventArgs e)
        {
            if (!_canEdit) return;
            if (string.IsNullOrWhiteSpace(NaamBox.Text)) return;
            _db.Categorieen.Add(new Categorie { Naam = NaamBox.Text });
            await _db.SaveChangesAsync();
            NaamBox.Text = "";
            await LoadData();
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (!_canEdit) return;
            if (((FrameworkElement)sender).Tag is not Categorie c) return;
            c.IsDeleted = true;
            c.DeletedAt = DateTime.Now;
            await _db.SaveChangesAsync();
            await LoadData();
        }
    }
}
