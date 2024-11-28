using MarcroRecord.Helper;
using MarcroRecord.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MarcroRecord.ViewModel
{
    public class MainViewModel
    {
        private MacroRecorderService _macroService = new MacroRecorderService();
        public bool IsRecording = false;

        public System.Windows.Input.ICommand RecordCommand { get; }

        public MainViewModel()
        {
            _macroService.KeyDown += OnKeyDown;
            _macroService.MouseDown += OnMouseDown;

            RecordCommand = new RelayCommand(ExecuteRecordCommand);
        }

        private void ExecuteRecordCommand()
        {
            IsRecording = !IsRecording;
        }

        public void Start()
        {
            IsRecording = true;
            _macroEvents.Clear();
            _macroService.StartRecording();
        }
        public void Stop()
        {
            IsRecording = false;
            _macroService.StopRecording();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (IsRecording)
            {
                AddMacroEvent("KeyDown", e.KeyCode.ToString());
            }
            else
            {

            }
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (IsRecording)
            {
                AddMacroEvent("MouseDown", e.Button.ToString());
            }
            else
            {

            }
        }

        private void AddMacroEvent(string eventType, string key = null, int x = 0, int y = 0)
        {
            _macroEvents.Add(new MacroEvent
            {
                EventType = eventType,
                Key = key,
                Delay = 20
            });
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
        #endregion
    }
}
