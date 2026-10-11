using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Taxi.Core.Models.Map;
using Taxi.src.Taxi.Map.Helpers;
using Taxi.src.Services;
using Taxi.WPF.Commands;

namespace Taxi.WPF.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string btnContent;
        public string BtnContent
        {
            get
            {
                return btnContent;
            }
            set
            {
                btnContent = value;
                OnPropertyChanged(nameof(BtnContent));
            }
        }

        private string thems { get; set; }
        private List<string> styles { get; set; }

        private RelayCommand _btnThemsCommand;

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public RelayCommand BtnThemsCommand
        {
            get
            {
                return _btnThemsCommand ??
                    (_btnThemsCommand = new RelayCommand(obj =>
                    {
                        string style = thems == "light" ? "dark" : "light";

                        var uri = new Uri("Thems/" + style + ".xaml", UriKind.Relative);

                        ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;

                        Application.Current.Resources.Clear();

                        Application.Current.Resources.MergedDictionaries.Add(resourceDict);

                        thems = style;
                        BtnContent = style;
                    }));
            }
        }
        public MainViewModel() {
            styles = new List<string> { "light", "dark" };
            thems = styles[0];
            BtnContent = styles[0];
        }
    }
}
