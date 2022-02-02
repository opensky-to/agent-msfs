﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PayloadStations.cs" company="OpenSky">
// OpenSky project 2021-2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenSky.Agent.Simulator.Models
{
    using System.Collections.Generic;

    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    /// Payload stations model.
    /// </summary>
    /// <remarks>
    /// sushi.at, 31/01/2022.
    /// </remarks>
    /// -------------------------------------------------------------------------------------------------
    public class PayloadStations
    {
        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the number of payload stations. 
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public int Count { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The payload station name 1.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public string Name1 { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The payload station name 10.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------

        public string Name10 { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The payload station name 11.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------

        public string Name11 { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The payload station name 12.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------

        public string Name12 { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The payload station name 13.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------

        public string Name13 { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The payload station name 14.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------

        public string Name14 { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The payload station name 15.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------

        public string Name15 { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The payload station name 16.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------

        public string Name16 { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The payload station name 17.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------

        public string Name17 { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The payload station name 18.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------

        public string Name18 { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The payload station name 19.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------

        public string Name19 { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The payload station name 2.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public string Name2 { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The payload station name 20.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------

        public string Name20 { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The payload station name 3.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------

        public string Name3 { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The payload station name 4.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------

        public string Name4 { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The payload station name 5.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------

        public string Name5 { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The payload station name 6.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------

        public string Name6 { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The payload station name 7.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------

        public string Name7 { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The payload station name 8.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------

        public string Name8 { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The payload station name 9.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------

        public string Name9 { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the payload stations names list.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public List<string> Names =>
            new()
            {
                this.Name1?.Replace("TT:MENU.PAYLOAD.", string.Empty),
                this.Name2?.Replace("TT:MENU.PAYLOAD.", string.Empty),
                this.Name3?.Replace("TT:MENU.PAYLOAD.", string.Empty),
                this.Name4?.Replace("TT:MENU.PAYLOAD.", string.Empty),
                this.Name5?.Replace("TT:MENU.PAYLOAD.", string.Empty),
                this.Name6?.Replace("TT:MENU.PAYLOAD.", string.Empty),
                this.Name7?.Replace("TT:MENU.PAYLOAD.", string.Empty),
                this.Name8?.Replace("TT:MENU.PAYLOAD.", string.Empty),
                this.Name9?.Replace("TT:MENU.PAYLOAD.", string.Empty),
                this.Name10?.Replace("TT:MENU.PAYLOAD.", string.Empty),
                this.Name11?.Replace("TT:MENU.PAYLOAD.", string.Empty),
                this.Name12?.Replace("TT:MENU.PAYLOAD.", string.Empty),
                this.Name13?.Replace("TT:MENU.PAYLOAD.", string.Empty),
                this.Name14?.Replace("TT:MENU.PAYLOAD.", string.Empty),
                this.Name15?.Replace("TT:MENU.PAYLOAD.", string.Empty),
                this.Name16?.Replace("TT:MENU.PAYLOAD.", string.Empty),
                this.Name17?.Replace("TT:MENU.PAYLOAD.", string.Empty),
                this.Name18?.Replace("TT:MENU.PAYLOAD.", string.Empty),
                this.Name19?.Replace("TT:MENU.PAYLOAD.", string.Empty),
                this.Name20?.Replace("TT:MENU.PAYLOAD.", string.Empty),
            };

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the total payload weight across all stations.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public double TotalWeight
        {
            get
            {
                var total = 0.0;
                for (var i = 1; i < this.Count + 1; i++)
                {
                    var propInfo = this.GetType().GetProperty($"Weight{i}");
                    if (propInfo != null)
                    {
                        total += (double)propInfo.GetValue(this);
                    }
                }

                return total;
            }
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The payload station weight 1.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public double Weight1 { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The payload station weight 10.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public double Weight10 { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The payload station weight 11.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public double Weight11 { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The payload station weight 12.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public double Weight12 { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The payload station weight 13.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public double Weight13 { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The payload station weight 14.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public double Weight14 { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The payload station weight 15.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public double Weight15 { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The payload station weight 16.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public double Weight16 { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The payload station weight 17.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public double Weight17 { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The payload station weight 18.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public double Weight18 { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The payload station weight 19.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public double Weight19 { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The payload station weight 2.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public double Weight2 { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The payload station weight 20.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public double Weight20 { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The payload station weight 3.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public double Weight3 { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The payload station weight 4.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public double Weight4 { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The payload station weight 5.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public double Weight5 { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The payload station weight 6.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public double Weight6 { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The payload station weight 7.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public double Weight7 { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The payload station weight 8.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public double Weight8 { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The payload station weight 9.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public double Weight9 { get; set; }
    }
}