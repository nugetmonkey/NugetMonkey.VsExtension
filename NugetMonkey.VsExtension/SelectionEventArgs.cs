using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetMonkey.VsExtension
{
    public class SelectionEventArgs : EventArgs
    {
        public List<Doc> Selection { get; set; }
    }
}
