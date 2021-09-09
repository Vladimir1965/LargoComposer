// <copyright file="AbstractText.cs" company="Largo">
// Copyright (c) 2009 All Right Reserved
// </copyright>
// <author> vl </author>
// <email></email>
// <date>2018-12-01</date>
// <summary>Contains ...</summary>
namespace LargoPanels.Abstract
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Abstract Text.
    /// </summary>
    public class AbstractText
    {
        #region Fields
        /// <summary>
        /// Singleton variable.
        /// </summary>
        private static readonly AbstractText InternalSingleton = new AbstractText();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractText"/> class.
        /// </summary>
        public AbstractText()
        {
            this.CultureInfo = new CultureInfo("en-US");
            this.FontSize = BaseCell.CurrentFontSize;
            this.FontFamily = new FontFamily("Andalus");
            this.FontWeight = FontWeights.Bold;
            this.Typeface = new Typeface(this.FontFamily, FontStyles.Normal, this.FontWeight, FontStretches.Normal);
        }
        #endregion

        #region Static properties

        /// <summary>
        /// Gets the singleton.
        /// </summary>
        /// <value>
        /// Returns value.
        /// </value>
        public static AbstractText Singleton
        {
            get
            {
                Contract.Ensures(Contract.Result<AbstractText>() != null);
                if (InternalSingleton == null) {
                    throw new InvalidOperationException("Singleton Abstract Text is null.");
                }

                return InternalSingleton;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the culture information.
        /// </summary>
        /// <value>
        /// The culture information.
        /// </value>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// Gets or sets the typeface.
        /// </summary>
        /// <value>
        /// The typeface.
        /// </value>
        public Typeface Typeface { get; set; }

        /// <summary>
        /// Gets or sets the background.
        /// </summary>
        /// <value>
        /// The background.
        /// </value>
        public Brush Background { get; set; }

        /// <summary>
        /// Gets or sets the size of the font.
        /// </summary>
        /// <value>
        /// The size of the font.
        /// </value>
        public int FontSize { get; set; }

        /// <summary>
        /// Gets or sets the font weight.
        /// </summary>
        /// <value>
        /// The font weight.
        /// </value>
        public FontWeight FontWeight { get; set; }

        /// <summary>
        /// Gets or sets the font family.
        /// </summary>
        /// <value>
        /// The font family.
        /// </value>
        public FontFamily FontFamily { get; set; }
        #endregion

        /// <summary>
        /// Formats the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="maxTextWidth">Maximum width of the text.</param>
        /// <returns> Returns value. </returns>
        public FormattedText FormatText(string text, int maxTextWidth)
        {
            double pixelsPerDip = 1;
            var formattedText = new FormattedText(
                text,
                this.CultureInfo,
                FlowDirection.LeftToRight,
                this.Typeface,
                this.FontSize,
                Brushes.Black,
                pixelsPerDip)
            { MaxTextWidth = maxTextWidth };

            return formattedText;
        }

        /// <summary>
        /// Loads the image.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns> Returns value. </returns>
        public BitmapImage LoadImage(string name)
        {
            var uri = new Uri("pack://application:,,,/LargoPanels;component/Images/" + name);

            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = uri;
            image.EndInit();

            return image;
        }
    }
}
