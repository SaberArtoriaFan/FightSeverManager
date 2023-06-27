// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: FightProto.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Proto {

  /// <summary>Holder for reflection information generated from FightProto.proto</summary>
  public static partial class FightProtoReflection {

    #region Descriptor
    /// <summary>File descriptor for FightProto.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static FightProtoReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChBGaWdodFByb3RvLnByb3RvEgVwcm90byI5CglOR3JpZFBhY2sSDgoGaGVy",
            "b0lEGAEgASgFEg8KB2xldmVsSUQYAiABKAUSCwoDcG9zGAMgASgFIikKDUlQ",
            "QW5kUG9ydFBhY2sSCgoCaXAYASABKAkSDAoEcG9ydBgCIAEoBSIwCg5IZXJv",
            "QW5kUG9zUGFjaxIeCgRsaXN0GAEgAygLMhAucHJvdG8uTkdyaWRQYWNrItAB",
            "CghNYWluUGFjaxIlCgpyZXR1cm5Db2RlGAEgASgOMhEucHJvdG8uUmV0dXJu",
            "Q29kZRIlCgphY3Rpb25Db2RlGAIgASgOMhEucHJvdG8uQWN0aW9uQ29kZRIr",
            "Cg1pcEFuZFBvcnRQYWNrGAMgASgLMhQucHJvdG8uSVBBbmRQb3J0UGFjaxIM",
            "CgR3b3JkGAQgASgJEgwKBGluZm8YBSADKAkSLQoOaGVyb0FuZFBvc0xpc3QY",
            "BiADKAsyFS5wcm90by5IZXJvQW5kUG9zUGFjayotCgpSZXR1cm5Db2RlEgsK",
            "B1N1Y2Nlc3MQABIICgRGYWlsEAESCAoEWmVybxACKmEKCkFjdGlvbkNvZGUS",
            "CQoFTG9naW4QABIUChBSZWFkeUZpZ2h0QWN0aW9uEAESDgoKQnJlYWtGaWdo",
            "dBACEhMKD1VwZGF0ZVJlc291cmNlcxADEg0KCUhlYXJ0QmVhdBAEYgZwcm90",
            "bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::Proto.ReturnCode), typeof(global::Proto.ActionCode), }, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Proto.NGridPack), global::Proto.NGridPack.Parser, new[]{ "HeroID", "LevelID", "Pos" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Proto.IPAndPortPack), global::Proto.IPAndPortPack.Parser, new[]{ "Ip", "Port" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Proto.HeroAndPosPack), global::Proto.HeroAndPosPack.Parser, new[]{ "List" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Proto.MainPack), global::Proto.MainPack.Parser, new[]{ "ReturnCode", "ActionCode", "IpAndPortPack", "Word", "Info", "HeroAndPosList" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Enums
  public enum ReturnCode {
    [pbr::OriginalName("Success")] Success = 0,
    [pbr::OriginalName("Fail")] Fail = 1,
    [pbr::OriginalName("Zero")] Zero = 2,
  }

  public enum ActionCode {
    [pbr::OriginalName("Login")] Login = 0,
    [pbr::OriginalName("ReadyFightAction")] ReadyFightAction = 1,
    [pbr::OriginalName("BreakFight")] BreakFight = 2,
    [pbr::OriginalName("UpdateResources")] UpdateResources = 3,
    [pbr::OriginalName("HeartBeat")] HeartBeat = 4,
  }

  #endregion

  #region Messages
  public sealed partial class NGridPack : pb::IMessage<NGridPack> {
    private static readonly pb::MessageParser<NGridPack> _parser = new pb::MessageParser<NGridPack>(() => new NGridPack());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<NGridPack> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Proto.FightProtoReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public NGridPack() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public NGridPack(NGridPack other) : this() {
      heroID_ = other.heroID_;
      levelID_ = other.levelID_;
      pos_ = other.pos_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public NGridPack Clone() {
      return new NGridPack(this);
    }

    /// <summary>Field number for the "heroID" field.</summary>
    public const int HeroIDFieldNumber = 1;
    private int heroID_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int HeroID {
      get { return heroID_; }
      set {
        heroID_ = value;
      }
    }

    /// <summary>Field number for the "levelID" field.</summary>
    public const int LevelIDFieldNumber = 2;
    private int levelID_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int LevelID {
      get { return levelID_; }
      set {
        levelID_ = value;
      }
    }

    /// <summary>Field number for the "pos" field.</summary>
    public const int PosFieldNumber = 3;
    private int pos_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Pos {
      get { return pos_; }
      set {
        pos_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as NGridPack);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(NGridPack other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (HeroID != other.HeroID) return false;
      if (LevelID != other.LevelID) return false;
      if (Pos != other.Pos) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (HeroID != 0) hash ^= HeroID.GetHashCode();
      if (LevelID != 0) hash ^= LevelID.GetHashCode();
      if (Pos != 0) hash ^= Pos.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (HeroID != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(HeroID);
      }
      if (LevelID != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(LevelID);
      }
      if (Pos != 0) {
        output.WriteRawTag(24);
        output.WriteInt32(Pos);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (HeroID != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(HeroID);
      }
      if (LevelID != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(LevelID);
      }
      if (Pos != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Pos);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(NGridPack other) {
      if (other == null) {
        return;
      }
      if (other.HeroID != 0) {
        HeroID = other.HeroID;
      }
      if (other.LevelID != 0) {
        LevelID = other.LevelID;
      }
      if (other.Pos != 0) {
        Pos = other.Pos;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            HeroID = input.ReadInt32();
            break;
          }
          case 16: {
            LevelID = input.ReadInt32();
            break;
          }
          case 24: {
            Pos = input.ReadInt32();
            break;
          }
        }
      }
    }

  }

  public sealed partial class IPAndPortPack : pb::IMessage<IPAndPortPack> {
    private static readonly pb::MessageParser<IPAndPortPack> _parser = new pb::MessageParser<IPAndPortPack>(() => new IPAndPortPack());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<IPAndPortPack> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Proto.FightProtoReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public IPAndPortPack() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public IPAndPortPack(IPAndPortPack other) : this() {
      ip_ = other.ip_;
      port_ = other.port_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public IPAndPortPack Clone() {
      return new IPAndPortPack(this);
    }

    /// <summary>Field number for the "ip" field.</summary>
    public const int IpFieldNumber = 1;
    private string ip_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Ip {
      get { return ip_; }
      set {
        ip_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "port" field.</summary>
    public const int PortFieldNumber = 2;
    private int port_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Port {
      get { return port_; }
      set {
        port_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as IPAndPortPack);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(IPAndPortPack other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Ip != other.Ip) return false;
      if (Port != other.Port) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Ip.Length != 0) hash ^= Ip.GetHashCode();
      if (Port != 0) hash ^= Port.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Ip.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Ip);
      }
      if (Port != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(Port);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Ip.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Ip);
      }
      if (Port != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Port);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(IPAndPortPack other) {
      if (other == null) {
        return;
      }
      if (other.Ip.Length != 0) {
        Ip = other.Ip;
      }
      if (other.Port != 0) {
        Port = other.Port;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            Ip = input.ReadString();
            break;
          }
          case 16: {
            Port = input.ReadInt32();
            break;
          }
        }
      }
    }

  }

  public sealed partial class HeroAndPosPack : pb::IMessage<HeroAndPosPack> {
    private static readonly pb::MessageParser<HeroAndPosPack> _parser = new pb::MessageParser<HeroAndPosPack>(() => new HeroAndPosPack());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<HeroAndPosPack> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Proto.FightProtoReflection.Descriptor.MessageTypes[2]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public HeroAndPosPack() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public HeroAndPosPack(HeroAndPosPack other) : this() {
      list_ = other.list_.Clone();
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public HeroAndPosPack Clone() {
      return new HeroAndPosPack(this);
    }

    /// <summary>Field number for the "list" field.</summary>
    public const int ListFieldNumber = 1;
    private static readonly pb::FieldCodec<global::Proto.NGridPack> _repeated_list_codec
        = pb::FieldCodec.ForMessage(10, global::Proto.NGridPack.Parser);
    private readonly pbc::RepeatedField<global::Proto.NGridPack> list_ = new pbc::RepeatedField<global::Proto.NGridPack>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Proto.NGridPack> List {
      get { return list_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as HeroAndPosPack);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(HeroAndPosPack other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if(!list_.Equals(other.list_)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      hash ^= list_.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      list_.WriteTo(output, _repeated_list_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      size += list_.CalculateSize(_repeated_list_codec);
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(HeroAndPosPack other) {
      if (other == null) {
        return;
      }
      list_.Add(other.list_);
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            list_.AddEntriesFrom(input, _repeated_list_codec);
            break;
          }
        }
      }
    }

  }

  public sealed partial class MainPack : pb::IMessage<MainPack> {
    private static readonly pb::MessageParser<MainPack> _parser = new pb::MessageParser<MainPack>(() => new MainPack());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<MainPack> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Proto.FightProtoReflection.Descriptor.MessageTypes[3]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public MainPack() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public MainPack(MainPack other) : this() {
      returnCode_ = other.returnCode_;
      actionCode_ = other.actionCode_;
      ipAndPortPack_ = other.ipAndPortPack_ != null ? other.ipAndPortPack_.Clone() : null;
      word_ = other.word_;
      info_ = other.info_.Clone();
      heroAndPosList_ = other.heroAndPosList_.Clone();
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public MainPack Clone() {
      return new MainPack(this);
    }

    /// <summary>Field number for the "returnCode" field.</summary>
    public const int ReturnCodeFieldNumber = 1;
    private global::Proto.ReturnCode returnCode_ = global::Proto.ReturnCode.Success;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Proto.ReturnCode ReturnCode {
      get { return returnCode_; }
      set {
        returnCode_ = value;
      }
    }

    /// <summary>Field number for the "actionCode" field.</summary>
    public const int ActionCodeFieldNumber = 2;
    private global::Proto.ActionCode actionCode_ = global::Proto.ActionCode.Login;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Proto.ActionCode ActionCode {
      get { return actionCode_; }
      set {
        actionCode_ = value;
      }
    }

    /// <summary>Field number for the "ipAndPortPack" field.</summary>
    public const int IpAndPortPackFieldNumber = 3;
    private global::Proto.IPAndPortPack ipAndPortPack_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Proto.IPAndPortPack IpAndPortPack {
      get { return ipAndPortPack_; }
      set {
        ipAndPortPack_ = value;
      }
    }

    /// <summary>Field number for the "word" field.</summary>
    public const int WordFieldNumber = 4;
    private string word_ = "";
    /// <summary>
    ///需要传一个字符串时可以使用这个，例如记录这是哪个玩家开启的战斗
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Word {
      get { return word_; }
      set {
        word_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "info" field.</summary>
    public const int InfoFieldNumber = 5;
    private static readonly pb::FieldCodec<string> _repeated_info_codec
        = pb::FieldCodec.ForString(42);
    private readonly pbc::RepeatedField<string> info_ = new pbc::RepeatedField<string>();
    /// <summary>
    ///战斗日志
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<string> Info {
      get { return info_; }
    }

    /// <summary>Field number for the "heroAndPosList" field.</summary>
    public const int HeroAndPosListFieldNumber = 6;
    private static readonly pb::FieldCodec<global::Proto.HeroAndPosPack> _repeated_heroAndPosList_codec
        = pb::FieldCodec.ForMessage(50, global::Proto.HeroAndPosPack.Parser);
    private readonly pbc::RepeatedField<global::Proto.HeroAndPosPack> heroAndPosList_ = new pbc::RepeatedField<global::Proto.HeroAndPosPack>();
    /// <summary>
    ///默认0是攻击方（玩家），1是防守方
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Proto.HeroAndPosPack> HeroAndPosList {
      get { return heroAndPosList_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as MainPack);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(MainPack other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (ReturnCode != other.ReturnCode) return false;
      if (ActionCode != other.ActionCode) return false;
      if (!object.Equals(IpAndPortPack, other.IpAndPortPack)) return false;
      if (Word != other.Word) return false;
      if(!info_.Equals(other.info_)) return false;
      if(!heroAndPosList_.Equals(other.heroAndPosList_)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (ReturnCode != global::Proto.ReturnCode.Success) hash ^= ReturnCode.GetHashCode();
      if (ActionCode != global::Proto.ActionCode.Login) hash ^= ActionCode.GetHashCode();
      if (ipAndPortPack_ != null) hash ^= IpAndPortPack.GetHashCode();
      if (Word.Length != 0) hash ^= Word.GetHashCode();
      hash ^= info_.GetHashCode();
      hash ^= heroAndPosList_.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (ReturnCode != global::Proto.ReturnCode.Success) {
        output.WriteRawTag(8);
        output.WriteEnum((int) ReturnCode);
      }
      if (ActionCode != global::Proto.ActionCode.Login) {
        output.WriteRawTag(16);
        output.WriteEnum((int) ActionCode);
      }
      if (ipAndPortPack_ != null) {
        output.WriteRawTag(26);
        output.WriteMessage(IpAndPortPack);
      }
      if (Word.Length != 0) {
        output.WriteRawTag(34);
        output.WriteString(Word);
      }
      info_.WriteTo(output, _repeated_info_codec);
      heroAndPosList_.WriteTo(output, _repeated_heroAndPosList_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (ReturnCode != global::Proto.ReturnCode.Success) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) ReturnCode);
      }
      if (ActionCode != global::Proto.ActionCode.Login) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) ActionCode);
      }
      if (ipAndPortPack_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(IpAndPortPack);
      }
      if (Word.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Word);
      }
      size += info_.CalculateSize(_repeated_info_codec);
      size += heroAndPosList_.CalculateSize(_repeated_heroAndPosList_codec);
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(MainPack other) {
      if (other == null) {
        return;
      }
      if (other.ReturnCode != global::Proto.ReturnCode.Success) {
        ReturnCode = other.ReturnCode;
      }
      if (other.ActionCode != global::Proto.ActionCode.Login) {
        ActionCode = other.ActionCode;
      }
      if (other.ipAndPortPack_ != null) {
        if (ipAndPortPack_ == null) {
          IpAndPortPack = new global::Proto.IPAndPortPack();
        }
        IpAndPortPack.MergeFrom(other.IpAndPortPack);
      }
      if (other.Word.Length != 0) {
        Word = other.Word;
      }
      info_.Add(other.info_);
      heroAndPosList_.Add(other.heroAndPosList_);
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            ReturnCode = (global::Proto.ReturnCode) input.ReadEnum();
            break;
          }
          case 16: {
            ActionCode = (global::Proto.ActionCode) input.ReadEnum();
            break;
          }
          case 26: {
            if (ipAndPortPack_ == null) {
              IpAndPortPack = new global::Proto.IPAndPortPack();
            }
            input.ReadMessage(IpAndPortPack);
            break;
          }
          case 34: {
            Word = input.ReadString();
            break;
          }
          case 42: {
            info_.AddEntriesFrom(input, _repeated_info_codec);
            break;
          }
          case 50: {
            heroAndPosList_.AddEntriesFrom(input, _repeated_heroAndPosList_codec);
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code