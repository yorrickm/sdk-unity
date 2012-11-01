
/*
 * Based on UnityScript Lieghtweight XML Parser by
 * Fraser McCormick whic is licensed as:
 *
 * UnityScript Lightweight XML Parser
 * by Fraser McCormick (unityscripts@roguishness.com)
 * http://twitter.com/flimgoblin
 * http://www.roguishness.com/unity/
 *
 * You may use this script under the terms of either the MIT License
 * or the Gnu Lesser General Public License (LGPL) Version 3.
 * See:
 * http://www.roguishness.com/unity/lgpl-3.0-standalone.html
 * http://www.roguishness.com/unity/gpl-3.0-standalone.html
 * or
 * http://www.roguishness.com/unity/MIT-license.txt
 *
 * Ported to C# by Michael Anderson (michael.anderson@runwithrobots.com)
 */
using System.Collections;
using System.Collections.Generic;
using System;

public interface IXMLNode
{
	string GetAttribute(string key);
	IEnumerable< KeyValuePair<string,string> > Attributes { get; }
	IEnumerable< IXMLNode > Children { get; }
	IXMLNode GetFirstChild( string key );
	string Text { get; set; }
	string Name { get; }

	string DebugAsString();

	List<IXMLNode> GetNodeList(string path);
	IXMLNode GetNode(string path);
	string GetValue(string path);
}

public class XMLNode : IXMLNode
{
	public string GetAttribute(string key)
	{
		string retval = null;
		Attributes_.TryGetValue(key, out retval);
		return retval;
	}

	public IEnumerable< KeyValuePair<string,string> > Attributes
	{
		get
		{
			foreach( KeyValuePair<string,string> kv in Attributes_ )
			{
				yield return kv;
			}
		}
	}

	public IEnumerable< IXMLNode > Children
	{
		get
		{
			foreach( KeyValuePair<string,XMLNodeList> nl in Children_ )
			{
				foreach( XMLNode n in nl.Value )
				{
					yield return n;
				}
			}
		}
	}

	public List<IXMLNode> ChildList()
	{
		return new List<IXMLNode>(Children);
	}

	public List<KeyValuePair<string,string> > AttributeList()
	{
		return new List<KeyValuePair<string, string>>(Attributes);
	}

	public IXMLNode GetFirstChild( string key )
	{
		XMLNodeList nodes=null;
		Children_.TryGetValue(key, out nodes);
		if(nodes==null || nodes.Count==0) { return null; }
		return nodes[0];
	}

	protected IDictionary<string, XMLNodeList> Children_;

	protected IDictionary<string, string> Attributes_;
	public string Name { get; set; }
	public string Text { get; set; }

	public XMLNode()
	{
		this.Children_ = new Dictionary<string, XMLNodeList>();
		this.Attributes_ = new Dictionary<string, string>();
	}

	public XMLNode( string name, string text )
	{
		Name = name;
		Text = text;
		Children_ = new Dictionary<string,XMLNodeList>();
		Attributes_ = new Dictionary<string,string>();
	}

	public List<IXMLNode> GetNodeList( string path)
	{
		XMLNodeList nl = GetObject(path) as XMLNodeList;
		List<IXMLNode> retval = new List<IXMLNode>();
		if(nl != null) {
			foreach( XMLNode n in nl )
			{
				retval.Add(n);
			}
		}
		return retval;
	}

	public IXMLNode GetNode( string path)
	{
		return GetObject(path) as IXMLNode;
	}

	public string GetValue( string path)
	{
		return GetObject(path) as string;
	}

	private object GetObject(string path)
	{
		string[] bits = path.Split(">"[0]);
		XMLNode currentNode=this;
		XMLNodeList currentNodeList = new XMLNodeList();
		bool listMode=false;
		object ob;
		for(int i=0;i<bits.Length;i++)
		{
			string segment = bits[i];
			if(listMode)
			{
				ob=currentNode= currentNodeList[int.Parse(segment)] as XMLNode;
				listMode=false;
			}
			else
			{
				if(currentNode.Children_.ContainsKey(segment)) {
					ob=currentNode.Children_[segment];
					currentNodeList=ob as XMLNodeList;
					listMode=true;
				}
				else
				{
					if(currentNode.Attributes_.ContainsKey(segment))
					{
						ob = currentNode.Attributes_[segment];
					}
					else
					{
						ob = currentNode.Text;
					}
					// reached a leaf node/attribute
					if(i!=(bits.Length-1))
					{
						// unexpected leaf node
						string actualPath="";
						for(int j=0;j<=i;j++)
						{
							actualPath=actualPath+">"+bits[j];
						}
					}
					return ob;
				}
			}
		}
		if(listMode) return currentNodeList;
		else return currentNode;
	}

	// Note: This is for debugging purposes only
	// It doesn't carefully escape XML entities etc. and will produce INVALID XML in many cases!
	public string DebugAsString()
	{
		string retval = "<"+Name;
		if( Attributes_.Count >0 )
		{
			retval+=" ";
			foreach(KeyValuePair<string,string> attribute in Attributes_ )
			{
				retval+=attribute.Key+"=\""+attribute.Value+"\" ";
			}
		}
		retval += ">";
		foreach(KeyValuePair<string,XMLNodeList> childs in Children_)
		{
			for(int i=0; i<childs.Value.Count; ++i)
			{
				XMLNode n = childs.Value[i];
				retval += n.DebugAsString();
			}
		}
		retval += Text;
		retval += "<"+Name+"/>";
		return retval;
	}


	public XMLNode AddChild( XMLNode n )
	{
		if( ! Children_.ContainsKey( n.Name ) )
		{
			Children_[n.Name] = new XMLNodeList();
		}
		Children_[n.Name].Add(n);
		return this;
	}

	public XMLNode AddAttribute( string K, string V)
	{
		Attributes_[K]=V;
		return this;
	}




	public string ShowAsBuildString( string indent )
	{
		string retval =  indent + "(new XMLNode(\""+Name+"\", \""+Text.Trim ()+"\"))\n";
		foreach( KeyValuePair<string,string> attribute_kv in Attributes_)
		{
			retval  += indent + "  .AddAttribute( \""+attribute_kv.Key+"\", \""+attribute_kv.Value+"\" )\n";
		}
		foreach( KeyValuePair<string,XMLNodeList> child_kv in Children_ )
		{
			foreach( XMLNode n in child_kv.Value )
			{
				retval  += indent + "  .AddChild(\n";
				retval  += n.ShowAsBuildString( indent+"    " );
				retval  += indent + "  )\n";
			}
		}
		return retval;
	}


	public class XMLParser
	{

	private const char LT='<';
	private const char GT='>';
	private const char SPACE=' ';
	private const char QUOTE='\"';
	private const char SLASH='/';
	private const char QMARK='?';
	private const char EQUALS='=';
	private const char EXCLAMATION='!';
	private const char DASH='-';
	private const char AMP='&';
	//private const char SQL='[';
	private const char SQR=']';

	public XMLNode Parse( string content )
	{
		XMLNode rootNode = new XMLNode();
		//rootNode["_text"]="";

		bool inElement=false;
		bool collectNodeName=false;
		bool collectAttributeName=false;
		bool collectAttributeValue=false;
		bool quoted=false;
		string attName="";
		string attValue="";
		string nodeName="";
		string textValue="";

		bool inMetaTag=false;
		bool inComment=false;
		bool inDoctype=false;
		bool inCDATA=false;

		XMLNodeList parents=new XMLNodeList();

		XMLNode currentNode=rootNode;
		for(var i=0;i<content.Length;i++)
		{
			char c=content[i];
			char cn='\0';
			char cnn='\0';
			char cp='\0';
			if((i+1)<content.Length) cn=content[i+1];
			if((i+2)<content.Length) cnn=content[i+2];
			if(i>0)cp=content[i-1];

			if(inMetaTag)
			{
				if(c==QMARK && cn==GT)
				{
					inMetaTag=false;
					i++;
				}
				continue;
			}
			else
			{
				if(!quoted && c==LT && cn==QMARK)
				{
					inMetaTag=true;
					continue;
				}
			}

			if(inDoctype)
			{
				if(cn==GT)
				{
					inDoctype=false;
					i++;
				}
				continue;
			}
			else if(inComment)
			{
				if(cp==DASH && c==DASH && cn==GT)
				{
					inComment=false;
					i++;
				}
				continue;
			}
			else
			{
				if(!quoted && c==LT && cn==EXCLAMATION)
				{
					if(content.Length>i+9 && content.Substring(i,9)=="<![CDATA[")
					{
						inCDATA=true;
						i+=8;
					}
					else if(content.Length > i+9 && content.Substring(i,9)=="<!DOCTYPE")
					{
						inDoctype=true;
						i+=8;
					}
					else if(content.Length > i+2 && content.Substring(i,4)=="<!--")
					{
						inComment=true;
						i+=3;
					}
					continue;
				}
			}

			if(inCDATA)
			{
				if(c==SQR && cn==SQR && cnn==GT)
				{
					inCDATA=false;
					i+=2;
					continue;
				}
				textValue+=c;
				continue;
			}


			if(inElement)
			{
				// Retrieving <NODE> name
				if(collectNodeName)
				{
					if(c==SPACE)
					{
						collectNodeName=false;
					}
					else if(c==GT)
					{
						collectNodeName=false;
						inElement=false;
					}
					else if(c==SLASH && cn==GT)
					{
						collectNodeName=false;
						i--;
					}


					if(!collectNodeName && nodeName.Length>0)
					{
						if(nodeName[0]==SLASH)
						{
							// close tag
							if(textValue.Length>0)
							{
								//currentNode["_text"]+=textValue;
								currentNode.Text += textValue;
							}

							textValue="";
							nodeName="";
							currentNode=parents[parents.Count-1];
							parents.RemoveAt(parents.Count-1);
						}
						else
						{
							if(textValue.Length>0)
							{
								//currentNode["_text"]+=textValue;
								currentNode.Text += textValue;
							}
							textValue="";
							XMLNode newNode=new XMLNode();
							newNode.Text ="";
							newNode.Name=nodeName;

							if(!currentNode.Children_.ContainsKey(nodeName))
							{
								currentNode.Children_.Add(nodeName, new XMLNodeList());
							}
							XMLNodeList a = currentNode.Children_[nodeName];
							a.Add(newNode);
							parents.Add(currentNode);
							currentNode=newNode;
							nodeName="";
						}
					}
					else
					{
						nodeName+=c;
					}
				}
				else
				{

					// 1. Checking for self closing node />
					// while not in a quote
					if(!quoted && c==SLASH && cn==GT)
					{
						inElement=false;
						collectAttributeName=false;
						collectAttributeValue=false;
						if(attName!=null && attName!="")
						{
							if(attValue!=null && attValue!="")
							{
								currentNode.Attributes_[attName] = attValue;
							}
							else
							{
								currentNode.Attributes_[attName]="true";
							}
						}

						i++;
						currentNode = parents[parents.Count-1];
						parents.RemoveAt(parents.Count-1);
						attName="";
						attValue="";
					}

					// 2. Check for Closing a node >
					else if(!quoted && c==GT)
					{
						inElement=false;
						collectAttributeName=false;
						collectAttributeValue=false;
						if(attName!=null && attName!="")
						{
							currentNode.Attributes_[attName]=attValue;
						}

						attName="";
						attValue="";
					}

					// 3. Otherwise grab the attribute OR the attribValue
					else
					{
						// 3.a. Add to the attribute name
						if(collectAttributeName)
						{
							if(c==SPACE || c==EQUALS)
							{
								collectAttributeName=false;
								collectAttributeValue=true;
							}
							else
							{
								attName+=c;
							}
						}
						// 3.b. Or add to the attribute value
						//      if we're in AttributeValue mode
						else if(collectAttributeValue)
						{
							// Begin or end quoting
							if(c==QUOTE)
							{
								// End quoting if already in quote mode
								if(quoted)
								{
									collectAttributeValue=false;
									currentNode.Attributes_[attName]=attValue;
									attValue="";
									attName="";
									quoted=false;
								}
								// Otherwise ENTER quoting mode
								else quoted=true;
							}

							// For everything else
							else
							{
								// Add to value if we're in quote mode
								if(quoted) attValue+=c;

								// Otherwise if we're not in quote mode
								else
								{
									// And the character is a space, reset
									// the attribute register
									if(c==SPACE)
									{
										collectAttributeValue=false;
										//currentNode[ATTRIB_APPEND+attName]=attValue;
										currentNode.Attributes_[attName]=attValue;
										attValue="";
										attName="";
									}
								}
							}
						}

						// 3.c. If it's a space, do NOTHING
						else if(c==SPACE){}

						// 3.d. For anything else, switch to Grab Attribute mode
						else
						{
							collectAttributeName=true;
							attName=""+c;
							attValue="";
							quoted=false;
						}
					}
				}
			}
			else
			{
				if(c==LT)
				{
					inElement=true;
					collectNodeName=true;
				}
				else
				{
					// this is where we can check for the 5 xml entities
					// TODO: add support for unicode/decimal versions of entities.
					if(c==AMP)
					{
						if(content.Length>i+5 && content.Substring(i,5)=="&amp;")
						{
							textValue+='&';
							i+=4;
						}
						else if(content.Length>i+6 && content.Substring(i,6)=="&quot;")
						{
							textValue+='"';
							i+=5;
						}
						else if(content.Length>i+6 && content.Substring(i,6)=="&apos;")
						{
							textValue+='\'';
							i+=5;
						}
						else if(content.Length>i+4 && content.Substring(i,4)=="&lt;")
						{
							textValue+='<';
							i+=3;
						}
						else if(content.Length>i+4 && content.Substring(i,4)=="&gt;")
						{
							textValue+='>';
							i+=3;
						}
					}
					else
					{
						textValue+=c;
					}
				}

			}
		}

		return rootNode;
	}

}

}
