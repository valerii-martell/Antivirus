using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Antivirus
{
    public static class IconReplacer
    {
        public static void IconReplace(this Window window, string iconFileName)
        {
            Uri iconUri = new Uri(AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug", "").Replace("\\bin\\Release", "") + "/Resources/" + iconFileName, UriKind.RelativeOrAbsolute);
            window.Icon = BitmapFrame.Create(iconUri);
        }
    }
}
