using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using ContractObjects;

namespace ClientApplication.ViewModel
{
    public class LobbySettings : IDataErrorInfo
    {
        public string LobbyName { get; set; }
        public int MaxPlayers { get; set; }

        public string DolphinVersion { get; set; }
        public CpuMode SelectedCpuMode { get; set; }
        public FpsMode SelectedFpsMode { get; set; }

        public Dictionary<CpuMode, string> CpuModeOptions { get; private set; }
        public Dictionary<FpsMode, string> FpsModeOptions { get; private set; }

        public LobbySettings()
        {
            FpsModeOptions = new Dictionary<FpsMode, string>();
            FpsModeOptions[FpsMode.FPS60] = "60 FPS (Full Speed)";
            FpsModeOptions[FpsMode.FPS45] = "45 FPS Hack";
            FpsModeOptions[FpsMode.FPS30] = "30 FPS Hack";

            CpuModeOptions = new Dictionary<CpuMode, string>();
            CpuModeOptions[CpuMode.Single] = "Single Core";
            CpuModeOptions[CpuMode.Dual] = "Dual Core";
        }
        
        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        public string this[string columnName]
        {
            get
            {
                string errorMsg = null;
                switch (columnName)
                {
                    case "LobbyName":
                        if (LobbyName.Length < 5|| LobbyName.Length > 40) errorMsg = "Lobby name must be at least 5 characters long and less than 40 characters long.";
                        break;
                    case "DolphinVersion":
                        if (DolphinVersion.Length < 1 || DolphinVersion.Length > 10) errorMsg = "DolphinVersion must be at least 1 character long and less than 10 characters long.";
                        break;
                    case "MaxPlayers":
                        if (MaxPlayers < 2) errorMsg = "MaxPlayers must be at least 2";
                        break;
                }

                return errorMsg;
            }
        }
    }
}
