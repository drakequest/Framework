﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DrakeQuest.Data.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("DrakeQuest.Data.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to The command must be set before call or use other method.
        /// </summary>
        internal static string ERR_COMMAND_NULL {
            get {
                return ResourceManager.GetString("ERR_COMMAND_NULL", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DatabaseFactoryConfiguration section not defined in configuration file.
        /// </summary>
        internal static string ERR_CONFIG_NOT_DEFINED {
            get {
                return ResourceManager.GetString("ERR_CONFIG_NOT_DEFINED", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An error occur during pre-execute of {0}, see inner exception for more information.
        /// </summary>
        internal static string ERR_CONNECTION {
            get {
                return ResourceManager.GetString("ERR_CONNECTION", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Can&apos;t open connection see inner exception for more information.
        /// </summary>
        internal static string ERR_CONNECTION_CANTOPEN {
            get {
                return ResourceManager.GetString("ERR_CONNECTION_CANTOPEN", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Database name not defined in DatabaseFactoryConfiguration section of configuration file.
        /// </summary>
        internal static string ERR_CONNECTION_NOT_DEFINED {
            get {
                return ResourceManager.GetString("ERR_CONNECTION_NOT_DEFINED", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Can&apos;t create factory provider name is empty.
        /// </summary>
        internal static string ERR_CONNECTION_NOT_PROVIDERNAME_CONFIGURED {
            get {
                return ResourceManager.GetString("ERR_CONNECTION_NOT_PROVIDERNAME_CONFIGURED", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The connection must be set before or use other method.
        /// </summary>
        internal static string ERR_CONNECTION_NULL {
            get {
                return ResourceManager.GetString("ERR_CONNECTION_NULL", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Connection string {0}  was not found in web.config. {1}.
        /// </summary>
        internal static string ERR_CONNECTION_STR_INVALID {
            get {
                return ResourceManager.GetString("ERR_CONNECTION_STR_INVALID", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The reader is null, this parameter is mandatory .
        /// </summary>
        internal static string ERR_DBREADER_ISNULL {
            get {
                return ResourceManager.GetString("ERR_DBREADER_ISNULL", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An error occur when try to execute {0}, see inner exception for more information.
        /// </summary>
        internal static string ERR_EXECUTING_EXECUTE {
            get {
                return ResourceManager.GetString("ERR_EXECUTING_EXECUTE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error instantiating database {0}. {1}+ sectionHandler.Name.
        /// </summary>
        internal static string ERR_FACTORY_INIT {
            get {
                return ResourceManager.GetString("ERR_FACTORY_INIT", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This factory type was not implemented.
        /// </summary>
        internal static string ERR_FACTORY_NOT_IMPLEMENTED {
            get {
                return ResourceManager.GetString("ERR_FACTORY_NOT_IMPLEMENTED", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The provider ({1}) defined in the connection {0} is not well configured or missing.
        /// </summary>
        internal static string ERR_FACTORYDB_NAME_INVALID {
            get {
                return ResourceManager.GetString("ERR_FACTORYDB_NAME_INVALID", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The provider name for the connection {0} is blank or null..
        /// </summary>
        internal static string ERR_FACTORYDB_NAME_MISSING {
            get {
                return ResourceManager.GetString("ERR_FACTORYDB_NAME_MISSING", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The provider ({1}) defined in the connection {0} is not implemented as ICustomDbDataProvider, type = {2}.
        /// </summary>
        internal static string ERR_FACTORYDB_NOT_IMPLEMENTED {
            get {
                return ResourceManager.GetString("ERR_FACTORYDB_NOT_IMPLEMENTED", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No log system has been configured.
        /// </summary>
        internal static string ERR_LOGGER_NOT_CONFIGURED {
            get {
                return ResourceManager.GetString("ERR_LOGGER_NOT_CONFIGURED", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Can&apos;t generate mapper Object, see inner exception : {0}.
        /// </summary>
        internal static string ERR_MAPPING_GET {
            get {
                return ResourceManager.GetString("ERR_MAPPING_GET", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Field {0} can&apos;t be set with the value {1}, see inner exception :  {2}.
        /// </summary>
        internal static string ERR_MAPPING_PROP {
            get {
                return ResourceManager.GetString("ERR_MAPPING_PROP", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to send file (Code - Error) = {0} - {1}, or as timeout : {2}.
        /// </summary>
        internal static string ERR_NET_FTP_SENDFAIL {
            get {
                return ResourceManager.GetString("ERR_NET_FTP_SENDFAIL", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The fill Action must be set before  call or use other method.
        /// </summary>
        internal static string ERR_READER_FILLACTION {
            get {
                return ResourceManager.GetString("ERR_READER_FILLACTION", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The statment can&apos;t be null or empty.
        /// </summary>
        internal static string ERR_STATMENT_MISSING {
            get {
                return ResourceManager.GetString("ERR_STATMENT_MISSING", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The table name is mandatory.
        /// </summary>
        internal static string ERR_TABLENAME_ISNULL {
            get {
                return ResourceManager.GetString("ERR_TABLENAME_ISNULL", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An address must be provided in To or CC or Bcc for sending message.
        /// </summary>
        internal static string ERROR_MAIL_ADDRESSE_EMPTY {
            get {
                return ResourceManager.GetString("ERROR_MAIL_ADDRESSE_EMPTY", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Can&apos;t send message, error {0}, see exception below.
        /// </summary>
        internal static string ERROR_MAIL_SEND_FAILURE {
            get {
                return ResourceManager.GetString("ERROR_MAIL_SEND_FAILURE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Execute of non return query - start for the command {0}.
        /// </summary>
        internal static string INFO_EXECUTE_NONQuery {
            get {
                return ResourceManager.GetString("INFO_EXECUTE_NONQuery", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Execute of non return query - end for the command {0}.
        /// </summary>
        internal static string INFO_EXECUTE_NonQueryEnd {
            get {
                return ResourceManager.GetString("INFO_EXECUTE_NonQueryEnd", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Execute of reader - start for the command {0}.
        /// </summary>
        internal static string INFO_EXECUTE_READER {
            get {
                return ResourceManager.GetString("INFO_EXECUTE_READER", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Execute of reader - end for the command {0}.
        /// </summary>
        internal static string INFO_EXECUTE_READER_END {
            get {
                return ResourceManager.GetString("INFO_EXECUTE_READER_END", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Executing plain sql as command.
        /// </summary>
        internal static string INFO_EXECUTE_RUNNING_PLAINSQL {
            get {
                return ResourceManager.GetString("INFO_EXECUTE_RUNNING_PLAINSQL", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Execute of scalar- start for the command {0}.
        /// </summary>
        internal static string INFO_EXECUTE_SCALAR {
            get {
                return ResourceManager.GetString("INFO_EXECUTE_SCALAR", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Execute of scalar - end for the command {0}.
        /// </summary>
        internal static string INFO_EXECUTE_SCALAR_END {
            get {
                return ResourceManager.GetString("INFO_EXECUTE_SCALAR_END", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Execute of Fill Action - start for the command {0}.
        /// </summary>
        internal static string INFO_FILL {
            get {
                return ResourceManager.GetString("INFO_FILL", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Execute of Fill Action - end for the command {0}.
        /// </summary>
        internal static string INFO_FILL_END {
            get {
                return ResourceManager.GetString("INFO_FILL_END", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sending file via sftp , sftp args : {0}.
        /// </summary>
        internal static string INFO_NET_SFTP_SENDING {
            get {
                return ResourceManager.GetString("INFO_NET_SFTP_SENDING", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Execute of bulk insert - start for the table {0}.
        /// </summary>
        internal static string VERBOSE_BULK_INSERT {
            get {
                return ResourceManager.GetString("VERBOSE_BULK_INSERT", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Execute of bulk insert - end for the table {0}.
        /// </summary>
        internal static string VERBOSE_BULK_INSERT_END {
            get {
                return ResourceManager.GetString("VERBOSE_BULK_INSERT_END", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This file &lt;{0}&gt; can&apos;t be attached to the message, error {1}, see exception below.
        /// </summary>
        internal static string WARN_MAIL_ATTACHMENT_EXCEPTION {
            get {
                return ResourceManager.GetString("WARN_MAIL_ATTACHMENT_EXCEPTION", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This file &lt;{0}&gt; not exists or path is incorect.
        /// </summary>
        internal static string WARN_MAIL_FILE_NOT_EXIST {
            get {
                return ResourceManager.GetString("WARN_MAIL_FILE_NOT_EXIST", resourceCulture);
            }
        }
    }
}