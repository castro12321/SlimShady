using SlimShadyCore.Utilities;
using System;
using System.Collections.Generic;

namespace SlimShadyEssentials.Monitors.Hardware.vcp
{
    public class CapabilitiesNode
    {
        public readonly string name;
        public readonly string value;
        public Dictionary<string, CapabilitiesNode> children = new Dictionary<string, CapabilitiesNode>();

        public CapabilitiesNode(string name, string value)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new Exception("VCP CapabilitiesNode name cannot be empty");
            this.name = name;
            this.value = value;
        }

        public void Add(CapabilitiesNode node)
        {
            if (children.ContainsKey(node.name))
                NkLogger.error("node '" + this + "' already contains '" + node + "'");
            children.Add(node.name, node);
        }

        public bool Contains(string key)
        {
            return children.ContainsKey(key);
        }

        public CapabilitiesNode this[string key]
        {
            get => children[key];
        }

        public bool TryGetNode(string key, out CapabilitiesNode value)
        {
            return children.TryGetValue(key, out value);
        }

        public override string ToString()
        {
            return name + ": " + value;
        }
    }
}
