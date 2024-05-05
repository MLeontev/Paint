using System;
using System.Drawing;
using System.Threading.Tasks;

namespace PluginInterface
{
    public interface IPlugin
    {
        string Name { get; }
        string Author { get; }
        void Transform(Bitmap bitmap);
        event EventHandler Filtered;
    }
}
