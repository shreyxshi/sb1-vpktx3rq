using System;

namespace CreativeCanvas.Models
{
    public class Particle
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double VelocityX { get; set; }
        public double VelocityY { get; set; }
        public double Size { get; set; }
        public string Color { get; set; }
        public double Alpha { get; set; } = 1.0;
        
        private Random random = new Random();
        
        public Particle(double canvasWidth, double canvasHeight, double size, string color)
        {
            X = random.NextDouble() * canvasWidth;
            Y = random.NextDouble() * canvasHeight;
            
            // Random velocity between -1 and 1
            VelocityX = (random.NextDouble() * 2 - 1) * 0.5;
            VelocityY = (random.NextDouble() * 2 - 1) * 0.5;
            
            Size = size;
            Color = color;
        }
        
        public void Update(double canvasWidth, double canvasHeight, double speed, bool useFlowField)
        {
            if (useFlowField)
            {
                // Simple flow field effect using noise (simplified version)
                double angle = (Math.Sin(X * 0.01) + Math.Cos(Y * 0.01)) * Math.PI;
                VelocityX = Math.Cos(angle) * 0.5;
                VelocityY = Math.Sin(angle) * 0.5;
            }
            
            X += VelocityX * speed;
            Y += VelocityY * speed;
            
            // Bounce off the edges
            if (X < 0 || X > canvasWidth)
            {
                VelocityX *= -1;
                X = Math.Clamp(X, 0, canvasWidth);
            }
            
            if (Y < 0 || Y > canvasHeight)
            {
                VelocityY *= -1;
                Y = Math.Clamp(Y, 0, canvasHeight);
            }
        }
    }
}