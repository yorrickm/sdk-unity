using System.Collections;
using System.Collections.Generic;

namespace Roar
{

public class Json
{

  public static string ArrayToJSON( ArrayList ar )
  {
    // Bail out if nothing passed correctly
    if (ar==null)
    {
      return "null";
    }

    string str = "[";
    for (int i=0; i<ar.Count; i++)
    {
      str += ObjectToJSON(ar[i]);
      if (i != (ar.Count-1)) str+= ",";
    }
    str += "]";
    return str;
  }

  public static string ObjectToJSON( object o )
  {
    if( o == null )
    {
      return "null";
    }
    else if( o is bool )
    {
      return ( (bool)o )? "true" : "false";
    }
    else if( o is string )
    {
      return StringToJSON( (string)o );
    }
    else if( o is Hashtable )
    {
      return HashToJSON ( (Hashtable)o );
    }
    else if( o is ArrayList )
    {
      return ArrayToJSON( (ArrayList)o );
    }
    // Could be a numeric type... we should really test .. but we'll just convert for now.
    else
    {
      return o.ToString();
    }
  }

  class KeyValuePair
  {
    public string Key;
    public string Value;
  };

  public static string HashToJSON( Hashtable h )
  {
    // Bail out if nothing passed correctly
    if (h==null)
    {
      return "null";
    }

    string str = "{";

    List<KeyValuePair> kvpairs = new List<KeyValuePair>();

    foreach ( DictionaryEntry kv in h )
    {
      KeyValuePair kvt = new KeyValuePair();
      kvt.Key = ObjectToJSON((string)kv.Key);
      kvt.Value = ObjectToJSON(kv.Value);
      kvpairs.Add(kvt);
    }

    for(int i=0; i<kvpairs.Count; ++i)
    {
      str += kvpairs[i].Key + ":"+kvpairs[i].Value;
      if( i != kvpairs.Count-1 ) str+=",";
    }
    str += "}";
    return str;
  }

  public static string StringToJSON( string s )
  {
    //TODO: We need to properly escape this string!
    return "\"" + s + "\"";
  }
}

}
