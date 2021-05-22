namespace Caliburn.Noesis.Transitions
{
    using System;
    using System.Windows;
    using System.Windows.Markup;
    using System.Windows.Media.Animation;

    /// <summary>Base class for a wipe transition.</summary>
    /// <seealso cref="MarkupExtension" />
    /// <seealso cref="ITransitionWipe" />
    public abstract class TransitionWipeBase : MarkupExtension, ITransitionWipe
    {
        #region Constants and Fields

        private TimeSpan delay = TimeSpan.Zero;
        private TimeSpan duration = TimeSpan.FromMilliseconds(500);
        private IEasingFunction easingFunction = new SineEase();
        private ITransitionEffect fromEffect;
        private ITransitionEffect toEffect;

        #endregion

        #region Protected Properties

        /// <summary>Gets the effect to apply to the item the wipe is transitioning from.</summary>
        /// <value>The effect to apply to the item the wipe is transitioning from.</value>
        protected ITransitionEffect FromEffect
        {
            get => this.fromEffect;
            set
            {
                if (Equals(this.fromEffect, value))
                {
                    return;
                }

                this.fromEffect = value;

                if (this.fromEffect != null)
                {
                    this.fromEffect.Delay = Delay;
                    this.fromEffect.Duration = Duration;
                    this.fromEffect.EasingFunction = EasingFunction;
                }
            }
        }

        /// <summary>Gets the effect to apply to the item the wipe is transitioning to.</summary>
        /// <value>The effect to apply to the item the wipe is transitioning to.</value>
        protected ITransitionEffect ToEffect
        {
            get => this.toEffect;
            set
            {
                if (Equals(this.toEffect, value))
                {
                    return;
                }

                this.toEffect = value;

                if (this.toEffect != null)
                {
                    this.toEffect.Delay = Delay;
                    this.toEffect.Duration = Duration;
                    this.toEffect.EasingFunction = EasingFunction;
                }
            }
        }

        #endregion

        #region ITransitionWipe Implementation

        /// <inheritdoc />
        public TimeSpan Delay
        {
            get => this.delay;
            set
            {
                if (Equals(this.delay, value))
                {
                    return;
                }

                this.delay = value;

                if (FromEffect != null)
                {
                    FromEffect.Delay = Delay;
                }

                if (ToEffect != null)
                {
                    ToEffect.Delay = Delay;
                }
            }
        }

        /// <inheritdoc />
        public TimeSpan Duration
        {
            get => this.duration;
            set
            {
                if (Equals(this.duration, value))
                {
                    return;
                }

                this.duration = value;

                if (FromEffect != null)
                {
                    FromEffect.Duration = Duration;
                }

                if (ToEffect != null)
                {
                    ToEffect.Duration = Duration;
                }
            }
        }

        /// <inheritdoc />
        public IEasingFunction EasingFunction
        {
            get => this.easingFunction;
            set
            {
                if (Equals(this.easingFunction, value))
                {
                    return;
                }

                this.easingFunction = value;

                if (FromEffect != null)
                {
                    FromEffect.EasingFunction = EasingFunction;
                }

                if (ToEffect != null)
                {
                    ToEffect.EasingFunction = EasingFunction;
                }
            }
        }

        /// <inheritdoc />
        public virtual void Wipe(TransitionerItem fromItem,
                                 TransitionerItem toItem,
                                 Point origin,
                                 IZIndexController zIndexController)
        {
            if (fromItem == null)
            {
                throw new ArgumentNullException(nameof(fromItem));
            }

            if (toItem == null)
            {
                throw new ArgumentNullException(nameof(toItem));
            }

            if (zIndexController == null)
            {
                throw new ArgumentNullException(nameof(zIndexController));
            }

            fromItem.TransitionEffect = FromEffect;
            toItem.TransitionEffect = ToEffect;

            fromItem.PerformTransition();
            toItem.PerformTransition();

            zIndexController.Stack(toItem, fromItem);
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        #endregion
    }
}