using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace TurtleSharp.WPF.Helpers
{
    public static class AnimationExtensions
    {
        public static async Task BeginAsync(this Storyboard sb)
        {
            SemaphoreSlim signal = new SemaphoreSlim(0, 1);
            sb.Completed += delegate
            {
                signal.Release();
            };
            sb.Begin();
            await signal.WaitAsync();
        }

        public static async Task BeginAnimationAsync(this Animatable reciever, DependencyProperty dp, AnimationTimeline animation)
        {
            SemaphoreSlim signal = new SemaphoreSlim(0, 1);
            animation.Completed += delegate
            {
                signal.Release();
            };
            reciever.BeginAnimation(dp, animation);
            await signal.WaitAsync();
        }
        public static async Task BeginAsync(this Storyboard animation, CancellationToken token)
        {
            SemaphoreSlim signal = new SemaphoreSlim(0, 1);
            animation.Completed += delegate
            {
                signal.Release();
            };
            token.Register(() => animation.Stop());
            animation.Begin();
            await signal.WaitAsync();
        }

        public static async Task BeginAnimationAsync(this Animatable reciever, DependencyProperty dp, AnimationTimeline animation, CancellationToken token)
        {
            SemaphoreSlim signal = new SemaphoreSlim(0, 1);
            animation.Completed += delegate
            {
                signal.Release();
            };
            token.Register(() => animation.CreateClock().Controller.Stop());
            reciever.BeginAnimation(dp, animation);
            await signal.WaitAsync();
        }
    }
}
