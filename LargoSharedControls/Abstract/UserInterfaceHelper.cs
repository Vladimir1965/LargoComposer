// <copyright file="UserInterfaceHelper.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedControls.Abstract
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;

    /// <summary>
    /// Helper methods for UI-related tasks.
    /// </summary>
    public static class UserInterfaceHelper
    {
        /// <summary>
        /// The default icon.
        /// </summary>
        public static Image DefaultIcon = new Image {
            Source = new BitmapImage(new Uri("pack://application:,,,/LargoSharedControls;component/Images/icon_emptygray.png"))
        };

        /// <summary>
        /// Empty Delegate.
        /// </summary>
        private static readonly Action EmptyDelegate = () => {
        };

        /// <summary>
        /// Icons the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static Image Icon(string name) {
            return new Image { Source = new BitmapImage(new Uri("pack://application:,,,/LargoSharedControls;component/Images/" + name + ".png")) };
        }

        /// <summary>
        /// Refreshes the specified UI element.
        /// </summary>
        /// <param name="givenElement">The given element.</param>
        public static void Refresh(this UIElement givenElement) {
            givenElement.Dispatcher?.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }

        #region find children

        /// <summary>
        /// Analyzes both visual and logical tree in order to find all elements of a given
        /// type that are descendants of the <paramref name="source"/> item.
        /// </summary>
        /// <typeparam name="T">The type of the queried items.</typeparam>
        /// <param name="source">The root element that marks the source of the search. If the
        /// source is already of the requested type, it will not be included in the result.</param>
        /// <returns>
        /// All descendants of <paramref name="source"/> that match the requested type.
        /// </returns>
        public static IEnumerable<T> FindChildren<T>(this DependencyObject source) where T : DependencyObject {
            if (source == null) {
                yield break;
            }

            var childObjects = GetChildObjects(source);
            foreach (var child in childObjects) {
                //// analyze if children match the requested type  //// child != null &&
                if (child is T children) {
                    yield return children;
                }

                //// recurse tree
                foreach (var descendant in FindChildren<T>(child)) {
                    yield return descendant;
                }
            }
        }

        /// <summary>
        /// This method is an alternative to 
        /// <see cref="VisualTreeHelper.GetChild"/> method, which also
        /// supports content elements. Keep in mind that for content elements,
        /// this method falls back to the logical tree of the element.
        /// </summary>
        /// <param name="parent">The item to be processed.</param>
        /// <returns>The submitted item's child elements, if available.</returns>
        private static IEnumerable<DependencyObject> GetChildObjects(this DependencyObject parent) {
            if (parent == null) {
                yield break;
            }

            if (parent is ContentElement || parent is FrameworkElement) {
                //// use the logical tree for content / framework elements
                // ReSharper disable once LoopCanBePartlyConvertedToQuery
                foreach (var obj in LogicalTreeHelper.GetChildren(parent)) {
                    if (obj is DependencyObject depObj) {
                        yield return depObj;
                    }
                }
            }
            else {
                //// use the visual tree per default
                var count = VisualTreeHelper.GetChildrenCount(parent);
                for (var i = 0; i < count; i++) {
                    yield return VisualTreeHelper.GetChild(parent, i);
                }
            }
        }

        #endregion
    }
}
