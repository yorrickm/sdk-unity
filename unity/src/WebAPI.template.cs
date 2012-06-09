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
using UnityEngine;


public class WebAPI : IWebAPI
{
  protected IRequestSender requestSender_;

  public WebAPI(IRequestSender requestSender)
  {
     requestSender_ = requestSender;
		
<% _.each( data.modules, function(m,i,l) {
     print( "    " +m.name + "_ = new " + capitalizeFirst(m.name)+"Actions(requestSender);\n" );
     } );
%>  }


<% _.each( data.modules, function(m,i,l) {
     print( "  public override I" + capitalizeFirst(m.name) + "Actions "+m.name+" { get { return "+m.name+"_; } }\n" );
     print( "  public " + capitalizeFirst(m.name)+"Actions " +m.name + "_;\n\n" );
     } );
%>

  public class APIBridge
  {
    protected IRequestSender api;
    public APIBridge( IRequestSender caller ) { api = caller; }
  }

<%
  _.each( data.modules, function(m,i,l) {
    var class_name = capitalizeFirst(m.name)+"Actions"
%>
  public class <%= class_name %> : APIBridge, I<%= class_name %>
  {
    public <%= class_name %>( IRequestSender caller ) : base(caller) {}

<% _.each( m.functions, function(f,j,ll) {
     url = f.url ? f.url : (m.name+"/"+f.name);
     obj = f.obj ? f.obj : "obj";
     print("    public void "+fix_reserved_word(f.name)+"( Hashtable obj, IRequestCallback<IXMLNode> cb)\n");
     print("    {\n");
     print("      api.make_call(\""+url+"\", "+obj+", cb);\n");
     print("    }\n\n");
} ) %>  }
<% } ) %>

}

