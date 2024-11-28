using Gma.System.MouseKeyHook;
using MarcroRecord.Helper;
using MarcroRecord.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;

namespace MarcroRecord
{
    public class MacroRecorderService
    {
        public event EventHandler<KeyEventArgs> KeyDown;
        public event EventHandler<MouseEventArgs> MouseDown;

        public IKeyboardMouseEvents KeyEvents;

        public void StartRecording()
        {
            KeyEvents = Hook.GlobalEvents();
            KeyEvents.KeyDown += OnKeyDown;
            KeyEvents.MouseDown += OnMouseDown;
        }

        public void StopRecording()
        {
            if (KeyEvents == null) return;
            KeyEvents.KeyDown -= OnKeyDown;
            KeyEvents.MouseDown -= OnMouseDown;
            //KeyEvents?.Dispose();
        }

        public async Task PlayMacroAsync(List<MacroEvent> events)
        {
            var simulator = new InputSimulator();

            foreach (var macroEvent in events)
            {
                await Task.Delay(macroEvent.Delay);

                switch (macroEvent.EventType)
                {
                    case "KeyDown":
                        var keycode = LogicHelper.MapKey(macroEvent.Key.ToString());
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

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            KeyDown?.Invoke(this, e);
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            MouseDown?.Invoke(this, e);
        }
    }
}