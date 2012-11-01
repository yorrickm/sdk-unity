/*
Copyright (c) 2012, Run With Robots
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright
      notice, this list of conditions and the following disclaimer in the
      documentation and/or other materials provided with the distribution.
    * Neither the name of the roar.io library nor the
      names of its contributors may be used to endorse or promote products
      derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY RUN WITH ROBOTS ''AS IS'' AND ANY
EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL MICHAEL ANDERSON BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
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