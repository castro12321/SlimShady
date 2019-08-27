using System;

namespace SlimShadyEssentials.Monitors.Hardware.vcp
{
    public class MonitorCapabilitiesParser
    {
        public static MonitorCapabilities ParseVcpCapabilities(string capsRaw)
        {
            capsRaw = capsRaw.Substring("(".Length, capsRaw.Length - ")".Length - 1); // Get rid of '(' and ')'
            CapabilitiesNode root = ParseVcpCapabilitiesNode("root", capsRaw);
            return new MonitorCapabilities(root);
        }

        private static CapabilitiesNode ParseVcpCapabilitiesNode(string rootName, string rootContent)
        {
            CapabilitiesNode root = new CapabilitiesNode(rootName, rootContent);

            rootContent += " "; // To match last node

            string key = "";
            for (int i = 0; i < rootContent.Length; ++i)
            {
                char c = rootContent[i];
                if (c == ' ')
                {
                    if (String.IsNullOrEmpty(key))
                        continue;
                    root.Add(new CapabilitiesNode(key, ""));
                    key = "";
                }
                else if (c == '(')
                {
                    string val = ParseVcpParenthesis(rootContent, i);
                    i += val.Length + ")".Length;
                    CapabilitiesNode node = ParseVcpCapabilitiesNode(key, val);
                    root.Add(node);
                    key = "";
                }
                else
                {
                    key += c;
                }
            }

            return root;
        }

        private static string ParseVcpParenthesis(string text, int parenthesisStart)
        {
            // Num of open parenthesis
            // Increment for each '(', decrement for each ')' character
            // Once we reach counter == 0, we're done
            int open = 0;

            int valEndIdx = parenthesisStart;
            for (; ; valEndIdx++)
            {
                char c = text[valEndIdx];
                if (c == '(')
                    open++;
                if (c == ')')
                    open--;
                if (open == 0)
                {
                    break;
                }
            }

            return text.Substring(parenthesisStart + 1, valEndIdx - parenthesisStart - 1);
        }
    }
}
