using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace PluginSageIntacct.DataContracts
{
	
#region Common
[XmlRoot(ElementName="control")]
public class Control { 

	[XmlElement(ElementName="status")] 
	public string Status { get; set; } 

	[XmlElement(ElementName="senderid")] 
	public string Senderid { get; set; } 

	[XmlElement(ElementName="controlid")] 
	public int Controlid { get; set; } 

	[XmlElement(ElementName="uniqueid")] 
	public bool Uniqueid { get; set; } 

	[XmlElement(ElementName="dtdversion")] 
	public double Dtdversion { get; set; } 
}

[XmlRoot(ElementName="authentication")]
public class Authentication { 

	[XmlElement(ElementName="status")] 
	public string Status { get; set; } 

	[XmlElement(ElementName="userid")] 
	public string Userid { get; set; } 

	[XmlElement(ElementName="companyid")] 
	public string Companyid { get; set; } 

	[XmlElement(ElementName="locationid")] 
	public object Locationid { get; set; } 

	[XmlElement(ElementName="sessiontimestamp")] 
	public DateTime Sessiontimestamp { get; set; } 

	[XmlElement(ElementName="sessiontimeout")] 
	public DateTime Sessiontimeout { get; set; } 
}
#endregion

#region ReadResponse

// [XmlRoot(ElementName="CUSTOMER")]
// public class ReadResponseCUSTOMER { 
//
// 	[XmlElement(ElementName="CUSTOMERID")] 
// 	public string CUSTOMERID { get; set; } 
// 	
// 	[XmlElement(ElementName="NAME")] 
// 	public string NAME { get; set; } 
// }

[XmlRoot(ElementName="data")]
public class ReadResponseData { 

	[XmlElement(ElementName="CUSTOMER")] 
	public List<XmlNode> CUSTOMER { get; set; } 
	// public List<XmlElement> CUSTOMER { get; set; } 

	//have these be nullable, and populate only found object
	//read the desired obj only
	
	
	[XmlAttribute(AttributeName="listtype")] 
	public string Listtype { get; set; } 
	
	[XmlAttribute(AttributeName="totalcount")] 
	public int Totalcount { get; set; } 
	
	[XmlAttribute(AttributeName="offset")] 
	public int Offset { get; set; } 
	
	[XmlAttribute(AttributeName="count")] 
	public int Count { get; set; } 
	
	[XmlAttribute(AttributeName="numremaining")] 
	public int Numremaining { get; set; } 

	// [XmlText] 
	// public string Text { get; set; } 
}

[XmlRoot(ElementName="result")]
public class ReadResponseResult { 

	// [XmlElement(ElementName="status")] 
	// public string Status { get; set; } 
	//
	// [XmlElement(ElementName="function")] 
	// public string Function { get; set; } 
	//
	// [XmlElement(ElementName="controlid")] 
	// public string Controlid { get; set; } 

	[XmlElement(ElementName="data")] 
	public ReadResponseData Data { get; set; } 
}

[XmlRoot(ElementName="operation")]
public class ReadResponseOperation { 

	[XmlElement(ElementName="authentication")] 
	public Authentication Authentication { get; set; } 

	[XmlElement(ElementName="result")] 
	public ReadResponseResult Result { get; set; } 
}

[XmlRoot(ElementName="response")]
public class ReadResponseResponse { 

	// [XmlElement(ElementName="control")] 
	// public Control Control { get; set; } 

	[XmlElement(ElementName="operation")] 
	public ReadResponseOperation Operation { get; set; } 
}

#endregion

#region ObjectDefinitionResponse
[XmlRoot(ElementName="response")]
public class ObjectDefinitionResponse { 

	// [XmlElement(ElementName="control")] 
	// public Control Control { get; set; } 

	[XmlElement(ElementName="operation")] 
	public ObjectDefinitionResponseOperation Operation { get; set; } 
}

[XmlRoot(ElementName="operation")]
public class ObjectDefinitionResponseOperation { 

	// [XmlElement(ElementName="authentication")] 
	// public Authentication Authentication { get; set; } 

	[XmlElement(ElementName="result")] 
	public ObjectDefinitionResponseResult Result { get; set; } 
}

[XmlRoot(ElementName="Field")]
public class Field { 

	[XmlElement(ElementName="ID")] 
	public string ID { get; set; } 
	
	[XmlElement(ElementName="LABEL")] 
	public string LABEL { get; set; } 
	
	[XmlElement(ElementName="DESCRIPTION")] 
	public string DESCRIPTION { get; set; } 
	
	[XmlElement(ElementName="REQUIRED")] 
	public bool REQUIRED { get; set; } 
	
	[XmlElement(ElementName="READONLY")] 
	public bool READONLY { get; set; } 
	
	[XmlElement(ElementName="DATATYPE")] 
	public string DATATYPE { get; set; } 
	
	[XmlElement(ElementName="ISCUSTOM")] 
	public bool ISCUSTOM { get; set; } 
	
	[XmlElement(ElementName="VALIDVALUES")] 
	public VALIDVALUES VALIDVALUES { get; set; } 
}

[XmlRoot(ElementName="VALIDVALUES")]
public class VALIDVALUES { 

	[XmlElement(ElementName="VALIDVALUE")] 
	public List<string> VALIDVALUE { get; set; } 
}

[XmlRoot(ElementName="Fields")]
public class Fields { 

	[XmlElement(ElementName="Field")] 
	public List<Field> FieldList { get; set; } 
}

[XmlRoot(ElementName="Relationship")]
public class Relationship { 

	[XmlElement(ElementName="OBJECTPATH")] 
	public string OBJECTPATH { get; set; } 

	[XmlElement(ElementName="OBJECTNAME")] 
	public string OBJECTNAME { get; set; } 

	[XmlElement(ElementName="LABEL")] 
	public string LABEL { get; set; } 

	[XmlElement(ElementName="RELATIONSHIPTYPE")] 
	public string RELATIONSHIPTYPE { get; set; } 

	[XmlElement(ElementName="RELATEDBY")] 
	public string RELATEDBY { get; set; } 
}

[XmlRoot(ElementName="Relationships")]
public class Relationships { 

	[XmlElement(ElementName="Relationship")] 
	public List<Relationship> Relationship { get; set; } 
}

[XmlRoot(ElementName="Type")]
public class Type { 

	[XmlElement(ElementName="Fields")] 
	public Fields Fields { get; set; } 

	[XmlElement(ElementName="Relationships")] 
	public Relationships Relationships { get; set; } 

	// [XmlAttribute(AttributeName="Name")] 
	// public string Name { get; set; } 
	//
	// [XmlAttribute(AttributeName="DocumentType")] 
	// public object DocumentType { get; set; } 
	//
	// [XmlText] 
	// public string Text { get; set; } 
}

[XmlRoot(ElementName="data")]
public class Data { 

	[XmlElement(ElementName="Type")] 
	public Type Type { get; set; } 
	
	[XmlAttribute(AttributeName="listtype")] 
	public string Listtype { get; set; } 
	
	[XmlAttribute(AttributeName="count")] 
	public int Count { get; set; } 
	
	[XmlText] 
	public string Text { get; set; } 
}

[XmlRoot(ElementName="result")]
public class ObjectDefinitionResponseResult { 

	// [XmlElement(ElementName="status")] 
	// public string Status { get; set; } 
	//
	// [XmlElement(ElementName="function")] 
	// public string Function { get; set; } 
	//
	// [XmlElement(ElementName="controlid")] 
	// public string Controlid { get; set; } 

	[XmlElement(ElementName="data")] 
	public Data Data { get; set; } 
}


#endregion

#region Auth Request

	[XmlRoot(ElementName="login")]
	public class AuthRequestLogin { 

		[XmlElement(ElementName="userid")] 
		public string Userid { get; set; } 

		[XmlElement(ElementName="companyid")] 
		public string Companyid { get; set; } 

		[XmlElement(ElementName="password")] 
		public string Password { get; set; } 
	}

	[XmlRoot(ElementName="authentication")]
	public class AuthRequestAuthentication { 

		[XmlElement(ElementName="login")] 
		public AuthRequestLogin Login { get; set; } 
	}

	[XmlRoot(ElementName="function")]
	public class AuthRequestFunction { 

		[XmlElement(ElementName="getAPISession")] 
		public object GetAPISession { get; set; } 

		[XmlAttribute(AttributeName="controlid")] 
		public string Controlid { get; set; } 
	}

	[XmlRoot(ElementName="content")]
	public class AuthRequestContent { 

		[XmlElement(ElementName="function")] 
		public AuthRequestFunction Function { get; set; } 
	}

	[XmlRoot(ElementName="request")]
	public class Request { 

		[XmlElement(ElementName="control")] 
		public Control Control { get; set; } 

		[XmlElement(ElementName="operation")] 
		public AuthRequestOperation Operation { get; set; } 
	}
	[XmlRoot(ElementName="operation")]
	public class AuthRequestOperation { 

		[XmlElement(ElementName="authentication")] 
		public Authentication Authentication { get; set; } 

		[XmlElement(ElementName="result")] 
		public AuthRequestFunction Function { get; set; } 
		
	}
	
	[XmlRoot(ElementName="errormessage")]
	public class AuthErrorMessage 
	{ 
		[XmlElement(ElementName="error")] 
		public AuthError Error { get; set; } 
	}
	
	[XmlRoot(ElementName="error")]
	public class AuthError 
	{ 
		[XmlElement(ElementName="errorno")] 
		public string ErrorNo { get; set; } 
		
		[XmlElement(ElementName="description")] 
		public string Description { get; set; } 
		
		[XmlElement(ElementName="description2")] 
		public string Description2 { get; set; } 
	}
	#endregion
	
#region Auth Response
[XmlRoot(ElementName="control")]
public class AuthResponseControl { 

	[XmlElement(ElementName="status")] 
	public string Status { get; set; } 

	[XmlElement(ElementName="senderid")] 
	public string Senderid { get; set; } 

	[XmlElement(ElementName="controlid")] 
	public string Controlid { get; set; } 

	[XmlElement(ElementName="uniqueid")] 
	public string Uniqueid { get; set; } 

	[XmlElement(ElementName="dtdversion")] 
	public string Dtdversion { get; set; } 
}

[XmlRoot(ElementName="authentication")]
public class AuthResponseAuthentication { 

	[XmlElement(ElementName="status")] 
	public string Status { get; set; } 

	[XmlElement(ElementName="userid")] 
	public string Userid { get; set; } 

	[XmlElement(ElementName="companyid")] 
	public string Companyid { get; set; } 

	[XmlElement(ElementName="locationid")] 
	public object Locationid { get; set; } 

	[XmlElement(ElementName="sessiontimestamp")] 
	public DateTime Sessiontimestamp { get; set; } 

	[XmlElement(ElementName="sessiontimeout")] 
	public DateTime Sessiontimeout { get; set; } 
}

[XmlRoot(ElementName="api")]
public class AuthResponseApi { 

	[XmlElement(ElementName="sessionid")] 
	public string Sessionid { get; set; } 

	[XmlElement(ElementName="endpoint")] 
	public string Endpoint { get; set; } 

	[XmlElement(ElementName="locationid")] 
	public object Locationid { get; set; } 
}

[XmlRoot(ElementName="data")]
public class AuthResponseData { 

	[XmlElement(ElementName="api")] 
	public AuthResponseApi Api { get; set; } 
}

[XmlRoot(ElementName="result")]
public class AuthResponseResult { 

	[XmlElement(ElementName="status")] 
	public string Status { get; set; } 

	[XmlElement(ElementName="function")] 
	public string Function { get; set; } 

	[XmlElement(ElementName="controlid")] 
	public string Controlid { get; set; } 

	[XmlElement(ElementName="data")] 
	public AuthResponseData Data { get; set; } 
}

[XmlRoot(ElementName="operation")]
public class AuthResponseOperation { 

	[XmlElement(ElementName="authentication")] 
	public AuthResponseAuthentication Authentication { get; set; } 

	[XmlElement(ElementName="result")] 
	public AuthResponseResult? Result { get; set; } 
	
	[XmlElement(ElementName="errormessage")] 
	public AuthErrorMessage? ErrorMessage { get; set; } 
}

[XmlRoot(ElementName="response")]
public class AuthResponse { 

	[XmlElement(ElementName="control")] 
	public AuthResponseControl Control { get; set; } 

	[XmlElement(ElementName="operation")] 
	public AuthResponseOperation Operation { get; set; } 
}

#endregion

#region old - delete?
    //old below
    public class ObjectResponseWrapper
    {
        [JsonProperty("results")]
        public List<ObjectResponse> Results { get; set; }
        
        [JsonProperty("paging")]
        public PagingResponse Paging { get; set; }
    }

    public class ObjectResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("properties")]
        public Dictionary<string, object> Properties { get; set; }
    }


    public class PropertyResponseWrapper
    {
        [JsonProperty("results")]
        public List<PropertyResponse> Results { get; set; }
        
        [JsonProperty("paging")]
        public PagingResponse Paging { get; set; }
    }

    public class PropertyResponse
    {
        [JsonProperty("name")]
        public string Id { get; set; }
        
        [JsonProperty("label")]
        public string Name { get; set; }
        
        [JsonProperty("description")]
        public string Description { get; set; }
        
        [JsonProperty("hasUniqueValue")]
        public bool IsKey { get; set; }
        
        [JsonProperty("calculated")]
        public bool Calculated { get; set; }
        
        [JsonProperty("type")]
        public string Type { get; set; }
        
        [JsonProperty("modificationMetadata")]
        public ModificationMetaData ModificationMetaData { get; set; }
    }

    public class PagingResponse
    {
        [JsonProperty("next")]
        public NextResponse Next { get; set; }
    }

    public class NextResponse
    {
        [JsonProperty("after")]
        public string After { get; set; }
        
        [JsonProperty("link")]
        public string Link { get; set; }
    }
    #endregion
}