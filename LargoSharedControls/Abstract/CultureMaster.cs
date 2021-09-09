// <copyright file="CultureMaster.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedControls.Abstract
{
    using LargoSharedClasses.Localization;
    using LargoSharedClasses.Settings;
    using System.Globalization;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Culture Master.
    /// </summary>
    public static class CultureMaster {
        /// <summary>
        /// Localizes this instance.
        /// </summary>
        /// <param name="master">The master.</param>
        public static void Localize(DependencyObject master) {
            CultureInfo cultureInfo = MusicalSettings.Singleton.CultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            //// this.tbFiles.Text = LocalizedMusic.String("Files");
            //// StringBuilder sb = new StringBuilder(); 

            var element = master as FrameworkElement;
            element?.ApplyTemplate();

            //// PanelAbstract.LocalizeComboBoxItems(master);
            LocalizeExpanders(master);
            LocalizeTabItems(master);
            LocalizeMenuItems(master);
            LocalizeButtons(master);
            LocalizeTextBlocks(master);
            LocalizeLabels(master);
            LocalizeCheckBoxes(master);
            LocalizeRadioButtons(master);
            LocalizeComboBoxes(master);
            LocalizeGroupBoxes(master);

            //// PanelAbstract.RecurseChildren(master);
            //// this.UpdateLayout(); 
            //// string x = sb.ToString();
            //// Console.Write(x);  
        }

        /// <summary>
        /// Localizes the specified given string.
        /// </summary>
        /// <param name="givenString">The given string.</param>
        /// <returns> Returns value. </returns>
        public static string Localize(string givenString) {
            if (string.IsNullOrEmpty(givenString)) {
                return givenString;
            }

            var s = LocalizedControls.String(givenString);
            if (!string.IsNullOrEmpty(s)) {
                return s;
            }

            s = LocalizedMusic.String(givenString);
            if (!string.IsNullOrEmpty(s)) {
                return s;
            }

            return givenString;
        }

        #region Localization

        /// <summary>
        /// Localizes the group boxes.
        /// </summary>
        /// <param name="master">The master.</param>
        private static void LocalizeGroupBoxes(DependencyObject master) {
            //// List<string> list = new List<string>();
            var list = master.FindChildren<GroupBox>();
            foreach (var tb in list) {
                var sc = tb.Header as string; //// tb.Name
                var s = Localize(sc);
                if (!string.IsNullOrEmpty(s)) {
                    tb.Header = s;
                }
            }
        }

        /// <summary>
        /// Localizes the expanders.
        /// </summary>
        /// <param name="master">The master.</param>
        private static void LocalizeExpanders(DependencyObject master) {
            //// List<string> list = new List<string>();
            var list = master.FindChildren<Expander>();
            foreach (var tb in list) {
                var sc = tb.Header as string; //// tb.Name
                var s = Localize(sc);
                if (!string.IsNullOrEmpty(s)) {
                    tb.Header = s;
                }
            }
        }

        /// <summary>
        /// Localizes the buttons.
        /// </summary>
        /// <param name="master">The master.</param>
        private static void LocalizeTabItems(DependencyObject master) {
            //// List<string> list = new List<string>();
            var list = master.FindChildren<TabItem>();
            foreach (var tb in list) {
                var sc = tb.Header as string; //// tb.Name
                var s = Localize(sc);
                if (!string.IsNullOrEmpty(s)) {
                    tb.Header = s;
                }
            }
        }

        /// <summary>
        /// Localizes the buttons.
        /// </summary>
        /// <param name="master">The master.</param>
        private static void LocalizeMenuItems(DependencyObject master) {
            //// List<string> list = new List<string>();
            var list = master.FindChildren<MenuItem>();
            foreach (var tb in list) {
                var sc = tb.Header as string; //// tb.Name
                var s = Localize(sc);
                if (!string.IsNullOrEmpty(s)) {
                    tb.Header = s;
                }
            }
        }

        /// <summary>
        /// Localizes the text blocks.
        /// </summary>
        /// <param name="master">The master.</param>
        private static void LocalizeTextBlocks(DependencyObject master) {
            //// List<string> list = new List<string>();
            //// foreach (TextBlock tb in master.FindChildren<TextBlock>()) {  
            //// foreach (DependencyObject deo in master.GetChildObjects()) {TextBlock tb = deo as TextBlock; if (tb == null) {  continue;  }
            //// foreach (DependencyObject deo in LogicalTreeHelper.GetChildren(master)) {TextBlock tb = deo as TextBlock; if (tb == null) {  continue;  }
            //// var x = master.GetChildObjects();
            //// var c = UserInterfaceHelper.FindVisualChildren<TextBlock>(master);
            var list = master.FindChildren<TextBlock>();
            foreach (var tb in list) {
                var s = Localize(tb.Name);
                if (!string.IsNullOrEmpty(s) && s != tb.Name) {
                    tb.Text = s;
                    continue;
                }
                //// if (!list.Contains(tb.Name)) { list.Add(tb.Name);   Console.WriteLine(tb.Name); }

                var sc = tb.Text;
                s = Localize(sc);
                if (!string.IsNullOrEmpty(s)) {
                    tb.Text = s;
                }
                //// if (!list.Contains(tb.Text)) { list.Add(tb.Text);  Console.WriteLine(tb.Text); }
            }
        }

        /// <summary>
        /// Localizes the buttons.
        /// </summary>
        /// <param name="master">The master.</param>
        private static void LocalizeButtons(DependencyObject master) {
            //// List<string> list = new List<string>();
            var list = master.FindChildren<Button>();
            foreach (var tb in list) {
                var sc = tb.Content as string; //// tb.Name
                var s = Localize(sc);
                if (!string.IsNullOrEmpty(s)) {
                    tb.Content = s;
                }
            }
        }

        /// <summary>
        /// Localizes the labels.
        /// </summary>
        /// <param name="master">The master.</param>
        private static void LocalizeLabels(DependencyObject master) {
            //// List<string> list = new List<string>();
            var list = master.FindChildren<Label>();
            foreach (var tb in list) {
                var sc = tb.Content as string; //// tb.Name
                var s = Localize(sc);
                if (!string.IsNullOrEmpty(s)) {
                    tb.Content = s;
                }
            }
        }

        /// <summary>
        /// Localizes the check boxes.
        /// </summary>
        /// <param name="master">The master.</param>
        private static void LocalizeCheckBoxes(DependencyObject master) {
            //// List<string> list = new List<string>();
            var list = master.FindChildren<CheckBox>();
            foreach (var tb in list) {
                var sc = tb.Content as string; //// tb.Name
                var s = Localize(sc);
                if (!string.IsNullOrEmpty(s)) {
                    tb.Content = s;
                }
            }
        }

        /// <summary>
        /// Localize Radio Buttons.
        /// </summary>
        /// <param name="master">The master.</param>
        private static void LocalizeRadioButtons(DependencyObject master) {
            //// List<string> list = new List<string>();
            var list = master.FindChildren<RadioButton>();
            foreach (var tb in list) {
                var sc = tb.Content as string; //// tb.Name
                var s = Localize(sc);
                if (!string.IsNullOrEmpty(s)) {
                    tb.Content = s;
                }
            }
        }

        /// <summary>
        /// Localizes the check boxes.
        /// </summary>
        /// <param name="master">The master.</param>
        private static void LocalizeComboBoxes(DependencyObject master) {
            //// List<string> list = new List<string>();
            var list = master.FindChildren<ComboBox>();
            foreach (var cb in list) {
                foreach (var obj in cb.Items)
                {
                    var cbi = obj as ComboBoxItem;
                    if (cbi == null)
                    {
                        continue;
                    }

                    var sc = cbi.Content as string; //// tb.Name
                    var s = Localize(sc);
                    if (!string.IsNullOrEmpty(s))
                    {
                        cbi.Content = s;
                    }
                }
            }
        }

        #endregion
    }
}
