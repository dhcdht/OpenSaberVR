using System.Collections.Generic;
using Boomlagoon.JSON;
using UnityEngine;

public class JSONObjectTester : MonoBehaviour {

	public GUIText infoText;

	private string stringToEvaluate = @"{""web-app"": {
  ""servlet"": [   
    {
      ""servlet-name"": ""cofaxCDS"",
      ""servlet-class"": ""org.cofax.cds.CDSServlet"",
      ""init-param"": {
        ""configGlossary:installationAt"": ""Philadelphia, PA"",
        ""configGlossary:adminEmail"": ""ksm@pobox.com"",
        ""configGlossary:poweredBy"": ""Cofax"",
        ""configGlossary:poweredByIcon"": ""/images/cofax.gif"",
        ""configGlossary:staticPath"": ""/content/static"",
        ""templateProcessorClass"": ""org.cofax.WysiwygTemplate"",
        ""templateLoaderClass"": ""org.cofax.FilesTemplateLoader"",
        ""templatePath"": ""templates"",
        ""templateOverridePath"": """",
        ""defaultListTemplate"": ""listTemplate.htm"",
        ""defaultFileTemplate"": ""articleTemplate.htm"",
        ""useJSP"": false,
        ""jspListTemplate"": ""listTemplate.jsp"",
        ""jspFileTemplate"": ""articleTemplate.jsp"",
        ""cachePackageTagsTrack"": 200,
        ""cachePackageTagsStore"": 200,
        ""cachePackageTagsRefresh"": 60,
        ""cacheTemplatesTrack"": 100,
        ""cacheTemplatesStore"": 50,
        ""cacheTemplatesRefresh"": 15,
        ""cachePagesTrack"": 200,
        ""cachePagesStore"": 100,
        ""cachePagesRefresh"": 10,
        ""cachePagesDirtyRead"": 10,
        ""searchEngineListTemplate"": ""forSearchEnginesList.htm"",
        ""searchEngineFileTemplate"": ""forSearchEngines.htm"",
        ""searchEngineRobotsDb"": ""WEB-INF/robots.db"",
        ""useDataStore"": true,
        ""dataStoreClass"": ""org.cofax.SqlDataStore"",
        ""redirectionClass"": ""org.cofax.SqlRedirection"",
        ""dataStoreName"": ""cofax"",
        ""dataStoreDriver"": ""com.microsoft.jdbc.sqlserver.SQLServerDriver"",
        ""dataStoreUrl"": ""jdbc:microsoft:sqlserver://LOCALHOST:1433;DatabaseName=goon"",
        ""dataStoreUser"": ""sa"",
        ""dataStorePassword"": ""dataStoreTestQuery"",
        ""dataStoreTestQuery"": ""SET NOCOUNT ON;select test='test';"",
        ""dataStoreLogFile"": ""/usr/local/tomcat/logs/datastore.log"",
        ""dataStoreInitConns"": 10,
        ""dataStoreMaxConns"": 100,
        ""dataStoreConnUsageLimit"": 100,
        ""dataStoreLogLevel"": ""debug"",
        ""maxUrlLength"": 500}},
    {
      ""servlet-name"": ""cofaxEmail"",
      ""servlet-class"": ""org.cofax.cds.EmailServlet"",
      ""init-param"": {
      ""mailHost"": ""mail1"",
      ""mailHostOverride"": ""mail2""}},
    {
      ""servlet-name"": ""cofaxAdmin"",
      ""servlet-class"": ""org.cofax.cds.AdminServlet""},
 
    {
      ""servlet-name"": ""fileServlet"",
      ""servlet-class"": ""org.cofax.cds.FileServlet""},
    {
      ""servlet-name"": ""cofaxTools"",
      ""servlet-class"": ""org.cofax.cms.CofaxToolsServlet"",
      ""init-param"": {
        ""templatePath"": ""toolstemplates/"",
        ""log"": 1,
        ""logLocation"": ""/usr/local/tomcat/logs/CofaxTools.log"",
        ""logMaxSize"": """",
        ""dataLog"": 1,
        ""dataLogLocation"": ""/usr/local/tomcat/logs/dataLog.log"",
        ""dataLogMaxSize"": """",
        ""removePageCache"": ""/content/admin/remove?cache=pages&id="",
        ""removeTemplateCache"": ""/content/admin/remove?cache=templates&id="",
        ""fileTransferFolder"": ""/usr/local/tomcat/webapps/content/fileTransferFolder"",
        ""lookInContext"": 1,
        ""adminGroupID"": 4,
        ""betaServer"": true}}],
  ""servlet-mapping"": {
    ""cofaxCDS"": ""/"",
    ""cofaxEmail"": ""/cofaxutil/aemail/*"",
    ""cofaxAdmin"": ""/admin/*"",
    ""fileServlet"": ""/static/*"",
    ""cofaxTools"": ""/tools/*""},
 
  ""taglib"": {
    ""taglib-uri"": ""cofax.tld"",
    ""taglib-location"": ""/WEB-INF/tlds/cofax.tld""}}}";

	void Start() {
		infoText.gameObject.active = false;

		//JSONObject usage example:

		//Parse string into a JSONObject:
		JSONObject jsonObject = JSONObject.Parse(stringToEvaluate);
		if (jsonObject == null) { //Just to shut up Unity's 'unused variable' warning
		}

		//You can also create an "empty" JSONObject
		JSONObject emptyObject = new JSONObject();

		//Adding values is easy (values are implicitly converted to JSONValues):
		emptyObject.Add("key", "value");
		emptyObject.Add("otherKey", 123);
		emptyObject.Add("thirdKey", false);
		emptyObject.Add("fourthKey", new JSONValue(JSONValueType.Null));

		//You can iterate through all values with a simple for-each loop
		foreach (KeyValuePair<string, JSONValue> pair in emptyObject) {
			Debug.Log("key : value -> " + pair.Key + " : " + pair.Value);
			
			//Each JSONValue has a JSONValueType that tells you what type of value it is. Valid values are: String, Number, Object, Array, Boolean or Null.
			Debug.Log("pair.Value.Type.ToString() -> " + pair.Value.Type.ToString());

			if (pair.Value.Type == JSONValueType.Number) {
				//You can access values with the properties Str, Number, Obj, Array and Boolean
				Debug.Log("Value is a number: " + pair.Value.Number);
			}
		}

		//JSONObject's can also be created using this syntax:
		JSONObject newObject = new JSONObject {
			{"key", "value"},
			{"otherKey", 123},
			{"thirdKey", false}
		};

		//JSONObject overrides ToString() and outputs valid JSON
		Debug.Log("newObject.ToString() -> " + newObject.ToString());

		//JSONObjects support array accessors
		Debug.Log("newObject[\"key\"].Str -> " + newObject["key"].Str);

		//It also has a method to do the same
		Debug.Log("newObject.GetValue(\"otherKey\").ToString() -> " + newObject.GetValue("otherKey").ToString());

		//As well as a method to determine whether a key exists or not
		Debug.Log("newObject.ContainsKey(\"NotAKey\") -> " + newObject.ContainsKey("NotAKey"));

		//Elements can removed with Remove() and the whole object emptied with Clear()
		newObject.Remove("key");
		Debug.Log("newObject with \"key\" removed: " + newObject.ToString());
		
		newObject.Clear();
		Debug.Log("newObject cleared: " + newObject.ToString());
	}
	
	void OnGUI() {
		stringToEvaluate = GUI.TextArea(new Rect(0, 0, Screen.width - 300, Screen.height - 5), stringToEvaluate);

		if (GUI.Button(new Rect(Screen.width - 150, 10, 145, 75), "Evaluate JSON")) {
			var jsonObject = JSONObject.Parse(stringToEvaluate);
			if (jsonObject == null) {
				Debug.LogError("Failed to parse string, JSONObject == null");
			} else {
				Debug.Log("Succesfully created JSONObject");
				Debug.Log(jsonObject.ToString());
			}
		}

		if (GUI.Button(new Rect(Screen.width - 150, 95, 145, 75), "Clear textbox")) {
			stringToEvaluate = string.Empty;
		}
	}
}
