using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ContractObjects
{
    public enum FpsMode { FPS60, FPS45, FPS30 }
    public enum CpuMode { Single, Dual }

    [DataContract]
    public class DolphinOptions
    {
        [DataMember]
        public FpsMode FpsSelection { get; private set; }
        [DataMember]
        public CpuMode CpuModeSelection { get; private set; }
        [DataMember]
        public string DolphinVersion { get; set; }

        public DolphinOptions(FpsMode fpsSelection, CpuMode cpuModeSelection, string dolphinVersion)
        {
            FpsSelection = fpsSelection;
            CpuModeSelection = cpuModeSelection;
            DolphinVersion = dolphinVersion;
        }
    }
}
