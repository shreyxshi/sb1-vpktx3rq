using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CreativeCanvas.Services
{
    public class CanvasService
    {
        private ElementReference canvasRef;
        private IJSRuntime jsRuntime;
        private double width;
        private double height;
        
        public async Task InitializeAsync(ElementReference canvasElement, IJSRuntime jsRuntime)
        {
            this.canvasRef = canvasElement;
            this.jsRuntime = jsRuntime;
            
            // Initialize canvas and get dimensions
            await jsRuntime.InvokeVoidAsync("eval", @"
                window.canvasInterop = {
                    getContext: function(canvasId) {
                        const canvas = document.getElementById(canvasId);
                        if (!canvas) return null;
                        return canvas.getContext('2d');
                    },
                    clearCanvas: function(canvasId) {
                        const canvas = document.getElementById(canvasId);
                        if (!canvas) return;
                        const ctx = canvas.getContext('2d');
                        ctx.clearRect(0, 0, canvas.width, canvas.height);
                    },
                    resizeCanvas: function(canvasId) {
                        const canvas = document.getElementById(canvasId);
                        if (!canvas) return { width: 0, height: 0 };
                        
                        const parent = canvas.parentElement;
                        canvas.width = parent.clientWidth;
                        canvas.height = parent.clientHeight;
                        
                        return { width: canvas.width, height: canvas.height };
                    },
                    drawParticle: function(canvasId, x, y, size, color, alpha = 1) {
                        const canvas = document.getElementById(canvasId);
                        if (!canvas) return;
                        const ctx = canvas.getContext('2d');
                        
                        ctx.globalAlpha = alpha;
                        ctx.fillStyle = color;
                        ctx.beginPath();
                        ctx.arc(x, y, size, 0, Math.PI * 2);
                        ctx.fill();
                        ctx.globalAlpha = 1;
                    },
                    drawLine: function(canvasId, x1, y1, x2, y2, color, alpha = 0.2) {
                        const canvas = document.getElementById(canvasId);
                        if (!canvas) return;
                        const ctx = canvas.getContext('2d');
                        
                        ctx.globalAlpha = alpha;
                        ctx.strokeStyle = color;
                        ctx.lineWidth = 1;
                        ctx.beginPath();
                        ctx.moveTo(x1, y1);
                        ctx.lineTo(x2, y2);
                        ctx.stroke();
                        ctx.globalAlpha = 1;
                    },
                    saveImage: function(canvasId, fileName) {
                        const canvas = document.getElementById(canvasId);
                        if (!canvas) return;
                        
                        const link = document.createElement('a');
                        link.download = fileName;
                        link.href = canvas.toDataURL('image/png');
                        document.body.appendChild(link);
                        link.click();
                        document.body.removeChild(link);
                    }
                };
            ");
            
            // Resize canvas to fit container
            var dimensions = await ResizeCanvasAsync();
            width = dimensions.Width;
            height = dimensions.Height;
        }
        
        public async Task<(double Width, double Height)> ResizeCanvasAsync()
        {
            var dimensions = await jsRuntime.InvokeAsync<DimensionsResult>(
                "canvasInterop.resizeCanvas", canvasRef.Id);
            
            width = dimensions.width;
            height = dimensions.height;
            
            return (dimensions.width, dimensions.height);
        }
        
        public async Task ClearAsync()
        {
            await jsRuntime.InvokeVoidAsync("canvasInterop.clearCanvas", canvasRef.Id);
        }
        
        public async Task DrawParticleAsync(double x, double y, double size, string color, double alpha = 1.0)
        {
            await jsRuntime.InvokeVoidAsync(
                "canvasInterop.drawParticle", canvasRef.Id, x, y, size, color, alpha);
        }
        
        public async Task DrawLineAsync(double x1, double y1, double x2, double y2, string color, double alpha = 0.2)
        {
            await jsRuntime.InvokeVoidAsync(
                "canvasInterop.drawLine", canvasRef.Id, x1, y1, x2, y2, color, alpha);
        }
        
        public async Task SaveImageAsync(string fileName)
        {
            await jsRuntime.InvokeVoidAsync("canvasInterop.saveImage", canvasRef.Id, fileName);
        }
        
        public double Width => width;
        public double Height => height;
        
        private class DimensionsResult
        {
            public double width { get; set; }
            public double height { get; set; }
        }
    }
}