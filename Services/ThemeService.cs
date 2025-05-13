using System.Threading.Tasks;
using CreativeCanvas.Models;

namespace CreativeCanvas.Services
{
    public class ThemeService
    {
        public ColorTheme CurrentTheme { get; private set; } = new ColorTheme();
        
        public async Task SetThemeAsync(ColorTheme theme)
        {
            CurrentTheme = theme;
            await Task.CompletedTask; // For potential future async operations
        }
    }
}