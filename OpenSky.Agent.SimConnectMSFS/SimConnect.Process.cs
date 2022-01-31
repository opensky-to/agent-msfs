﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimConnect.Process.cs" company="OpenSky">
// OpenSky project 2021-2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenSky.AgentMSFS.SimConnect
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Device.Location;
    using System.Diagnostics;
    using System.Media;
    using System.Reflection;
    using System.Threading;
    using System.Windows;
    using System.Windows.Media;

    using JetBrains.Annotations;

    using Microsoft.Maps.MapControl.WPF;

    using OpenSky.Agent.SimConnectMSFS.Helpers;
    using OpenSky.Agent.SimConnectMSFS.Structs;
    using OpenSky.Agent.Simulator;
    using OpenSky.Agent.Simulator.Enums;
    using OpenSky.AgentMSFS.Models;
    using OpenSky.AgentMSFS.SimConnect.Enums;
    using OpenSky.AgentMSFS.SimConnect.Helpers;
    using OpenSky.AgentMSFS.SimConnect.Structs;
    using OpenSky.FlightLogXML;

    using TrackingEventLogEntry = OpenSky.Agent.Simulator.Models.TrackingEventLogEntry;
    using TrackingEventMarker = Models.TrackingEventMarker;

    /// -------------------------------------------------------------------------------------------------
    /// <summary>
    /// Simconnect client - data processing code.
    /// </summary>
    /// <remarks>
    /// sushi.at, 13/03/2021.
    /// </remarks>
    /// -------------------------------------------------------------------------------------------------
    public partial class SimConnect
    {
        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The landing analysis processing queue.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        [NotNull]
        private readonly ConcurrentQueue<ProcessLandingAnalysis> landingAnalysisProcessingQueue;

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The primary tracking processing queue.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        [NotNull]
        private readonly ConcurrentQueue<ProcessPrimaryTracking> primaryTrackingProcessingQueue;

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The secondary tracking processing queue.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        [NotNull]
        private readonly ConcurrentQueue<ProcessSecondaryTracking> secondaryTrackingProcessingQueue;

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// The tracking event markers.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        private readonly List<TrackingEventMarker> trackingEventMarkers = new();

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Occurs when the plane's location changed.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public event EventHandler<Location> LocationChanged;

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Occurs when SimConnect adds a new tracking event marker.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public event EventHandler<TrackingEventMarker> TrackingEventMarkerAdded;

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the length of the landing analysis processing queue.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public int LandingAnalysisProcessingQueueLength => this.landingAnalysisProcessingQueue.Count;

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the length of the primary tracking processing queue.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public int PrimaryTrackingProcessingQueueLength => this.primaryTrackingProcessingQueue.Count;

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the length of the secondary tracking processing queue.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public int SecondaryTrackingProcessingQueueLength => this.secondaryTrackingProcessingQueue.Count;

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the tracking event log entries.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public ObservableCollection<TrackingEventLogEntry> TrackingEventLogEntries { get; }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Add a tracking event to the map and log.
        /// </summary>
        /// <remarks>
        /// sushi.at, 16/03/2021.
        /// </remarks>
        /// <param name="primary">
        /// The primary Simconnect tracking data.
        /// </param>
        /// <param name="secondary">
        /// The secondary Simconnect tracking data.
        /// </param>
        /// <param name="type">
        /// The flight tracking event type.
        /// </param>
        /// <param name="color">
        /// The color to use for the marker.
        /// </param>
        /// <param name="text">
        /// The event text (what happened?).
        /// </param>
        /// -------------------------------------------------------------------------------------------------
        private void AddTrackingEvent(PrimaryTracking primary, SecondaryTracking secondary, FlightTrackingEventType type, Color color, string text)
        {
            if (this.TrackingStatus != TrackingStatus.Tracking && this.TrackingStatus != TrackingStatus.GroundOperations)
            {
                return;
            }

            UpdateGUIDelegate addTrackingEvent = () =>
            {
                Debug.WriteLine($"Adding tracking event: {text}");
                if (this.lastNonPositionReportMarker == null || this.lastNonPositionReportMarker.GetDistanceTo(primary) >= 20)
                {
                    lock (this.trackingEventMarkers)
                    {
                        var newMarker = new TrackingEventMarker(primary, secondary, this.WeightAndBalanceStruct.FuelTotalQuantity, 16, color, text);
                        this.lastNonPositionReportMarker = newMarker;
                        this.trackingEventMarkers.Add(newMarker);
                        this.TrackingEventMarkerAdded?.Invoke(this, newMarker);
                    }
                }
                else
                {
                    this.lastNonPositionReportMarker.AddEventToMarker(DateTime.UtcNow, text);
                }

                this.TrackingEventLogEntries.Add(new TrackingEventLogEntry(type, DateTime.UtcNow, color, text, primary.MapLocation));
            };
            Application.Current.Dispatcher.BeginInvoke(addTrackingEvent);
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Monitor tracking start conditions.
        /// </summary>
        /// <remarks>
        /// sushi.at, 28/03/2021.
        /// </remarks>
        /// <param name="pst">
        /// The secondary Simconnect tracking data (old and new).
        /// </param>
        /// -------------------------------------------------------------------------------------------------
        private void MonitorTrackingStartConditions(ProcessSecondaryTracking pst)
        {
            if (this.Flight != null && this.TrackingStatus == TrackingStatus.Preparing || this.TrackingStatus == TrackingStatus.Resuming)
            {
                this.TrackingConditions[(int)Agent.Simulator.Models.TrackingConditions.DateTime].Expected = $"{DateTime.UtcNow.AddHours(this.Flight?.UtcOffset ?? 0):HH:mm dd.MM.yyyy}";
                this.TrackingConditions[(int)Agent.Simulator.Models.TrackingConditions.DateTime].Current = $"{pst.New.UtcDateTime:HH:mm dd.MM.yyyy}";
                this.TrackingConditions[(int)Agent.Simulator.Models.TrackingConditions.PlaneModel].Current = this.AircraftIdentityStruct.Type;

                this.TrackingConditions[(int)Agent.Simulator.Models.TrackingConditions.DateTime].ConditionMet =
                    this.TrackingConditions[(int)Agent.Simulator.Models.TrackingConditions.DateTime].AutoSet || Math.Abs((DateTime.UtcNow.AddHours(this.Flight?.UtcOffset ?? 0) - pst.New.UtcDateTime).TotalMinutes) < 1;
                this.TrackingConditions[(int)Agent.Simulator.Models.TrackingConditions.PlaneModel].ConditionMet = this.Flight?.Aircraft.Type.MatchesAircraftInSimulator() ?? false;

                if (this.TrackingStatus == TrackingStatus.Preparing)
                {
                    this.TrackingConditions[(int)Agent.Simulator.Models.TrackingConditions.Fuel].Enabled = true;
                    this.TrackingConditions[(int)Agent.Simulator.Models.TrackingConditions.Payload].Enabled = true;

                    this.TrackingConditions[(int)Agent.Simulator.Models.TrackingConditions.Fuel].Current = $"{this.WeightAndBalanceStruct.FuelTotalQuantity:F1} gal, {this.WeightAndBalanceStruct.FuelTotalQuantity * 3.78541:F1} liters ▶ {this.WeightAndBalanceStruct.FuelTotalQuantity * this.Flight?.Aircraft.Type.FuelWeightPerGallon:F1} lbs, {this.WeightAndBalanceStruct.FuelTotalQuantity * this.Flight?.Aircraft.Type.FuelWeightPerGallon * 0.453592:F1} kg";
                    this.TrackingConditions[(int)Agent.Simulator.Models.TrackingConditions.Payload].Current = $"{this.PayloadStationsStruct.TotalWeight:F1} lbs, {this.PayloadStationsStruct.TotalWeight * 0.453592:F1} kg";

                    this.TrackingConditions[(int)Agent.Simulator.Models.TrackingConditions.Fuel].ConditionMet =
                        this.TrackingConditions[(int)Agent.Simulator.Models.TrackingConditions.Fuel].AutoSet || Math.Abs(this.WeightAndBalanceStruct.FuelTotalQuantity - this.Flight?.FuelGallons ?? 0) < 0.27; // Allow roughly 1 liter of margin

                    this.TrackingConditions[(int)Agent.Simulator.Models.TrackingConditions.Payload].ConditionMet =
                        this.TrackingConditions[(int)Agent.Simulator.Models.TrackingConditions.Payload].AutoSet || Math.Abs(this.PayloadStationsStruct.TotalWeight - this.Flight?.PayloadPounds ?? 0) < 2.2; // Allow 1 kg of margin

                    this.TrackingConditions[(int)Agent.Simulator.Models.TrackingConditions.RealismSettings].Expected = "No slew, No unlimited fuel,\r\nCrash detection, SimRate=1";
                    this.TrackingConditions[(int)Agent.Simulator.Models.TrackingConditions.RealismSettings].ConditionMet = !this.PrimaryTrackingStruct.SlewActive && !pst.New.UnlimitedFuel && pst.New.CrashDetection && Math.Abs(this.PrimaryTrackingStruct.SimulationRate - 1) == 0;

                    this.TrackingConditions[(int)Agent.Simulator.Models.TrackingConditions.Location].Current =
                        $"{this.PrimaryTrackingStruct.GeoCoordinate.GetDistanceTo(new GeoCoordinate(this.Flight?.Origin.Latitude ?? 0, this.Flight?.Origin.Longitude ?? 0)) / 1000:F2} km from starting location - {(this.PrimaryTrackingStruct.OnGround ? "On ground" : "Airborne")}";
                    this.TrackingConditions[(int)Agent.Simulator.Models.TrackingConditions.Location].ConditionMet = this.PrimaryTrackingStruct.GeoCoordinate.GetDistanceTo(new GeoCoordinate(this.Flight?.Origin.Latitude ?? 0, this.Flight?.Origin.Longitude ?? 0)) < 5000;
                }

                if (this.TrackingStatus == TrackingStatus.Resuming)
                {
                    if (this.Flight?.Aircraft.Type.RequiresManualFuelling != true)
                    {
                        this.TrackingConditions[(int)Agent.Simulator.Models.TrackingConditions.Fuel].Enabled = false;
                    }
                    else
                    {
                        this.TrackingConditions[(int)Agent.Simulator.Models.TrackingConditions.Fuel].Enabled = true;
                        this.TrackingConditions[(int)Agent.Simulator.Models.TrackingConditions.Fuel].Current = $"{this.WeightAndBalanceStruct.FuelTotalQuantity:F1} gal, {this.WeightAndBalanceStruct.FuelTotalQuantity * 3.78541:F1} liters ▶ {this.WeightAndBalanceStruct.FuelTotalQuantity * this.Flight?.Aircraft.Type.FuelWeightPerGallon:F1} lbs, {this.WeightAndBalanceStruct.FuelTotalQuantity * this.Flight?.Aircraft.Type.FuelWeightPerGallon * 0.453592:F1} kg";
                        this.TrackingConditions[(int)Agent.Simulator.Models.TrackingConditions.Fuel].ConditionMet =
                            this.TrackingConditions[(int)Agent.Simulator.Models.TrackingConditions.Fuel].AutoSet || Math.Abs(this.WeightAndBalanceStruct.FuelTotalQuantity - this.flightLoadingTempStructs.FuelTanks.TotalQuantity) < 0.81; // Allow roughly 3 liters of margin, as it can be very hard to get this right otherwise
                    }

                    if (this.Flight?.Aircraft.Type.RequiresManualLoading != true)
                    {
                        this.TrackingConditions[(int)Agent.Simulator.Models.TrackingConditions.Payload].Enabled = false;
                    }
                    else
                    {
                        this.TrackingConditions[(int)Agent.Simulator.Models.TrackingConditions.Payload].Enabled = true;
                        this.TrackingConditions[(int)Agent.Simulator.Models.TrackingConditions.Payload].Current = $"{this.PayloadStationsStruct.TotalWeight:F1} lbs, {this.PayloadStationsStruct.TotalWeight * 0.453592:F1} kg";
                        this.TrackingConditions[(int)Agent.Simulator.Models.TrackingConditions.Payload].ConditionMet =
                            this.TrackingConditions[(int)Agent.Simulator.Models.TrackingConditions.Payload].AutoSet || Math.Abs((double)(this.PayloadStationsStruct.TotalWeight - this.Flight?.PayloadPounds)) < 2.2; // Allow 1 kg of margin
                    }

                    var currentLocation = $"{this.PrimaryTrackingStruct.GeoCoordinate.GetDistanceTo(this.flightLoadingTempStructs?.SlewPlaneIntoPosition.GeoCoordinate ?? new GeoCoordinate(0, 0, 0)) / 1000:F2} km from resume location";
                    currentLocation += $"\r\nLatitude: {this.flightLoadingTempStructs?.SlewPlaneIntoPosition.Latitude:F4}";
                    currentLocation += $"\r\nLongitude: {this.flightLoadingTempStructs?.SlewPlaneIntoPosition.Longitude:F4}";
                    currentLocation += $"\r\nAltitude (AGL): {this.flightLoadingTempStructs?.SlewPlaneIntoPosition.RadioHeight:F0}";

                    this.TrackingConditions[(int)Agent.Simulator.Models.TrackingConditions.RealismSettings].Expected = "No unlimited fuel,\r\nCrash detection, SimRate=1";
                    this.TrackingConditions[(int)Agent.Simulator.Models.TrackingConditions.RealismSettings].ConditionMet = !pst.New.UnlimitedFuel && pst.New.CrashDetection && Math.Abs(this.PrimaryTrackingStruct.SimulationRate - 1) == 0;

                    this.TrackingConditions[(int)Agent.Simulator.Models.TrackingConditions.Location].Current = currentLocation;
                    this.TrackingConditions[(int)Agent.Simulator.Models.TrackingConditions.Location].ConditionMet =
                        (this.PrimaryTrackingStruct.GeoCoordinate.GetDistanceTo(this.flightLoadingTempStructs?.SlewPlaneIntoPosition.GeoCoordinate ?? new GeoCoordinate(0, 0, 0)) < 100) &&
                        Math.Abs(this.PrimaryTrackingStruct.RadioHeight - this.flightLoadingTempStructs?.SlewPlaneIntoPosition.RadioHeight ?? -1000) < 50;
                }
            }
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Process the landing analysis data (old vs new)
        /// </summary>
        /// <remarks>
        /// sushi.at, 25/03/2021.
        /// </remarks>
        /// -------------------------------------------------------------------------------------------------
        private void ProcessLandingAnalysis()
        {
            while (!this.close)
            {
                while (this.landingAnalysisProcessingQueue.TryDequeue(out var pla))
                {
                    try
                    {
                        this.CheckForAndAnalyseLanding(pla);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Error processing landing analysis: " + ex);
                    }
                }

                Thread.Sleep(500);
            }
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Process the payload stations.
        /// </summary>
        /// <remarks>
        /// sushi.at, 26/11/2021.
        /// </remarks>
        /// <param name="oldPayload">
        /// The old payload stations.
        /// </param>
        /// <param name="newPayload">
        /// The new payload stations.
        /// </param>
        /// -------------------------------------------------------------------------------------------------
        private void ProcessPayloadStations(PayloadStations oldPayload, PayloadStations newPayload)
        {
            try
            {
                if (this.Flight != null && (this.TrackingStatus is TrackingStatus.GroundOperations or TrackingStatus.Tracking))
                {
                    if (Math.Abs(oldPayload.TotalWeight - newPayload.TotalWeight) > 0.1)
                    {
                        if (Math.Abs(newPayload.TotalWeight - this.Flight.PayloadPounds) > this.Flight.Aircraft.Type.MaxPayloadDeltaAllowed)
                        {
                            Debug.WriteLine("OpenSky Warning: Tracking aborted, payload changed below required load.");
                            var assembly = Assembly.GetExecutingAssembly();
                            var player = new SoundPlayer(assembly.GetManifestResourceStream("OpenSky.AgentMSFS.Resources.OSnegative.wav"));
                            player.PlaySync();
                            SpeechSoundPacks.Instance.PlaySpeechEvent(SpeechEvent.AbortedPayloadChange);
                            this.StopTracking(false);
                            this.fsConnect.SetText("OpenSky Warning: Tracking aborted, payload changed.", 5);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error processing payload stations: " + ex);
            }
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Process the plane identity.
        /// </summary>
        /// <remarks>
        /// sushi.at, 28/03/2021.
        /// </remarks>
        /// -------------------------------------------------------------------------------------------------
        private void ProcessPlaneIdentity()
        {
            try
            {
                // Make sure the player didn't use the dev mode to switch the plane
                if (this.Flight != null && (this.TrackingStatus is TrackingStatus.GroundOperations or TrackingStatus.Tracking) && !this.Flight.Aircraft.Type.MatchesAircraftInSimulator())
                {
                    Debug.WriteLine("OpenSky Warning: Tracking aborted, aircraft type was changed.");
                    var assembly = Assembly.GetExecutingAssembly();
                    var player = new SoundPlayer(assembly.GetManifestResourceStream("OpenSky.AgentMSFS.Resources.OSnegative.wav"));
                    player.PlaySync();
                    SpeechSoundPacks.Instance.PlaySpeechEvent(SpeechEvent.AbortedAircraftType);
                    this.StopTracking(false);
                    this.fsConnect.SetText("OpenSky Warning: Tracking aborted, aircraft type was changed.", 5);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error processing plane identity: " + ex);
            }
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Process the primary tracking data (old vs new).
        /// </summary>
        /// <remarks>
        /// sushi.at, 15/03/2021.
        /// </remarks>
        /// -------------------------------------------------------------------------------------------------
        private void ProcessPrimaryTracking()
        {
            while (!this.close)
            {
                Location newLocation = null;
                while (this.primaryTrackingProcessingQueue.TryDequeue(out var ppt))
                {
                    try
                    {
                        this.CheckChangesOverTime(ppt.New);
                        this.MonitorPrimarySystems(ppt);
                        this.AddPositionReport(ppt.New);
                        this.TrackFlight(ppt);

                        // Fire the location changed event?
                        if (!ppt.Old.MapLocation.Equals(ppt.New.MapLocation))
                        {
                            newLocation = ppt.New.MapLocation;
                        }

                        // Are we close to landing?
                        this.SampleRates[Requests.LandingAnalysis] = this.WasAirborne && ppt.New.RadioHeight < 500 ? 25 : 500;
                        this.OnPropertyChanged(nameof(this.SampleRates));
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Error processing primary tracking: " + ex);
                    }
                }

                if (newLocation != null)
                {
                    this.LocationChanged?.Invoke(this, newLocation);
                }

                Thread.Sleep(500);
            }
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Process the secondary tracking data (old vs new).
        /// </summary>
        /// <remarks>
        /// sushi.at, 15/03/2021.
        /// </remarks>
        /// -------------------------------------------------------------------------------------------------
        private void ProcessSecondaryTracking()
        {
            while (!this.close)
            {
                while (this.secondaryTrackingProcessingQueue.TryDequeue(out var pst))
                {
                    try
                    {
                        this.MonitorLights(pst);
                        this.TransitionFlightPhase();
                        this.MonitorSecondarySystems(pst);
                        this.MonitorTrackingStartConditions(pst);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Error processing secondary tracking: " + ex);
                    }
                }

                Thread.Sleep(500);
            }
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Process the weight and balance.
        /// </summary>
        /// <remarks>
        /// sushi.at, 28/03/2021.
        /// </remarks>
        /// <param name="oldWB">
        /// The old weight and balance.
        /// </param>
        /// <param name="newWB">
        /// The new weight and balance.
        /// </param>
        /// -------------------------------------------------------------------------------------------------
        private void ProcessWeightAndBalance(WeightAndBalance oldWB, WeightAndBalance newWB)
        {
            try
            {
                if (this.Flight != null && (this.TrackingStatus is TrackingStatus.GroundOperations or TrackingStatus.Tracking))
                {
                    // Did the fuel go up?
                    if (newWB.FuelTotalQuantity > oldWB.FuelTotalQuantity)
                    {
                        if (newWB.FuelTotalQuantity - oldWB.FuelTotalQuantity > 0.5)
                        {
                            Debug.WriteLine("OpenSky Warning: Tracking aborted, fuel increased.");
                            var assembly = Assembly.GetExecutingAssembly();
                            var player = new SoundPlayer(assembly.GetManifestResourceStream("OpenSky.AgentMSFS.Resources.OSnegative.wav"));
                            player.PlaySync();
                            SpeechSoundPacks.Instance.PlaySpeechEvent(SpeechEvent.AbortedFuelIncreased);
                            this.StopTracking(false);
                            this.fsConnect.SetText("OpenSky Warning: Tracking aborted, fuel increased.", 5);
                        }
                        else
                        {
                            Debug.WriteLine($"Small fuel jump detected: {newWB.FuelTotalQuantity - oldWB.FuelTotalQuantity} gallons");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error processing weight and balance: " + ex);
            }
        }
    }
}