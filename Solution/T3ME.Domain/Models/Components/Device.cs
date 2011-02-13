using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace T3ME.Domain.Models.Components
{
    public class Device
    {
        protected Device() { }

        public Device(string name, string url)
        {
            this.Name = name;
            this.Url = url;
        }

        public string Name { get; protected set; }
        public string Url { get; protected set; }

        public static Device FromAnchor(string anchorHtml)
        {
            Match match = Regex.Match(anchorHtml, @"<a(.*?)href=""(.*?)""(.*?)>(.*?)</a>", RegexOptions.IgnoreCase);

            Device device = new Device();

            device.Name = match.Groups[4].Value.Trim();
            device.Url = match.Groups[2].Value.Trim();

            return device;
        }
    }
}