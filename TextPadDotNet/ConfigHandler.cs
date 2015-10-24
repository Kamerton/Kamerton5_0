using System;
using System.Configuration;
using System.Xml;
using System.IO;
using System.Reflection;

namespace Notepad.NET
{
	public class ConfigHandler
	{ 
		public ConfigHandler()
		{
		}

		static public void LoadConfigPath()
		{
			_configurationPath = Assembly.GetExecutingAssembly().GetName().CodeBase  + ".config";
			// remove file stuff
			if (_configurationPath.Substring(0, 5) == "file:")
			{
				_configurationPath = _configurationPath.Substring(5);
				while (_configurationPath[0] == '/')
				{
					_configurationPath = _configurationPath.Substring(1); // keep stripping path variable
				}
			}
		}

		/// <summary>
		/// This is the configuration file path property.  Set this before using this class
		/// </summary>
		static string _configurationPath = "App.Config";
		static public string ConfigurationPath
		{
			get
			{
				return _configurationPath ;
			}
			set
			{
				_configurationPath = value ;
			}
		}

		/// <summary>
		/// Replace the key and value pair in the configuration file
		/// </summary>
		/// <param name="key"></param>
		/// <param name="val"></param>
		static public void replaceConfigSettings(string key, string val)
		{
			XmlDocument xDoc = new XmlDocument();
			string sFileName = _configurationPath;
			try
			{
				// load the configuration file
				xDoc.Load(sFileName);

				// find the node of interest containing the key using XPATH
				XmlNode theNode = xDoc.SelectSingleNode(@"/configuration/appSettings/add[@key = '" + key + "\']");

				// Set the new value for the node
				if (theNode != null)
					theNode.Attributes["value"].Value = val;

				// lop off file prefix if it exists
				if(sFileName.StartsWith("file:///"))
					sFileName = sFileName.Remove(0,8);

				// save the new configuration settings
				xDoc.Save(sFileName);
				xDoc = null;
			}
			catch(Exception ex)
			{
#if DEBUG
				System.Windows.Forms.MessageBox.Show("replaceConfigSettings()"+ex.Message);
#else
         System.Windows.Forms.MessageBox.Show("Unable to save changes");
#endif
				xDoc = null;
			}
		}

		static public bool GetConfigSettingBool(string key)
		{
			return (GetConfigSetting(key).ToUpper() == "TRUE");
		}

		static public int GetConfigSettingInt(string key)
		{
			string val = GetConfigSetting(key);
			int result = 0;
			try
			{
				result = Convert.ToInt32(val);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message.ToString());
				return result;
			}

			return result;
		}

		static public float GetConfigSettingFloat(string key)
		{
			string val = GetConfigSetting(key);
			float result = 0;
			try
			{
				result = Convert.ToSingle(val);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message.ToString());
					return result;
			}

			return result;
		}


		/// <summary>
		/// Retrieve the configuration value given the key
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		static public string GetConfigSetting(string key)
		{
			XmlDocument xDoc = new XmlDocument();
			string sFileName = _configurationPath;
			// make sure the file exists, if not return
			if (File.Exists(sFileName) == false)
			{
				return "";
			}

			try
			{
				// load the configuration file
				xDoc.Load(sFileName);

				// find the node of interest from the key
				XmlNode theNode = xDoc.SelectSingleNode(@"/configuration/appSettings/add[@key = '" + key + "\']");

				// retrieve the nodes value if it exists
				if (theNode != null)
					return theNode.Attributes["value"].Value;

				xDoc = null;
			}
			catch(Exception ex)
			{
#if DEBUG
				System.Windows.Forms.MessageBox.Show("replaceConfigSettings()"+ex.Message);
#else
           System.Windows.Forms.MessageBox.Show("Unable to load changes");
#endif
				xDoc = null;
			}
 
			return "";
		}

	}
}