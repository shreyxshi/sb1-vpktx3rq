using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CreativeCanvas.Models;

namespace CreativeCanvas.Services
{
    public class ParticleSystemService
    {
        private List<Particle> particles = new List<Particle>();
        private CanvasService canvasService;
        private ThemeService themeService;
        private Random random = new Random();
        private CancellationTokenSource animationCts;
        
        public async Task InitializeAsync(CanvasService canvasService, ThemeService themeService)
        {
            this.canvasService = canvasService;
            this.themeService = themeService;
            
            // Create initial particles
            await ResetAsync(200, 3, 5, true, false);
        }
        
        public async Task ResetAsync(int count, double speed, double size, bool showConnections, bool useFlowField)
        {
            // Stop any existing animation loop
            animationCts?.Cancel();
            
            // Create new particles
            particles.Clear();
            for (int i = 0; i < count; i++)
            {
                var color = i % 2 == 0 
                    ? themeService.CurrentTheme.PrimaryColor 
                    : themeService.CurrentTheme.SecondaryColor;
                    
                particles.Add(new Particle(
                    canvasService.Width,
                    canvasService.Height,
                    size,
                    color
                ));
            }
            
            // Start animation loop
            await StartAnimationLoopAsync(speed, showConnections, useFlowField);
        }
        
        public async Task UpdateAsync(int count, double speed, double size, bool showConnections, bool useFlowField)
        {
            // Stop existing animation
            animationCts?.Cancel();
            
            // Adjust particle count
            if (particles.Count < count)
            {
                // Add more particles
                for (int i = particles.Count; i < count; i++)
                {
                    var color = i % 2 == 0 
                        ? themeService.CurrentTheme.PrimaryColor 
                        : themeService.CurrentTheme.SecondaryColor;
                        
                    particles.Add(new Particle(
                        canvasService.Width,
                        canvasService.Height,
                        size,
                        color
                    ));
                }
            }
            else if (particles.Count > count)
            {
                // Remove excess particles
                particles.RemoveRange(count, particles.Count - count);
            }
            
            // Update particle sizes
            foreach (var particle in particles)
            {
                particle.Size = size;
            }
            
            // Start new animation loop
            await StartAnimationLoopAsync(speed, showConnections, useFlowField);
        }
        
        public async Task UpdateColorsAsync()
        {
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Color = i % 2 == 0 
                    ? themeService.CurrentTheme.PrimaryColor 
                    : themeService.CurrentTheme.SecondaryColor;
            }
        }
        
        private async Task StartAnimationLoopAsync(double speed, bool showConnections, bool useFlowField)
        {
            animationCts = new CancellationTokenSource();
            var token = animationCts.Token;
            
            try
            {
                while (!token.IsCancellationRequested)
                {
                    // Clear canvas
                    await canvasService.ClearAsync();
                    
                    // Update and draw particles
                    foreach (var particle in particles)
                    {
                        particle.Update(canvasService.Width, canvasService.Height, speed, useFlowField);
                        await canvasService.DrawParticleAsync(
                            particle.X, particle.Y, particle.Size, particle.Color, particle.Alpha);
                    }
                    
                    // Draw connections if enabled
                    if (showConnections)
                    {
                        const double maxDistance = 100;
                        
                        for (int i = 0; i < particles.Count; i++)
                        {
                            for (int j = i + 1; j < particles.Count; j++)
                            {
                                var p1 = particles[i];
                                var p2 = particles[j];
                                
                                var dx = p1.X - p2.X;
                                var dy = p1.Y - p2.Y;
                                var distance = Math.Sqrt(dx * dx + dy * dy);
                                
                                if (distance < maxDistance)
                                {
                                    // The closer particles are, the more opaque the line
                                    var alpha = 1 - (distance / maxDistance);
                                    await canvasService.DrawLineAsync(
                                        p1.X, p1.Y, p2.X, p2.Y, 
                                        themeService.CurrentTheme.PrimaryColor, 
                                        alpha * 0.2);
                                }
                            }
                        }
                    }
                    
                    // Wait for next frame
                    await Task.Delay(16); // ~60fps
                }
            }
            catch (TaskCanceledException)
            {
                // Animation was cancelled, which is expected
            }
        }
    }
}