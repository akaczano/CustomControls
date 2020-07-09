using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    class SettingsAttribute : Attribute
    {

        private string displayName;

        public virtual string DisplayName {
            get {
                return displayName;
            }
        }

        public SettingsAttribute(string displayName) {
            this.displayName = displayName;
        }

    }
}
