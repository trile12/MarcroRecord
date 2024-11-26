
using Gma.System.MouseKeyHook;
using MarcroRecord.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;

namespace MarcroRecord
{
    public class MacroRecorderService
    {
        private IKeyboardMouseEvents _events;
        private ObservableCollection<MacroEvent> _macroEvents = new ObservableCollection<MacroEvent>();
        public ObservableCollection<MacroEvent> MacroEvents => _macroEvents;

        private DateTime _lastEventTime;

        public void StartRecording()
        {
            _macroEvents.Clear();
            _lastEventTime = DateTime.Now;

            _events = Hook.GlobalEvents();
            _events.KeyDown += OnKeyDown;
            //_events.MouseMove += OnMouseMove;
            _events.MouseDown += OnMouseDown;
        }

        public void StopRecording()
        {
            _events?.Dispose();
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

        private void OnKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            AddMacroEvent("KeyDown", e.KeyCode.ToString());
        }

        private void OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            AddMacroEvent("MouseMove", x: e.X, y: e.Y);
        }

        private void OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            AddMacroEvent("MouseDown", e.Button.ToString());
        }

        private void AddMacroEvent(string eventType, string key = null, int x = 0, int y = 0)
        {
            var delay = (int)(DateTime.Now - _lastEventTime).TotalMilliseconds;
            _macroEvents.Add(new MacroEvent
            {
                EventType = eventType,
                Key = key,
                X = x,
                Y = y,
                Delay = 20
            });
            _lastEventTime = DateTime.Now;
        }
    }
}
