using UnityEngine;
using System.Collections;
using System.Linq;

public class TextXMLParser : MonoBehaviour {
	
	void Check( bool condition, string mesg )
	{
		if(!condition) Debug.LogError(mesg);
	}
	
	void Awake() 
	{
		Debug.Log ("Checking the XML Parser classes!");
		
		XMLParser parser=new XMLParser();
        XMLNode node=parser.Parse("<example><value type=\"String\">Foobar</value><value type=\"Int\">3</value></example>");
		
		/*
		 * e.g. the above XML is parsed to:
 		 * node= // The root has no name and no text, and one child : "example"
 		 * { 
 		 *   "_text":"",
 		 *   "example":
 		 *   [ 
 		 *      //This is the single example node.
 		 *	    {
 		 *        "_name":"example",
 		 *        "_text":"", 
 		 *          //It has two children
 		 *	      "value":
 		 *        [ 
 		 *           //First value node
 		 *          { 
 		 *             "_name":"value",
 		 *             "_text":"Foobar",
 		 *             "@type":"String"
 		 *          }, 
 		 *           // Second value node
 		 *          {
 		 *            "_name":"value",
 		 *            "_text":"3",
 		 *            "@type":"Int"
 		 *          }
 		 *       ]
 	     *     } 
         *  ],
         * }
         */
		
		//Check the parameters on the root 
		  Check( node.Count == 2 , "Incorrect number of members in root node");
		  Check( node["example"] != null, "No example node in root node found");
		  Check ( node["_text"] != null, "No _text in root node!");
		  Check( node["_text"] as string == "", "Text not as expected in root node : "+( node["_text"] as string) );
		
		//Get the example node from the node.
		ArrayList node_list = node["example"] as ArrayList;
		  Check(node_list as ArrayList != null, "Expected children to be an array!" + node["example"].GetType() );
		  Check(node_list.Count == 1,"Node list of incorrect length");
		
		//Check the first child.
		Hashtable e = node_list[0] as Hashtable;
		  Check( e != null, "Expected node to be of type Hashtable but instead its "+ e.GetType() );
		  Check ( e.Count == 3, "expected 3 members, got " + e.Count.ToString()+"\n"+ string.Join(" ", e.Keys.Cast<string>().Select( x=>":"+x+":").ToArray() ) );
		  Check ( e["_text"] as string == "", "Expected an empty _text field");
		  Check ( e["_name"] as string == "example", "Expected name to be example");
		
		
		ArrayList value_nodes = e["value"] as ArrayList;
		  Check( value_nodes!=null, "Expected value nodes");
		  Check ( value_nodes.Count == 2, "value node should have two entries");
		
		Hashtable v1 = value_nodes[0] as Hashtable;
		  Check( v1 != null, "Expected node to be of type Hashtable but instead its "+ v1.GetType() );
		  Check ( v1.Count == 3, "expected 3 members, got " + v1.Count.ToString()+"\n"+ string.Join(" ", v1.Keys.Cast<string>().Select( x=>":"+x+":").ToArray() ) );
		
		  foreach( DictionaryEntry d in v1 )
		  {
			Debug.Log ( d.Key as string + " => " + d.Value.GetType() +":"+ d.Value.ToString()  );
		  }
			
		
		  Check ( v1["_text"] as string == "Foobar", "Expected an empty _text field");
		  Check ( v1["_name"] as string == "value", "Expected name to be example");		
		
		
		
			
		
	}
}
