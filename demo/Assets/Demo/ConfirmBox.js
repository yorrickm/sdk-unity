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
#pragma strict

private var confirmMessage:String;
private var callback : Function;

function Start () {

}

function Update () {

}

function OnGUI() {
	if(confirmMessage) {
		
		GUI.depth = -10;
		// Make an area at the center of the screen  
		//var tempColor = GUI.backgroundColor; 
		//GUI.backgroundColor = Color.black;
		GUILayout.BeginArea(RenderUtils.getCentredRect(200, 80), GUI.skin.GetStyle("Box"));	
		
		//GUI.backgroundColor = tempColor;
		GUI.color = Color.red;
		
		// make a label indicating the status.
		var labelStyle = GUI.skin.GetStyle("Label");
		labelStyle.alignment = TextAnchor.MiddleCenter; 
		GUILayout.Label(confirmMessage as String, labelStyle);
	
	    if(GUILayout.Button("OK")) {
			confirmMessage = null;
			callback();
		}
		
		// End the area we started above.
		GUILayout.EndArea();
	}
}

function Show(msg:String, cb:Function) {
	confirmMessage = msg;
	callback = cb;
}
