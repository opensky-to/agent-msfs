﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WeightAndBalance.cs" company="OpenSky">
// OpenSky project 2021-2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenSky.Agent.Simulator.Models
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    /// Weight and balance model.
    /// </summary>
    /// <remarks>
    /// sushi.at, 31/01/2022.
    /// </remarks>
    /// -------------------------------------------------------------------------------------------------
    public class WeightAndBalance
    {
        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The center of gravity aft limit.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public double CgAftLimit { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The center of gravity forward limit.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public double CgFwdLimit { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The center of gravity (longitudinal)
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public double CgPercent { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The center of gravity (lateral)
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public double CgPercentLateral { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Empty weight in pounds.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public double EmptyWeight { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Fuel total capacity in gallons.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public double FuelTotalCapacity { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Fuel total quantity on board in gallons.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public double FuelTotalQuantity { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The fuel weight in pounds.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public double FuelWeight => this.FuelTotalQuantity * this.FuelWeightPerGallon;

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Fuel weight (pounds/gallon).
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public double FuelWeightPerGallon { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The maximum fuel weight in pounds.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public double MaxFuelWeight => this.FuelTotalCapacity * this.FuelWeightPerGallon;

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Max gross weight in pounds.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public double MaxGrossWeight { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The maximum payload weight in pounds.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public double MaxPayloadWeight => this.MaxGrossWeight - this.EmptyWeight - this.FuelWeight;

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the payload percentage out of maximum allowed.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public double PayloadPercentOfMax => this.MaxPayloadWeight != 0 ? this.PayloadWeight / this.MaxPayloadWeight : 0;

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The payload weight in pounds.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public double PayloadWeight => this.TotalWeight - this.EmptyWeight - this.FuelWeight;

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Total weight in pounds.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public double TotalWeight { get; set; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the total weight percentage of the maximum allowed.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public double TotalWeightPercentOfMax => this.MaxGrossWeight != 0 ? this.TotalWeight / this.MaxGrossWeight : 0;
    }
}