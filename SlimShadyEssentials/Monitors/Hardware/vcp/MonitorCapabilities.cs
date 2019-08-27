using System.Collections.Generic;

namespace SlimShadyEssentials.Monitors.Hardware.vcp
{
    public class MonitorCapabilities
    {
        public string MccsVersion { get; private set; }
        public string VcpName { get; private set; }
        public string ProtocolClass { get; private set; }
        public string DisplayType { get; private set; }
        public string Model { get; private set; }
        public CapabilitiesNode SupportedCommands { get; private set; }
        //public string Window { get; private set; }

        public MonitorCapabilities(CapabilitiesNode root)
        {
            CapabilitiesNode prot;
            if (root.TryGetNode("prot", out prot))
                ProtocolClass = prot.value;

            CapabilitiesNode type;
            if (root.TryGetNode("type", out type))
                DisplayType = type.value;

            CapabilitiesNode model;
            if (root.TryGetNode("model", out model))
                Model = model.value;

            CapabilitiesNode mccs_ver;
            if (root.TryGetNode("mccs_ver", out mccs_ver))
                MccsVersion = mccs_ver.value;

            CapabilitiesNode vcpname;
            if (root.TryGetNode("vcpname", out vcpname))
                VcpName = vcpname.value;

            CapabilitiesNode vcp;
            if (root.TryGetNode("vcp", out vcp))
                SupportedCommands = vcp;
        }

        public bool SupportsInputSource()
        {
            return SupportedCommands.Contains(Vcp.CODE_INPUT_SOURCE_STR);
        }

        public List<InputSource> GetSupportedInputSources()
        {
            List<InputSource> supportedInputSources = new List<InputSource>();

            CapabilitiesNode supportedInputSourcesNode = SupportedCommands[Vcp.CODE_INPUT_SOURCE_STR];
            foreach(string supportedInputSourceStr in supportedInputSourcesNode.children.Keys)
                supportedInputSources.Add(InputSourceExt.FromHex(supportedInputSourceStr));

            return supportedInputSources;
        }

        public override string ToString()
        {
            return "DisplayType: " + DisplayType + ", Model: " + Model + ", SupportedCommands: " + SupportedCommands;
        }
    }
}
