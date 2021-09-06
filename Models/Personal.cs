using System;
using System.Collections.Generic;

namespace UniversityAssistantBlazorWasm.Models
{
    public class Personal
    {
        public List<string> Subjects { get; set; }
        public List<DateTime> StartTimes { get; set; }
        public List<DateTime> EndTimes { get; set; }
    }
}
