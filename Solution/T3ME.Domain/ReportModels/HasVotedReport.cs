using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace T3ME.Domain.ReportModels
{
    public class HasVotedReport
    {
        public long TwitterId { get; set; }
        public bool HasVoted { get; set; }
    }
}