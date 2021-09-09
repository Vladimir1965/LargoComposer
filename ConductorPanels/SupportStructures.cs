// <copyright file="SupportStructures.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Abstract;
using System.Collections.Generic;

namespace ConductorPanels
{
    /// <summary>
    /// Support Structures.
    /// </summary>
    public class SupportStructures
    {
        /// <summary>
        /// Dynamics the elements.
        /// </summary>
        /// <returns> Returns value. </returns>
        public static List<MelodicElement> DynamicElements()
        {
            //// non-equals
            var e0 = new MelodicElement(-1, -1, "Down-Down");
            var e1 = new MelodicElement(-1, 1, "Down-Up");
            var e2 = new MelodicElement(1, -1, "Up-Down");
            var e3 = new MelodicElement(1, 1, "Up-Up");

            var listElements = new List<MelodicElement> {
                e0, e1, e2,  e3
            };

            return listElements;
        }

        /// <summary>
        /// Builds the elements.
        /// </summary>
        /// <param name="section">The section.</param>
        public static void BuildElements(KitSection section)
        {
            List<MelodicElement> listElements = SupportStructures.DynamicElements(); //// SupportStructures.AllKitElements();

            foreach (var zone in section.ListZones) {
                zone.Elements = new List<MelodicElement>();
                for (int i = 0; i < zone.Varia; i++) {
                    var idx = MathSupport.RandomNatural(listElements.Count - 1);
                    var element = listElements[idx];
                    zone.Elements.Add(element);
                }
            }
        }
    }
}