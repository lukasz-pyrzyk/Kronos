// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: requestType.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Kronos.Core.Messages {

  /// <summary>Holder for reflection information generated from requestType.proto</summary>
  public static partial class RequestTypeReflection {

    #region Descriptor
    /// <summary>File descriptor for requestType.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static RequestTypeReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChFyZXF1ZXN0VHlwZS5wcm90bypqCgtSZXF1ZXN0VHlwZRILCgdVbmtub3du",
            "EAASCgoGSW5zZXJ0EAESBwoDR2V0EAISCgoGRGVsZXRlEAMSCQoFQ291bnQQ",
            "BBIMCghDb250YWlucxAFEgkKBUNsZWFyEAYSCQoFU3RhdHMQB0IXqgIUS3Jv",
            "bm9zLkNvcmUuTWVzc2FnZXNiBnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::Kronos.Core.Messages.RequestType), }, null));
    }
    #endregion

  }
  #region Enums
  public enum RequestType {
    [pbr::OriginalName("Unknown")] Unknown = 0,
    [pbr::OriginalName("Insert")] Insert = 1,
    [pbr::OriginalName("Get")] Get = 2,
    [pbr::OriginalName("Delete")] Delete = 3,
    [pbr::OriginalName("Count")] Count = 4,
    [pbr::OriginalName("Contains")] Contains = 5,
    [pbr::OriginalName("Clear")] Clear = 6,
    [pbr::OriginalName("Stats")] Stats = 7,
  }

  #endregion

}

#endregion Designer generated code
