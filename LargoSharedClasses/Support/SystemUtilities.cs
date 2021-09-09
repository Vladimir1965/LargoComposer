// <copyright file="SystemUtilities.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Globalization;
using System.Management;
using JetBrains.Annotations;

namespace LargoSharedClasses.Support
{
    /// <summary>
    /// System Utilities.
    /// </summary>
    public static class SystemUtilities {
        #region Windows System Informations

        /// <summary>
        /// Processors the id.
        /// </summary>
        /// <returns> Returns value. </returns>
        public static string ProcessorId() {
            //// CPU ID:
            var cpuInfo = string.Empty;
            using (var mc = new ManagementClass("win32_processor")) {
                using (var moc = mc.GetInstances()) {
                    // ReSharper disable once LoopCanBePartlyConvertedToQuery
                    foreach (var o in moc) {
                        var mo = (ManagementObject)o;
                        var pid = mo.Properties["processorID"];
                        //// resharper (no null test needed)
                        cpuInfo = pid.Value.ToString();
                        break;
                    }
                }
            }

            return cpuInfo;
        }

        /// <summary>
        /// Volumes the serial number.
        /// </summary>
        /// <returns> Returns value. </returns>
        public static string VolumeSerialNumber() {
            //// HD ID:
            const string drive = "C";
            var volumeSerial = string.Empty;
            using (var dsk = new ManagementObject(
                string.Format(CultureInfo.InvariantCulture, @"win32_logicaldisk.deviceid=""{0}:""", drive))) {
                dsk.Get();
                var vsn = dsk["VolumeSerialNumber"];
                if (vsn != null) {
                    volumeSerial = vsn.ToString();
                }
            }

            return volumeSerial;
        }

        /// <summary>
        /// Combine two serials to get a uniqueId for that machine.
        /// </summary>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public static string MachineUniqueId() {
            var cpuInfo = ProcessorId();
            var volumeSerial = VolumeSerialNumber();
            var uniqueId = string.Format(CultureInfo.InvariantCulture, "{0}#{1}", cpuInfo, volumeSerial);
            return uniqueId;
        }

        #endregion
    }
}