using System;
#if UNITY_5_5_OR_NEWER
using Noesis;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
#endif

namespace Caliburn.Noesis
{
    /// <summary>
    /// A <see cref="Decorator"/> which allows to overlay elements with a <see cref="DialogHost"/>.
    /// </summary>
    public class DialogHostDecorator : Decorator
    {
        private readonly DialogHost _dialogHost = new();

        public DialogHostDecorator()
        {
            AddVisualChild(_dialogHost);
        }

        /// <summary>Gets the <see cref="Visual"/> children count.</summary>
        protected override int VisualChildrenCount
        {
            get
            {
                return Child is null ? 0 : 2;
            }
        }

        /// <inheritdoc/>
        protected override Size ArrangeOverride(Size finalSize)
        {
            base.ArrangeOverride(finalSize);
            var finalSizeRect = new Rect(finalSize);

            _dialogHost.Arrange(finalSizeRect);

            return finalSize;
        }

        /// <inheritdoc/>
        protected override Visual GetVisualChild(int index)
        {
            if (Child == null)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            switch (index)
            {
                case 0:
                    return Child;

                case 1:
                    return _dialogHost;

                default:
                    throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size constraint)
        {
            var desiredSize = base.MeasureOverride(constraint);

            _dialogHost.Measure(constraint);

            return desiredSize;
        }
    }
}
