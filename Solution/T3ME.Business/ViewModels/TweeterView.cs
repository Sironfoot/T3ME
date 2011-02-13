using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace T3ME.Business.ViewModels
{
    public class TweeterPanelView
    {
        public string Username { get; set; }
        public string ImageUrl { get; set; }
        public int TotalVotes { get; set; }
        public string ApplicationName { get; set; }
    }
}