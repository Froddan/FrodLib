﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FrodLib.Resources {
    using System;
    using System.Reflection;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class StringResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal StringResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("FrodLib.Resources.StringResources", typeof(StringResources).GetTypeInfo().Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Temperature cannot be below absolute zero.
        /// </summary>
        internal static string BelowAbsoluteZeroErrorMessage {
            get {
                return ResourceManager.GetString("BelowAbsoluteZeroErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Buffer cannot be null..
        /// </summary>
        internal static string BufferCannotBeNull {
            get {
                return ResourceManager.GetString("BufferCannotBeNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Number of bytes to copy cannot be negative..
        /// </summary>
        internal static string BytesCopyCannotBeNegative {
            get {
                return ResourceManager.GetString("BytesCopyCannotBeNegative", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Thread is not allowed to acquire upgradeable read lock while it has a read lock.
        /// </summary>
        internal static string CannotAcquireUpgradeableLockWhileReadLock {
            get {
                return ResourceManager.GetString("CannotAcquireUpgradeableLockWhileReadLock", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Thread is not allowed to acquire writelock while it has a read lock.
        /// </summary>
        internal static string CannotAcquireWriteLockWhileReadLock {
            get {
                return ResourceManager.GetString("CannotAcquireWriteLockWhileReadLock", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Couldn&apos;t parse the time with any of the specified format(s).
        /// </summary>
        internal static string CannotParseTimeWithFormats {
            get {
                return ResourceManager.GetString("CannotParseTimeWithFormats", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot redo an operation while a transaction active.
        /// </summary>
        internal static string CannotRedoWhileTransaction {
            get {
                return ResourceManager.GetString("CannotRedoWhileTransaction", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot undo an operation while a transaction active..
        /// </summary>
        internal static string CannotUndoWhileTransaction {
            get {
                return ResourceManager.GetString("CannotUndoWhileTransaction", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Destination offset cannot be negative..
        /// </summary>
        internal static string DestinationOffsetCannotBeNegative {
            get {
                return ResourceManager.GetString("DestinationOffsetCannotBeNegative", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot divide with Zero.
        /// </summary>
        internal static string DivideByZero {
            get {
                return ResourceManager.GetString("DivideByZero", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Conversion from unit &apos;{0}&apos; to unit &apos;{0}&apos; resulted in division by zero.
        /// </summary>
        internal static string DivisionByZeroExceptionTextFormat {
            get {
                return ResourceManager.GetString("DivisionByZeroExceptionTextFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hour can only be in range [0-23].
        /// </summary>
        internal static string HourOutOfRange {
            get {
                return ResourceManager.GetString("HourOutOfRange", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The implementation type is not an instantiatable class.
        /// </summary>
        internal static string ImplementationTypeIsNotAnInstantiatableClass {
            get {
                return ResourceManager.GetString("ImplementationTypeIsNotAnInstantiatableClass", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Circular reference detected for type: {0}..
        /// </summary>
        internal static string IoCCircularReferenceDetected {
            get {
                return ResourceManager.GetString("IoCCircularReferenceDetected", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Max items cannot be less then zero.
        /// </summary>
        internal static string MaxItemsLessThenZero {
            get {
                return ResourceManager.GetString("MaxItemsLessThenZero", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Millisecond can only be in range [0-999].
        /// </summary>
        internal static string MilliSecondsOutOfRange {
            get {
                return ResourceManager.GetString("MilliSecondsOutOfRange", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Minute can only be in range [0-59].
        /// </summary>
        internal static string MinutesOutOfRange {
            get {
                return ResourceManager.GetString("MinutesOutOfRange", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to More then one default factory exist for Interface: {0}.
        /// </summary>
        internal static string MoreThenOneFactorySpecified {
            get {
                return ResourceManager.GetString("MoreThenOneFactorySpecified", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No default constructor could be found.
        /// </summary>
        internal static string NoDefaultConstructorCouldBeFound {
            get {
                return ResourceManager.GetString("NoDefaultConstructorCouldBeFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No format(s) specified.
        /// </summary>
        internal static string NoFormatsSpecified {
            get {
                return ResourceManager.GetString("NoFormatsSpecified", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No implementation was configured for the interface: {0}..
        /// </summary>
        internal static string NoImplementationConfigured {
            get {
                return ResourceManager.GetString("NoImplementationConfigured", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No redo operations available.
        /// </summary>
        internal static string NoRedoAvailable {
            get {
                return ResourceManager.GetString("NoRedoAvailable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No required primary constructor could be found for type: {0}..
        /// </summary>
        internal static string NoRequiredPrimaryConstructorCouldBeFoundForTypeFormat {
            get {
                return ResourceManager.GetString("NoRequiredPrimaryConstructorCouldBeFoundForTypeFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No undo operations available.
        /// </summary>
        internal static string NoUndoAvailable {
            get {
                return ResourceManager.GetString("NoUndoAvailable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Recursive policy doesn&apos;t allow recursive locks.
        /// </summary>
        internal static string PolicyNotAllowRecursiveLocks {
            get {
                return ResourceManager.GetString("PolicyNotAllowRecursiveLocks", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Property &apos;{0}&apos; doesn&apos;t exist for Type: &apos;{1}&apos;.
        /// </summary>
        internal static string PropertyNotExist {
            get {
                return ResourceManager.GetString("PropertyNotExist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Queue doesn&apos;t contain any items.
        /// </summary>
        internal static string QueueIsEmpty {
            get {
                return ResourceManager.GetString("QueueIsEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Second can only be in range [0-59].
        /// </summary>
        internal static string SecondsOutOfRange {
            get {
                return ResourceManager.GetString("SecondsOutOfRange", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Thread doesn&apos;t hold any read lock.
        /// </summary>
        internal static string ThreadDoesntHoldReadLock {
            get {
                return ResourceManager.GetString("ThreadDoesntHoldReadLock", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Thread doesn&apos;t hold any upgradeable lock.
        /// </summary>
        internal static string ThreadDoesntHoldUpgradeableLock {
            get {
                return ResourceManager.GetString("ThreadDoesntHoldUpgradeableLock", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Thread doesn&apos;t hold any write lock.
        /// </summary>
        internal static string ThreadDoesntHoldWriteLock {
            get {
                return ResourceManager.GetString("ThreadDoesntHoldWriteLock", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There is already an transaction active.
        /// </summary>
        internal static string TransactionActive {
            get {
                return ResourceManager.GetString("TransactionActive", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There is already an transaction {0} active.
        /// </summary>
        internal static string TransactionNameActive {
            get {
                return ResourceManager.GetString("TransactionNameActive", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Type: {0} was not mapped.
        /// </summary>
        internal static string TypeWasNotMapped {
            get {
                return ResourceManager.GetString("TypeWasNotMapped", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to create instance of type.
        /// </summary>
        internal static string UnableToCreateInstanceOfType {
            get {
                return ResourceManager.GetString("UnableToCreateInstanceOfType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unknown temperature unit.
        /// </summary>
        internal static string UnknownTemperatureUnit {
            get {
                return ResourceManager.GetString("UnknownTemperatureUnit", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Worker queue is already running. Please shut it down first before you try to start it.
        /// </summary>
        internal static string WorkerQueueAlreadyRunning {
            get {
                return ResourceManager.GetString("WorkerQueueAlreadyRunning", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The worker queue has been shutdown and is no longer running.
        /// </summary>
        internal static string WorkerQueueShutedDown {
            get {
                return ResourceManager.GetString("WorkerQueueShutedDown", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to xml string can&apos;t be null or empty.
        /// </summary>
        internal static string XMLStringCannotBeNull {
            get {
                return ResourceManager.GetString("XMLStringCannotBeNull", resourceCulture);
            }
        }
    }
}
