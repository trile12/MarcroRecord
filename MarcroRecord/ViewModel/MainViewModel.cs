using MarcroRecord.Helper;
using MarcroRecord.Model;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;

namespace MarcroRecord.ViewModel
{
    public class MainViewModel
    {
        public MainViewModel()
        {
        }

        #region Properties

        private ObservableCollection<MacroEvent> _macroEvents = new ObservableCollection<MacroEvent>();
        public ObservableCollection<MacroEvent> MacroEvents => _macroEvents;

        private ObservableCollection<MacroModel> _macroModels = new ObservableCollection<MacroModel>();

        public ObservableCollection<MacroModel> MacroModels
        {
            get => _macroModels;
            set
            {
                if (_macroModels != value)
                {
                    if (_macroModels != null)
                        UnsubscribeFromCollection(_macroModels);

                    _macroModels = value;

                    if (_macroModels != null)
                        SubscribeToCollection(_macroModels);
                }
            }
        }

        private void SubscribeToCollection(ObservableCollection<MacroModel> collection)
        {
            foreach (var item in collection)
                item.PropertyChanged += OnMacroModelPropertyChanged;

            collection.CollectionChanged += OnMacroModelsCollectionChanged;
        }

        private void UnsubscribeFromCollection(ObservableCollection<MacroModel> collection)
        {
            foreach (var item in collection)
                item.PropertyChanged -= OnMacroModelPropertyChanged;

            collection.CollectionChanged -= OnMacroModelsCollectionChanged;
        }

        private void OnMacroModelsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (MacroModel item in e.OldItems)
                    item.PropertyChanged -= OnMacroModelPropertyChanged;
            }

            if (e.NewItems != null)
            {
                foreach (MacroModel item in e.NewItems)
                    item.PropertyChanged += OnMacroModelPropertyChanged;
            }
        }

        private void OnMacroModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MacroModel.Key))
            {
                var changedMacro = (MacroModel)sender;
                Console.WriteLine($"Key changed: {changedMacro.Key}");
            }
        }

        #endregion Properties
    }
}