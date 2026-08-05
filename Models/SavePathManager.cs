using System;
using System.IO;

namespace WorldSimApp.Models;

public static class SavePathManager
{
    private static readonly string AppName = "WorldSimApp";

    private static string? _saveDirectory;
    
    public static string SaveDirectory
    {
        get
        {
            if (_saveDirectory == null)
            {
                var baseDir = Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData,
                    Environment.SpecialFolderOption.Create);
                _saveDirectory = Path.Combine(baseDir, AppName);
                
                if (!Directory.Exists(_saveDirectory))
                    Directory.CreateDirectory(_saveDirectory);
            }
            
            return _saveDirectory;
        }
    }

    public static string GameSavePath => Path.Combine(SaveDirectory, "WorldSimSave.json");
    
    public static string PlayerDataPath => Path.Combine(SaveDirectory, "PlayerCountryData.json");
}
