using Gma.System.MouseKeyHook;
using MarcroRecord.Model;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;

namespace MarcroRecord
{
    public class MacroRecorderService
    {
        private ObservableCollection<MacroEvent> _macroEvents = new ObservableCollection<MacroEvent>();
        public ObservableCollection<MacroEvent> MacroEvents => _macroEvents;

        public event EventHandler<KeyEventArgs> KeyDown;
        public event EventHandler<MouseEventArgs> MouseDown;

        public IKeyboardMouseEvents KeyEvents;
        private bool _isRecording = false;
        private DateTime _lastEventTime;

        public void StartRecording()
        {
            _isRecording = true;
            _macroEvents.Clear();
            _lastEventTime = DateTime.Now;

            KeyEvents = Hook.GlobalEvents();
            KeyEvents.KeyDown += OnKeyDown;
            KeyEvents.MouseDown += OnMouseDown;
        }

        public void StopRecording()
        {
            _isRecording = false;
            if (KeyEvents == null) return;

            KeyEvents.KeyDown -= OnKeyDown;
            KeyEvents.MouseDown -= OnMouseDown;
            //KeyEvents?.Dispose();
        }

        public async Task PlayMacroAsync()
        {
            var simulator = new InputSimulator();

            foreach (var macroEvent in _macroEvents)
            {
                await Task.Delay(macroEvent.Delay);

                switch (macroEvent.EventType)
                {
                    case "KeyDown":
                        var keycode = MapKey(macroEvent.Key.ToString());
                        if (keycode != null)
                        {
                            simulator.Keyboard.KeyPress(keycode.Value);
                        }
                        break;

                    case "MouseDown":
                        simulator.Mouse.LeftButtonClick();
                        break;
                }
            }
        }

        private VirtualKeyCode? MapKey(string key)
        {
            if (key.StartsWith("D") && key.Length == 2 && char.IsDigit(key[1]))
            {
                return Enum.TryParse("VK_" + key[1], out VirtualKeyCode numberKey) ? numberKey : 0;
            }

            if (key.Length == 1 && char.IsLetter(key[0]))
            {
                return Enum.TryParse("VK_" + key.ToUpper(), out VirtualKeyCode letterKey) ? letterKey : 0;
            }

            if (key.Equals("Enter", StringComparison.OrdinalIgnoreCase))
                return VirtualKeyCode.RETURN;
            if (key.Equals("Space", StringComparison.OrdinalIgnoreCase))
                return VirtualKeyCode.SPACE;

            return null;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (_isRecording)
            {
                AddMacroEvent("KeyDown", e.KeyCode.ToString());
                KeyDown?.Invoke(this, e);
            }
            else
            {

            }
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (_isRecording)
            {
                AddMacroEvent("MouseDown", e.Button.ToString());
                MouseDown?.Invoke(this, e);
            }
            else
            {

            }
        }

        private void AddMacroEvent(string eventType, string key = null, int x = 0, int y = 0)
        {
            var delay = (int)(DateTime.Now - _lastEventTime).TotalMilliseconds;
            _macroEvents.Add(new MacroEvent
            {
                EventType = eventType,
                Key = key,
                Delay = 20
            });
            _lastEventTime = DateTime.Now;
        }
    }
}