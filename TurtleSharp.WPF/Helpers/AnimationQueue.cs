using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace TurtleSharp.WPF.Helpers
{
    public class AnimationQueue
    {
        private Action start;
        private object last;

        private object getLast() { return last; }

        private bool running = false;

        public void Start()
        {
            if (!running) {
                running = true;
                start.Invoke();
            }
        }

        public void Enqueue(Storyboard animation, FrameworkElement reciever)
        {
            Action begin = () => {
                animation.Begin(reciever);
            };

            animation.Completed += delegate
            {
                if (animation == getLast())
                {
                    start = null;
                    last = null;
                    running = false;
                }
            };

            if (last != null)
            {
                BindToLast(begin);
            }
            else
            {
                start = begin;
            }
            last = animation;
            Start();
        }

        internal void Enqueue(Storyboard animation)
        {
            Action begin = () => {
                animation.Begin();
            };

            animation.Completed += delegate
            {
                if (animation == getLast())
                {
                    start = null;
                    last = null;
                    running = false;
                }
            };

            if (last != null)
            {
                BindToLast(begin);
            }
            else
            {
                start = begin;
            }
            last = animation;
            Start();
        }

        public void Enqueue(AnimationTimeline animation, UIElement reciever, DependencyProperty dp)
        {

            Action begin = () => {
                reciever.BeginAnimation(dp, animation);
            };

            animation.Completed += delegate
            {
                if (animation == getLast())
                {
                    start = null;
                    last = null;
                    running = false;
                }
            };

            if (last != null)
            {
                BindToLast(begin);
            }
            else
            {
                start = begin;
            }
            last = animation;
            Start();
        }

        public void Enqueue(AnimationTimeline animation, Animatable reciever, DependencyProperty dp)
        {

            Action begin = () => {
                reciever.BeginAnimation(dp, animation);
            };

            animation.Completed += delegate
            {
                if (animation == getLast())
                {
                    start = null;
                    last = null;
                    running = false;
                }
            };

            if (last != null)
            {
                BindToLast(begin);
            }
            else
            {
                start = begin;
            }
            last = animation;
            Start();
        }

        private void BindToLast(Action action)
        {
            var that = last;
            if (last is Storyboard sb)
            {
                sb.Completed += delegate { action.Invoke(); };
            }
            else if (last is AnimationTimeline at)
            {
                at.Completed += delegate { action.Invoke(); };
            }
        }


        /*internal void EnqueueAnimationStop(DependencyProperty property, UIElement reciever)
        {
            reciever.BeginAnimation(property, null,);
        }*/
    }
}
