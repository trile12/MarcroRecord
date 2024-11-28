using MarcroRecord.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput.Native;

namespace MarcroRecord.Helper
{
    public static class LogicHelper
    {
        public static MacroModel GetMarcroModelFromJson(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            var jsonContent = File.ReadAllText(filePath);
            var model = JsonConvert.DeserializeObject<MacroModel>(jsonContent);

            return model;
        }

        public static VirtualKeyCode? MapKey(string key)
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
    }
}
