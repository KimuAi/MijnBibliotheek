using Microsoft.EntityFrameworkCore;
using MijnBibliotheekModels.Data;
using MijnBibliotheekModels.Identity;
using MijnBibliotheekModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MijnBibliotheekWPF.Windows
{
    public partial class UitleningWindow : Window
    {
        private readonly BibliotheekContext _db;
        private bool _canEdit = false;

        public UitleningWindow(BibliotheekContext db)
        {
            InitializeComponent();
            _db = db;
            Loaded += async (_, __) => await LoadData();
        }

        public void SetPermissions(IList<string> roles)
        {
            _canEdit = roles.Contains("Admin") || roles.Contains("Medewerker");
            BoekSelect.IsEnabled = _canEdit;
            UserSelect.IsEnabled = _canEdit;
            StartPicker.IsEnabled = _canEdit;
            EndPicker.IsEnabled = _canEdit;
            OpslaanBtn.IsEnabled = _canEdit;
            TerugColumn.Visibility = _canEdit ? Visibility.Visible : Visibility.Collapsed;
        }

        public async Task LoadData()
        {
            BoekSelect.ItemsSource = await _db.Boeken.Where(b => b.IsBeschikbaar && !b.IsDeleted).OrderBy(b => b.Titel).ToListAsync();
            UserSelect.ItemsSource = await _db.Users.OrderBy(u => u.VolledigeNaam).Select(u => new AppUser { Id = u.Id, VolledigeNaam = u.VolledigeNaam }).ToListAsync();
            Grid.ItemsSource = await _db.Uitleningen.Include(b => b.Boek).Include(b => b.AppUser).OrderByDescending(b => b.StartDatum).ToListAsync();
        }

        private async void Opslaan_Click(object sender, RoutedEventArgs e)
        {
            if (!_canEdit) return;
            if (BoekSelect.SelectedItem is not Boek boek) return;
            if (UserSelect.SelectedItem is not AppUser user) return;

            boek.IsBeschikbaar = false;
            _db.Uitleningen.Add(new Uitlening
            {
                BoekId = boek.Id,
                AppUserId = user.Id,
                StartDatum = StartPicker.SelectedDate ?? DateTime.Today,
                EindDatum = EndPicker.SelectedDate,
                IsTeruggebracht = false
            });

            await _db.SaveChangesAsync();
            await LoadData();
        }

        private async void Terug_Click(object sender, RoutedEventArgs e)
        {
            if (!_canEdit) return;
            if (((FrameworkElement)sender).Tag is not Uitlening u) return;
            u.IsTeruggebracht = true;
            (await _db.Boeken.FindAsync(u.BoekId)).IsBeschikbaar = true;
            await _db.SaveChangesAsync();
            await LoadData();
        }
    }
}
