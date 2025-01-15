using System;
#if UNITY_5_5_OR_NEWER
using Noesis;
#else
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
#endif

namespace Caliburn.Noesis
{
    /// <summary>
    /// An <see cref="AdornerDecorator"/> which allows to overlay elements with a <see cref="DialogHost"/>.
    /// The additional element will be rendered on top of the regular <see cref="AdornerLayer"/>.
    /// </summary>
    public class DialogAdornerDecorator : AdornerDecorator
    {
        private readonly DialogHost _dialogHost = new();

        public DialogAdornerDecorator()
        {
            AddVisualChild(_dialogHost);
        }

        /// <summary>Gets the <see cref="Visual"/> children count.</summary>
        protected override int VisualChildrenCount
        {
            get
            {
                return Child is null ? 0 : 3;
            }
        }

#if UNITY_5_5_OR_NEWER
        private AdornerLayer AdornerLayer
        {
            get
            {
                for (int i = 0; i < base.VisualChildrenCount; i++)
                {
                    if (base.GetVisualChild(i) is AdornerLayer layer)
                    {
                        return layer;
                    }
                }
                
                return null;
            }
        }
#endif

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
                    return AdornerLayer;

                case 2:
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
